using System.Linq;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace DallyWorkReoprt.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IWorkReportRepository _reportRepo;
    private readonly IEmailSettingRepository _smtpRepo;
    private readonly IEmailRecipientRepository _recipientRepo;
    private readonly IMailTemplateRepository _templateRepo;
    private readonly IWorkLogRepository _workLogRepo;

    public EmailService(
        IWorkReportRepository reportRepo, 
        IEmailSettingRepository smtpRepo, 
        IEmailRecipientRepository recipientRepo,
        IMailTemplateRepository templateRepo,
        IWorkLogRepository workLogRepo)
    {
        _reportRepo = reportRepo;
        _smtpRepo = smtpRepo;
        _recipientRepo = recipientRepo;
        _templateRepo = templateRepo;
        _workLogRepo = workLogRepo;
    }

    public async Task SendDailyReportEmailAsync(int reportId)
    {
        var report = await _reportRepo.GetFullReportByIdAsync(reportId);
        if (report == null) throw new Exception("Report not found");

        var settings = await _smtpRepo.GetLatestAsync(report.EmployeeId);
        if (settings == null) throw new Exception("Email settings not configured for this employee. Go to Email Settings to configure SMTP.");
        if (!settings.ActiveStatus) throw new Exception("SMTP gateway is currently deactivated. Please toggle 'ActiveStatus' in Email Settings to authorize email dispatches.");

        var recipients = _recipientRepo.GetAll(report.EmployeeId, activeStatus: true).ToList();
        if (!recipients.Any()) throw new Exception("No active email recipients found. Add recipients in Email Settings.");

        var template = await _templateRepo.GetTemplateAsync(report.Employee.CompanyId, report.EmployeeId);
        if (template == null) throw new Exception("No mail template found for this company.");

        using var client = new SmtpClient(settings.SmtpServer, settings.Port)
        {
            Credentials = new NetworkCredential(settings.SenderEmail, settings.Password),
            EnableSsl = true
        };

        var subject = template.SubjectFormat
            .Replace("{{Date}}", report.ReportDate.ToString("dd MMM yyyy"))
            .Replace("{{EmployeeName}}", report.Employee.EmployeeName);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(settings.SenderEmail, settings.SenderName),
            Subject = subject,
            Body = BuildEmailBodyFromTemplate(report, template),
            IsBodyHtml = true
        };

        foreach (var r in recipients)
        {
            if (r.RecipientType == "To") mailMessage.To.Add(r.Email);
            else if (r.RecipientType == "CC") mailMessage.CC.Add(r.Email);
            else if (r.RecipientType == "BCC") mailMessage.Bcc.Add(r.Email);
        }

        await client.SendMailAsync(mailMessage);
    }

    public async Task SendWorkLogEmailAsync(int workLogId)
    {
        // One individual log send (if needed)
        var log = await _workLogRepo.GetLogByIdAsync(workLogId);
        if (log == null) throw new Exception("Work log not found");
        await SendDailyWorkReportEmailAsync(log.CompanyId, log.EmployeeId, log.WorkDate); // ALWAYS SEND AS DAILY BATCH
    }

    public async Task SendDailyWorkReportEmailAsync(int companyId, int employeeId, DateTime date)
    {
        var logs = await _workLogRepo.GetLogsAsync(companyId, employeeId);
        var dailyLogs = logs.Where(x => x.WorkDate.Date == date.Date).OrderBy(x => x.CreateDate).ToList();

        if (!dailyLogs.Any()) throw new Exception("No work logs found for this date.");

        var log = dailyLogs.First();
        var settings = await _smtpRepo.GetLatestAsync(employeeId);
        if (settings == null) throw new Exception("Email configurations missing.");
        if (!settings.ActiveStatus) throw new Exception("SMTP gateway is currently deactivated. Please toggle 'Email Service' in Email Settings to authorize email dispatches.");

        var recipients = _recipientRepo.GetAll(employeeId, activeStatus: true).ToList();
        if (!recipients.Any()) throw new Exception("No active recipients.");

        var template = await _templateRepo.GetTemplateAsync(log.CompanyId, employeeId);
        if (template == null) throw new Exception("Template missing.");

        using var client = new SmtpClient(settings.SmtpServer, settings.Port)
        {
            Credentials = new NetworkCredential(settings.SenderEmail, settings.Password),
            EnableSsl = true
        };

        var subject = template.SubjectFormat
            .Replace("{{Date}}", date.ToString("dd MMM yyyy"))
            .Replace("{{EmployeeName}}", log.Employee.EmployeeName);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(settings.SenderEmail, settings.SenderName),
            Subject = subject,
            Body = BuildDailyWorkLogBody(dailyLogs, template, date),
            IsBodyHtml = true
        };

        foreach (var r in recipients)
        {
            if (r.RecipientType == "To") mailMessage.To.Add(r.Email);
            else if (r.RecipientType == "CC") mailMessage.CC.Add(r.Email);
            else if (r.RecipientType == "BCC") mailMessage.Bcc.Add(r.Email);
        }

        await client.SendMailAsync(mailMessage);
    }

    private string BuildDailyWorkLogBody(List<DAL.Models.WorkLog> logs, DAL.Models.MailTemplate template, DateTime date)
    {
        var config = new Dictionary<string, bool> { 
            { "srNo", true }, { "project", true }, { "status", true }, 
            { "title", true }, { "description", true }, { "duration", true } 
        };

        if (!string.IsNullOrEmpty(template.TableConfigJson))
        {
            try {
                var saved = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(template.TableConfigJson);
                if (saved != null) config = saved;
            } catch { /* fallback */ }
        }

        var sbRows = new StringBuilder();
        int srNo = 1;
        decimal totalDayHours = 0;

        foreach (var log in logs)
        {
            totalDayHours += log.InputTime;
            foreach (var task in log.WorkLogTasks)
            {
                sbRows.Append("<tr>");
                if (config.GetValueOrDefault("srNo"))
                    sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center;'>{srNo++}</td>");
                
                sbRows.Append("<td style='border: 1px solid #e2e8f0; padding: 12px;'>");
                if (config.GetValueOrDefault("project"))
                    sbRows.Append($"<div style='font-weight: bold; color: #1e293b; margin-bottom: 4px;'>{log.Project.ProjectName}</div>");
                
                sbRows.Append($"<div style='color: #64748b; font-size: 11px; margin-bottom: 6px;'>{log.Client.ClientName} ({log.Mode})</div>");
                
                if (config.GetValueOrDefault("description"))
                    sbRows.Append($"<div style='color: #475569; font-size: 13px;'>• {task.Description}</div>");
                sbRows.Append("</td>");

                if (config.GetValueOrDefault("duration"))
                    sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; color: #64748b;'>{log.InputTime} HH</td>");
                
                if (config.GetValueOrDefault("status"))
                    sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; font-weight: bold;'>{task.Status?.StatusName ?? "Done"}</td>");
                
                sbRows.Append("</tr>");
            }
        }

        // Professional aggregated table
        var fullTableSb = new StringBuilder();
        fullTableSb.Append("<table style='width:100%; border-collapse: collapse; border: 1px solid #e2e8f0; font-family: sans-serif; font-size: 13px;'>");
        fullTableSb.Append("<tr style='background-color: #f8fafc; color: #64748b; text-transform: uppercase; font-size: 11px; letter-spacing: 0.05em;'>");
        
        if (config.GetValueOrDefault("srNo"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; width: 40px;'>SR</th>");
        
        fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; text-align: left;'>Work Details</th>");
        
        if (config.GetValueOrDefault("duration"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; width: 80px;'>Time</th>");
        
        if (config.GetValueOrDefault("status"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; width: 100px;'>Status</th>");
        
        fullTableSb.Append("</tr>");
        fullTableSb.Append(sbRows.ToString());
        
        if (config.GetValueOrDefault("duration"))
        {
            fullTableSb.Append("<tr style='background-color: #f8fafc; font-weight: bold;'>");
            int colSpan = 1 + (config.GetValueOrDefault("srNo") ? 1 : 0);
            fullTableSb.Append($"<td colspan='{colSpan}' style='border: 1px solid #e2e8f0; padding: 12px; text-align: right;'>TOTAL DAY HOURS:</td>");
            fullTableSb.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center;'>{totalDayHours} HH</td>");
            if (config.GetValueOrDefault("status")) fullTableSb.Append("<td style='border: 1px solid #e2e8f0; padding: 12px;'></td>");
            fullTableSb.Append("</tr>");
        }
        
        fullTableSb.Append("</table>");        
        
        var body = (template.HeaderHtml ?? "") + template.BodyHtml + (template.FooterHtml ?? "");
        body = body
            .Replace("{{Date}}", date.ToString("dd MMMM yyyy"))
            .Replace("{{EmployeeName}}", logs.First().Employee?.EmployeeName ?? "Employee")
            .Replace("{{TasksTable}}", fullTableSb.ToString())
            .Replace("{{TaskRows}}", sbRows.ToString());

        if (body.Contains("{{#Rows}}") && body.Contains("{{/Rows}}"))
        {
            var startTag = "{{#Rows}}";
            var endTag = "{{/Rows}}";
            var startIndex = body.IndexOf(startTag);
            var endIndex = body.IndexOf(endTag);
            
            if (startIndex < endIndex)
            {
                var rowTemplate = body.Substring(startIndex + startTag.Length, endIndex - startIndex - startTag.Length);
                var allRowsSb = new StringBuilder();
                int rowSrNo = 1;
                
                foreach (var log in logs)
                {
                    var tasks = log.WorkLogTasks.OrderBy(x => x.CreateDate).ToList();
                    for (int i = 0; i < tasks.Count; i++)
                    {
                        var task = tasks[i];
                        var rowHtml = rowTemplate;
                        
                        // Proper Conditional Logic: Only the first task in a session gets the "FirstLog" headers
                        rowHtml = ProcessConditional(rowHtml, "{{#FirstLog}}", "{{/FirstLog}}", i == 0);
                        rowHtml = ProcessConditional(rowHtml, "{{#OtherLogs}}", "{{/OtherLogs}}", i > 0);
                        rowHtml = ProcessConditional(rowHtml, "{{#OtherEmployeeIds}}", "{{/OtherEmployeeIds}}", !string.IsNullOrEmpty(log.OtherEmployeeIds));

                        var statusName = task.Status?.StatusName ?? "Done";
                        rowHtml = rowHtml
                            .Replace("{{SrNo}}", rowSrNo.ToString())
                            .Replace("{{ProjectName}}", log.Project?.ProjectName ?? "")
                            .Replace("{{ClientName}}", log.Client?.ClientName ?? "")
                            .Replace("{{Time}}", log.InputTime.ToString())
                            .Replace("{{Mode}}", log.Mode ?? "")
                            .Replace("{{StatusName}}", statusName)
                            .Replace("{{OtherEmployeeIds}}", log.OtherEmployeeIds ?? "")
                            .Replace("{{Description}}", FormatDescription(task.Description, statusName, false))
                            .Replace("{{DescriptionStatus}}", FormatDescription(task.Description, statusName, true));
                        
                        allRowsSb.Append(rowHtml);
                    }
                    rowSrNo++;
                }

                body = body.Remove(startIndex, (endIndex + endTag.Length) - startIndex);
                body = body.Insert(startIndex, allRowsSb.ToString());
            }
        }
        else 
        {
            // NEW: Intelligent Multi-Log Fallback (if they didn't use {{#Rows}})
            var sbFinalBody = new StringBuilder();
            
            foreach (var log in logs)
            {
                var sessionHtml = body;
                var taskSb = new StringBuilder();
                foreach (var t in log.WorkLogTasks)
                {
                    if (string.IsNullOrWhiteSpace(t.Description)) continue;
                    taskSb.Append($"- {t.Description} - {t.Status?.StatusName ?? "Done"}<br/>");
                }

                sessionHtml = sessionHtml
                    .Replace("{{ProjectName}}", log.Project?.ProjectName ?? "")
                    .Replace("{{ClientName}}", log.Client?.ClientName ?? "")
                    .Replace("{{Time}}", log.InputTime.ToString())
                    .Replace("{{Mode}}", log.Mode ?? "")
                    .Replace("{{StatusName}}", log.WorkLogTasks.FirstOrDefault()?.Status?.StatusName ?? "Done")
                    .Replace("{{Description}}", taskSb.ToString());

                sessionHtml = ProcessConditional(sessionHtml, "{{#OtherEmployeeIds}}", "{{/OtherEmployeeIds}}", !string.IsNullOrEmpty(log.OtherEmployeeIds));
                sessionHtml = sessionHtml.Replace("{{OtherEmployeeIds}}", log.OtherEmployeeIds ?? "");
                
                sbFinalBody.Append(sessionHtml).Append("<hr style='border: 1px solid #eee; margin: 20px 0;' />");
            }
            body = sbFinalBody.ToString();
        }

        // Final Global Fallback for any remaining tags in Header/Footer (using first log)
        var globalFirstLog = logs.First();
        body = body
            .Replace("{{ProjectName}}", globalFirstLog.Project?.ProjectName ?? "")
            .Replace("{{ClientName}}", globalFirstLog.Client?.ClientName ?? "")
            .Replace("{{Time}}", globalFirstLog.InputTime.ToString())
            .Replace("{{Mode}}", globalFirstLog.Mode ?? "")
            .Replace("{{StatusName}}", globalFirstLog.WorkLogTasks.FirstOrDefault()?.Status?.StatusName ?? "Done");

        return body;
    }

    private string ProcessConditional(string html, string startTag, string endTag, bool condition)
    {
        if (!html.Contains(startTag)) return html;
        
        while (html.Contains(startTag))
        {
            int sIdx = html.IndexOf(startTag);
            int eIdx = html.IndexOf(endTag);
            if (sIdx == -1 || eIdx == -1 || sIdx > eIdx) break;

            if (condition)
            {
                html = html.Remove(eIdx, endTag.Length);
                html = html.Remove(sIdx, startTag.Length);
            }
            else
            {
                html = html.Remove(sIdx, (eIdx + endTag.Length) - sIdx);
            }
        }
        return html;
    }

    private string FormatDescription(string description, string statusName, bool includeStatus = true)
    {
        if (string.IsNullOrEmpty(description)) 
            return includeStatus ? $"• - {statusName}" : "";
            
        var lines = description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) 
            return includeStatus ? $"• - {statusName}" : "";
        
        var sb = new StringBuilder();
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            // Avoid double-bullets if description already has one
            if (!line.StartsWith("•") && !line.StartsWith("-") && !line.StartsWith("*"))
            {
                sb.Append("• ");
            }
            
            sb.Append(line);
            
            if (includeStatus) 
                sb.Append($" - {statusName}");
                
            if (i < lines.Length - 1) 
                sb.Append("<br/>");
        }
        return sb.ToString();
    }

    private string BuildEmailBodyFromWorkLog(DAL.Models.WorkLog log, DAL.Models.MailTemplate template)
    {
        var (fullTable, rowsOnly) = BuildWorkLogTable(log);
        
        var body = (template.HeaderHtml ?? "") + template.BodyHtml + (template.FooterHtml ?? "");

        body = body
            .Replace("{{Date}}", log.WorkDate.ToString("dd MMMM yyyy"))
            .Replace("{{EmployeeName}}", log.Employee.EmployeeName)
            .Replace("{{TasksTable}}", fullTable)
            .Replace("{{TaskRows}}", rowsOnly);

        return body;
    }

    private (string, string) BuildWorkLogTable(DAL.Models.WorkLog log)
    {
        var sbRows = new StringBuilder();
        int srNo = 1;

        foreach (var task in log.WorkLogTasks)
        {
            sbRows.Append("<tr>");
            sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center;'>{srNo++}</td>");
            sbRows.Append("<td style='border: 1px solid #e2e8f0; padding: 12px;'>");
            sbRows.Append($"<div style='font-weight: bold; color: #1e293b; margin-bottom: 4px;'>{log.Project.ProjectName}</div>");
            sbRows.Append($"<div style='color: #64748b; font-size: 13px;'>{log.Client.ClientName} ({log.Mode})</div>");
            sbRows.Append($"<div style='margin-top: 8px; color: #475569;'>• {task.Description}</div>");
            sbRows.Append("</td>");
            sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; color: #64748b;'>{log.InputTime} HH</td>");
            sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; font-weight: bold;'>{task.Status?.StatusName ?? "Done"}</td>");
            sbRows.Append("</tr>");
        }

        var rowsOnly = sbRows.ToString();
        var table = $@"<table style='width:100%; border-collapse: collapse; border: 1px solid #e2e8f0; font-family: sans-serif; font-size: 13px;'>
            <tr style='background-color: #f8fafc; color: #64748b;'>
                <th style='border: 1px solid #e2e8f0; padding: 12px; width: 40px;'>SR</th>
                <th style='border: 1px solid #e2e8f0; padding: 12px; text-align: left;'>Project & Task Details</th>
                <th style='border: 1px solid #e2e8f0; padding: 12px; width: 80px;'>Time</th>
                <th style='border: 1px solid #e2e8f0; padding: 12px; width: 80px;'>Status</th>
            </tr>
            {rowsOnly}
        </table>";

        return (table, rowsOnly);
    }

    private string BuildEmailBodyFromTemplate(DAL.Models.WorkReport report, DAL.Models.MailTemplate template)
    {
        var (fullTable, rowsOnly) = BuildTasksTableData(report, template);
        
        var body = (template.HeaderHtml ?? "") + template.BodyHtml + (template.FooterHtml ?? "");

        body = body
            .Replace("{{Date}}", report.ReportDate.ToString("dd MMMM yyyy"))
            .Replace("{{EmployeeName}}", report.Employee.EmployeeName)
            .Replace("{{TasksTable}}", fullTable)
            .Replace("{{TaskRows}}", rowsOnly);

        // Advanced Mode: {{#Rows}} ... {{/Rows}}
        if (body.Contains("{{#Rows}}") && body.Contains("{{/Rows}}"))
        {
            var startTag = "{{#Rows}}";
            var endTag = "{{/Rows}}";
            var startIndex = body.IndexOf(startTag);
            var endIndex = body.IndexOf(endTag);
            
            if (startIndex < endIndex)
            {
                var rowTemplate = body.Substring(startIndex + startTag.Length, endIndex - (startIndex + startTag.Length));
                var sbAllRows = new StringBuilder();
                int srNo = 1;

                foreach (var entry in report.WorkEntries.OrderBy(x => x.SrNo))
                {
                    var logs = entry.TimeLogs.OrderBy(x => x.InTime).ToList();
                    int logCount = Math.Max(1, logs.Count);

                    for (int i = 0; i < logCount; i++)
                    {
                        var log = logs.Count > 0 ? logs[i] : null;
                        var duration = "0:00";
                        if (log != null && TimeSpan.TryParse(log.InTime, out var t1) && TimeSpan.TryParse(log.OutTime, out var t2))
                        {
                            var diff = t2 - t1;
                            duration = $"{(int)diff.TotalHours}:{diff.Minutes:D2}";
                        }

                        var sName = entry.Status?.StatusName ?? "";
                        var rowHtml = rowTemplate
                            .Replace("{{SrNo}}", srNo.ToString())
                            .Replace("{{Title}}", entry.Title)
                            .Replace("{{Project}}", entry.Project?.ProjectName ?? "")
                            .Replace("{{Status}}", sName)
                            .Replace("{{Description}}", FormatDescription(entry.Description, sName, false))
                            .Replace("{{DescriptionStatus}}", FormatDescription(entry.Description, sName, true))
                            .Replace("{{StartTime}}", log?.InTime ?? "")
                            .Replace("{{EndTime}}", log?.OutTime ?? "")
                            .Replace("{{Duration}}", duration)
                            .Replace("{{RowSpan}}", logCount.ToString());
                        
                        // Handle {{#FirstLog}} and {{#OtherLogs}}
                        if (rowHtml.Contains("{{#FirstLog}}"))
                        {
                            var fStartToken = "{{#FirstLog}}";
                            var fEndToken = "{{/FirstLog}}";
                            if (i == 0) {
                                rowHtml = rowHtml.Replace(fStartToken, "").Replace(fEndToken, "");
                            } else {
                                int fSIdx = rowHtml.IndexOf(fStartToken);
                                int fEIdx = rowHtml.IndexOf(fEndToken);
                                if (fSIdx != -1 && fEIdx != -1)
                                    rowHtml = rowHtml.Remove(fSIdx, (fEIdx + fEndToken.Length) - fSIdx);
                            }
                        }

                        if (rowHtml.Contains("{{#OtherLogs}}"))
                        {
                            var oStartToken = "{{#OtherLogs}}";
                            var oEndToken = "{{/OtherLogs}}";
                            if (i > 0) {
                                rowHtml = rowHtml.Replace(oStartToken, "").Replace(oEndToken, "");
                            } else {
                                int oSIdx = rowHtml.IndexOf(oStartToken);
                                int oEIdx = rowHtml.IndexOf(oEndToken);
                                if (oSIdx != -1 && oEIdx != -1)
                                    rowHtml = rowHtml.Remove(oSIdx, (oEIdx + oEndToken.Length) - oSIdx);
                            }
                        }

                        sbAllRows.Append(rowHtml);
                    }
                    srNo++;
                }

                body = body.Remove(startIndex, (endIndex + endTag.Length) - startIndex);
                body = body.Insert(startIndex, sbAllRows.ToString());
            }
        }

        return body;
    }

    private (string, string) BuildTasksTableData(DAL.Models.WorkReport report, DAL.Models.MailTemplate template)
    {
        var config = new Dictionary<string, bool> { 
            { "srNo", true }, { "project", true }, { "status", true }, 
            { "title", true }, { "description", true }, { "inTime", true }, 
            { "outTime", true }, { "duration", true } 
        };

        if (!string.IsNullOrEmpty(template.TableConfigJson))
        {
            try {
                var saved = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, bool>>(template.TableConfigJson);
                if (saved != null) config = saved;
            } catch { /* fallback to default */ }
        }

        var sbRows = new StringBuilder();
        int srNo = 1;
        var entries = report.WorkEntries.OrderBy(x => x.SrNo).ToList();

        foreach (var entry in entries)
        {
            var logs = entry.TimeLogs.OrderBy(x => x.InTime).ToList();
            int logCount = Math.Max(1, logs.Count);

            for (int i = 0; i < logCount; i++)
            {
                sbRows.Append("<tr>");

                // Main Entry Columns (span rows if multiple time logs)
                if (i == 0)
                {
                    if (config.GetValueOrDefault("srNo"))
                        sbRows.Append($"<td rowspan='{logCount}' style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; vertical-align: top;'>{srNo++}</td>");

                    if (config.GetValueOrDefault("title") || config.GetValueOrDefault("project") || config.GetValueOrDefault("status") || config.GetValueOrDefault("description"))
                    {
                        sbRows.Append($"<td rowspan='{logCount}' style='border: 1px solid #e2e8f0; padding: 12px; vertical-align: top;'>");
                        
                        if (config.GetValueOrDefault("title"))
                            sbRows.Append($"<div style='font-weight: bold; color: #1e293b; margin-bottom: 4px;'>{entry.Title}</div>");
                        
                        if (config.GetValueOrDefault("project") || config.GetValueOrDefault("status"))
                        {
                            sbRows.Append("<div style='margin-bottom: 6px;'>");
                            if (config.GetValueOrDefault("project"))
                                sbRows.Append($"<span style='background: #f1f5f9; color: #64748b; padding: 2px 6px; border-radius: 4px; font-size: 11px; font-weight: bold; margin-right: 4px;'>{entry.Project?.ProjectName}</span>");
                            if (config.GetValueOrDefault("status"))
                            {
                                var statusColor = (entry.StatusId == 5 || entry.StatusId == 1) ? "#f97316" : "#3b82f6";
                                sbRows.Append($"<span style='background: {statusColor}15; color: {statusColor}; padding: 2px 6px; border-radius: 4px; font-size: 11px; font-weight: bold;'>{entry.Status?.StatusName}</span>");
                            }
                            sbRows.Append("</div>");
                        }

                        if (config.GetValueOrDefault("description") && !string.IsNullOrEmpty(entry.Description))
                        {
                            sbRows.Append("<div style='color: #64748b; font-size: 12px; line-height: 1.5;'>");
                            sbRows.Append(FormatDescription(entry.Description, entry.Status?.StatusName ?? "", false));
                            sbRows.Append("</div>");
                        }
                        sbRows.Append("</td>");
                    }
                }

                // Time Log Columns
                if (logs.Count > 0)
                {
                    var log = logs[i];
                    if (config.GetValueOrDefault("inTime"))
                        sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; color: #475569;'>{log.InTime}</td>");
                    if (config.GetValueOrDefault("outTime"))
                        sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; color: #475569;'>{log.OutTime}</td>");
                    
                    if (config.GetValueOrDefault("duration"))
                    {
                        var hoursFormatted = "0:00";
                        if (TimeSpan.TryParse(log.InTime, out var t1) && TimeSpan.TryParse(log.OutTime, out var t2))
                        {
                            var diff = t2 - t1;
                            hoursFormatted = $"{(int)diff.TotalHours}:{diff.Minutes:D2}";
                        }
                        sbRows.Append($"<td style='border: 1px solid #e2e8f0; padding: 12px; text-align: center; font-weight: bold; color: #0f172a;'>{hoursFormatted}</td>");
                    }
                }
                else
                {
                    if (config.GetValueOrDefault("inTime")) sbRows.Append("<td style='border: 1px solid #e2e8f0; padding: 12px;'></td>");
                    if (config.GetValueOrDefault("outTime")) sbRows.Append("<td style='border: 1px solid #e2e8f0; padding: 12px;'></td>");
                    if (config.GetValueOrDefault("duration")) sbRows.Append("<td style='border: 1px solid #e2e8f0; padding: 12px;'></td>");
                }
                sbRows.Append("</tr>");
            }
        }

        var rowsOnly = sbRows.ToString();
        var fullTableSb = new StringBuilder();
        fullTableSb.Append("<table style='width:100%; border-collapse: collapse; border: 1px solid #e2e8f0; font-family: -apple-system, BlinkMacSystemFont, \"Segoe UI\", Roboto, sans-serif; font-size: 13px;'>");
        fullTableSb.Append("<tr style='background-color: #f8fafc; text-align: center; font-weight: bold; color: #64748b;'>");
        
        if (config.GetValueOrDefault("srNo"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; width: 40px;'>SR</th>");
        
        if (config.GetValueOrDefault("title") || config.GetValueOrDefault("project") || config.GetValueOrDefault("status") || config.GetValueOrDefault("description"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; text-align: left;'>Work Details</th>");
        
        if (config.GetValueOrDefault("inTime"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; width: 80px;'>Start</th>");
        
        if (config.GetValueOrDefault("outTime"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; width: 80px;'>End</th>");
        
        if (config.GetValueOrDefault("duration"))
            fullTableSb.Append("<th style='border: 1px solid #e2e8f0; padding: 14px; width: 80px;'>Hours</th>");
        
        fullTableSb.Append("</tr>");
        fullTableSb.Append(rowsOnly);
        fullTableSb.Append("</table>");

        return (fullTableSb.ToString(), rowsOnly);
    }
}
