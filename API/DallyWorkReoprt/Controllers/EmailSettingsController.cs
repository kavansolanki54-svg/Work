using AutoMapper;
using DallyWorkReoprt.DAL.Interface;
using DallyWorkReoprt.DAL.Models;
using DallyWorkReoprt.DTO.Models;
using DallyWorkReoprt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DallyWorkReoprt.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmailSettingsController : BaseApiController
{
    private readonly IEmailRecipientRepository _recipientRepo;
    private readonly IEmailSettingRepository _smtpRepo;
    private readonly IMapper _mapper;

    public EmailSettingsController(
        IEmailRecipientRepository recipientRepo, 
        IEmailSettingRepository smtpRepo, 
        IMapper mapper)
    {
        _recipientRepo = recipientRepo;
        _smtpRepo = smtpRepo;
        _mapper = mapper;
    }

    #region Recipients

    [HttpGet("Recipients/{employeeId}")]
    public IActionResult GetRecipients(int employeeId)
    {
        var recipients = _recipientRepo.GetAll(employeeId).ToList();
        var dtos = _mapper.Map<List<EmailRecipientDTO>>(recipients);
        return Ok(ApiResponse<List<EmailRecipientDTO>>.SuccessResponse(dtos, "Recipients retrieved successfully"));
    }

    [HttpPost("Recipient/Save")]
    public async Task<IActionResult> SaveRecipient([FromBody] EmailRecipientDTO recipientDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var recipient = _mapper.Map<EmailRecipient>(recipientDTO);
        recipient.CreateDate = DateTime.Now;
        
        await _recipientRepo.AddAsync(recipient);
        await _recipientRepo.SaveAsync();

        return Ok(ApiResponse<EmailRecipient>.SuccessResponse(recipient, "Recipient saved successfully"));
    }

    [HttpDelete("Recipient/Delete/{id}")]
    public async Task<IActionResult> DeleteRecipient(int id)
    {
        var recipient = await _recipientRepo.GetByIdAsync(id);
        if (recipient == null) 
            return NotFound(ApiResponse<EmailRecipient>.ErrorResponse("Recipient not found"));

        _recipientRepo.Remove(id);
        await _recipientRepo.SaveAsync();

        return Ok(ApiResponse<bool>.SuccessResponse(true, "Recipient deleted successfully"));
    }

    #endregion

    #region SMTP Settings

    [HttpGet("Smtp/{employeeId}")]
    public async Task<IActionResult> GetSmtpSettings(int employeeId)
    {
        var settings = await _smtpRepo.GetLatestAsync(employeeId);
        if (settings == null)
            return Ok(ApiResponse<EmailSettingDTO>.SuccessResponse(new EmailSettingDTO { EmployeeId = employeeId }, "No settings found"));

        var dto = _mapper.Map<EmailSettingDTO>(settings);
        return Ok(ApiResponse<EmailSettingDTO>.SuccessResponse(dto, "SMTP settings retrieved successfully"));
    }

    [HttpPost("Smtp/Save")]
    public async Task<IActionResult> SaveSmtpSettings([FromBody] EmailSettingDTO smtpDTO)
    {
        if (!ModelState.IsValid) return ValidationErrorResponse();

        var existing = await _smtpRepo.GetLatestAsync(smtpDTO.EmployeeId ?? 0);
        if (existing == null)
        {
            var settings = _mapper.Map<EmailSetting>(smtpDTO);
            settings.CreatedAt = DateTime.Now;
            settings.CreateDate = DateTime.Now;
            await _smtpRepo.AddAsync(settings);
        }
        else
        {
            _mapper.Map(smtpDTO, existing);
            existing.CreateDate = DateTime.Now; // Update timestamp
            _smtpRepo.Update(existing);
        }

        await _smtpRepo.SaveAsync();
        return Ok(ApiResponse<EmailSettingDTO>.SuccessResponse(smtpDTO, "SMTP settings saved successfully"));
    }

    #endregion
}
