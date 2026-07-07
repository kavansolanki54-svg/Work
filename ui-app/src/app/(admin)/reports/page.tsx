"use client";

import { useState } from "react";
import { FileText, Download, FileSpreadsheet, Calendar as CalendarIcon } from "lucide-react";
import { getFullUrl } from "@/services/api/apiConfig";

export default function ReportsPage() {
    const [dateRange, setDateRange] = useState({ from: "", to: "" });

    const handleDownload = (format: 'pdf' | 'csv') => {
        const url = new URL(getFullUrl(`/api/Export/${format}`));
        if (dateRange.from) url.searchParams.append('from', dateRange.from);
        if (dateRange.to) url.searchParams.append('to', dateRange.to);
        
        // Use browser window to trigger download
        window.open(url.toString(), '_blank');
    };

    return (
        <div className="flex flex-col gap-6 font-sans pb-10 animate-in fade-in duration-500">
            {/* Header */}
            <div className="w-full bg-white/90 backdrop-blur-md border border-slate-200 shadow-sm rounded-2xl p-6">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center text-white">
                            <FileText className="w-5 h-5" />
                        </div>
                        <div>
                            <h1 className="text-xl font-bold text-slate-800">Reports Generator</h1>
                            <p className="text-sm text-slate-500">Export Call Analytics and Audit Logs to PDF or CSV</p>
                        </div>
                    </div>
                </div>
            </div>

            {/* Export Configurations */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col gap-6">
                    <div className="flex items-center gap-3 border-b border-slate-100 pb-4">
                        <CalendarIcon className="w-5 h-5 text-indigo-500" />
                        <h2 className="text-lg font-bold text-slate-800">Date Range Selection</h2>
                    </div>

                    <div className="flex flex-col gap-4">
                        <div className="flex flex-col gap-1.5">
                            <label className="text-sm font-semibold text-slate-600">Start Date</label>
                            <input 
                                type="date" 
                                value={dateRange.from}
                                onChange={(e) => setDateRange(prev => ({ ...prev, from: e.target.value }))}
                                className="border border-slate-200 rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-indigo-100 focus:border-indigo-400 outline-none text-sm text-slate-700"
                            />
                        </div>
                        <div className="flex flex-col gap-1.5">
                            <label className="text-sm font-semibold text-slate-600">End Date</label>
                            <input 
                                type="date" 
                                value={dateRange.to}
                                onChange={(e) => setDateRange(prev => ({ ...prev, to: e.target.value }))}
                                className="border border-slate-200 rounded-lg px-4 py-2.5 focus:ring-2 focus:ring-indigo-100 focus:border-indigo-400 outline-none text-sm text-slate-700"
                            />
                        </div>
                    </div>
                </div>

                <div className="flex flex-col gap-6">
                    <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col gap-4 hover:border-indigo-200 transition-colors group">
                        <div className="flex items-start justify-between">
                            <div className="flex items-center gap-3">
                                <div className="p-3 bg-red-50 text-red-600 rounded-xl group-hover:bg-red-100 transition-colors">
                                    <FileText className="w-6 h-6" />
                                </div>
                                <div>
                                    <h3 className="font-bold text-slate-800">PDF Report</h3>
                                    <p className="text-xs text-slate-500">Structured layout for printing and management review.</p>
                                </div>
                            </div>
                        </div>
                        <button 
                            onClick={() => handleDownload('pdf')}
                            className="w-full mt-2 flex items-center justify-center gap-2 bg-slate-50 border border-slate-200 text-slate-700 px-4 py-2.5 rounded-lg font-semibold hover:bg-slate-100 hover:text-indigo-600 transition-all text-sm"
                        >
                            <Download className="w-4 h-4" /> Download PDF
                        </button>
                    </div>

                    <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col gap-4 hover:border-indigo-200 transition-colors group">
                        <div className="flex items-start justify-between">
                            <div className="flex items-center gap-3">
                                <div className="p-3 bg-emerald-50 text-emerald-600 rounded-xl group-hover:bg-emerald-100 transition-colors">
                                    <FileSpreadsheet className="w-6 h-6" />
                                </div>
                                <div>
                                    <h3 className="font-bold text-slate-800">CSV Export</h3>
                                    <p className="text-xs text-slate-500">Raw tabular data ideal for Excel or custom analysis.</p>
                                </div>
                            </div>
                        </div>
                        <button 
                            onClick={() => handleDownload('csv')}
                            className="w-full mt-2 flex items-center justify-center gap-2 bg-slate-50 border border-slate-200 text-slate-700 px-4 py-2.5 rounded-lg font-semibold hover:bg-slate-100 hover:text-indigo-600 transition-all text-sm"
                        >
                            <Download className="w-4 h-4" /> Download CSV
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
