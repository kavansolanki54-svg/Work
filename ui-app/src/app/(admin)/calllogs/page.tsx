"use client";

import { useQuery } from "@tanstack/react-query";
import { PhoneCall, Clock, Calendar, User, Search, PhoneIncoming, PhoneOutgoing, PhoneMissed } from "lucide-react";
import { callLogService, PhoneCallLog } from "@/services/api/callLog.service";
import { getFullUrl } from "@/services/api/apiConfig";
import { useAuthStore } from "@/store/useAuthStore";
import { useState } from "react";

export default function CallLogsPage() {
    const { user } = useAuthStore();
    const employeeId = user?.employeeID || 0;
    const [searchTerm, setSearchTerm] = useState("");

    const { data: logs = [], isLoading } = useQuery({
        queryKey: ["callLogs", employeeId],
        queryFn: () => callLogService.getLogs(employeeId),
        select: (res) => res.data?.data || []
    });

    const filteredLogs = logs.filter((log: PhoneCallLog) => 
        log.phoneNumber?.includes(searchTerm) || 
        log.contactName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        log.employeeName?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const getCallTypeIcon = (type: string) => {
        switch (type?.toLowerCase()) {
            case "incoming": return <PhoneIncoming className="w-4 h-4 text-green-500" />;
            case "outgoing": return <PhoneOutgoing className="w-4 h-4 text-blue-500" />;
            case "missed": return <PhoneMissed className="w-4 h-4 text-red-500" />;
            case "rejected": return <PhoneMissed className="w-4 h-4 text-red-500" />;
            default: return <PhoneCall className="w-4 h-4 text-slate-500" />;
        }
    };

    const formatDuration = (seconds: number) => {
        const h = Math.floor(seconds / 3600);
        const m = Math.floor((seconds % 3600) / 60);
        const s = seconds % 60;
        
        if (h > 0) return `${h}h ${m}m ${s}s`;
        if (m > 0) return `${m}m ${s}s`;
        return `${s}s`;
    };

    return (
        <div className="flex flex-col gap-6 font-sans pb-10 animate-in fade-in duration-500">
            {/* TOP NAV */}
            <div className="w-full bg-white/90 backdrop-blur-md border border-slate-200 shadow-sm rounded-2xl sticky top-2 z-[30]">
                <div className="px-6 h-16 flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center text-white">
                            <PhoneCall className="w-5 h-5" />
                        </div>
                        <h1 className="text-lg font-bold text-slate-800">Synced Call Logs</h1>
                    </div>

                    <div className="flex items-center gap-3">
                        <div className="flex items-center gap-2 bg-slate-100 h-10 px-4 rounded-lg border border-slate-200 focus-within:ring-2 focus-within:ring-indigo-100 focus-within:border-indigo-400 transition-all">
                            <Search className="w-4 h-4 text-slate-400" />
                            <input 
                                type="text" 
                                placeholder="Search number or name..." 
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                className="bg-transparent border-none focus:ring-0 text-xs font-bold text-slate-700 outline-none w-48"
                            />
                        </div>
                    </div>
                </div>
            </div>

            {/* TABLE CONTAINER */}
            <div className="w-full bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full text-left border-collapse">
                        <thead>
                            <tr className="bg-slate-50 border-b border-slate-200 text-[11px] uppercase tracking-widest text-slate-500 font-bold">
                                <th className="px-6 py-4">Employee</th>
                                <th className="px-6 py-4">Contact</th>
                                <th className="px-6 py-4">Type</th>
                                <th className="px-6 py-4">Start Time</th>
                                <th className="px-6 py-4">Duration</th>
                                <th className="px-6 py-4">Synced On</th>
                                <th className="px-6 py-4">Recording</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100 text-sm text-slate-700">
                            {isLoading ? (
                                <tr>
                                    <td colSpan={7} className="px-6 py-12 text-center text-slate-400 font-medium">
                                        Loading call logs...
                                    </td>
                                </tr>
                            ) : filteredLogs.length === 0 ? (
                                <tr>
                                    <td colSpan={7} className="px-6 py-12 text-center text-slate-400 font-medium">
                                        No call logs found.
                                    </td>
                                </tr>
                            ) : (
                                filteredLogs.map((log: PhoneCallLog) => (
                                    <tr key={log.callLogId} className="hover:bg-slate-50/50 transition-colors">
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-3">
                                                <div className="w-8 h-8 rounded-full bg-indigo-100 flex items-center justify-center text-indigo-700 font-bold text-xs">
                                                    {log.employeeName.charAt(0)}
                                                </div>
                                                <span className="font-bold text-slate-700">{log.employeeName}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex flex-col">
                                                <span className="font-bold text-slate-800">{log.phoneNumber}</span>
                                                {log.contactName && <span className="text-xs text-slate-500">{log.contactName}</span>}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-2">
                                                {getCallTypeIcon(log.callType)}
                                                <span className="capitalize text-xs font-bold text-slate-600">{log.callType}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-2 text-slate-600">
                                                <Calendar className="w-4 h-4 text-slate-400" />
                                                {new Date(log.startTime).toLocaleString()}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-2 text-slate-600">
                                                <Clock className="w-4 h-4 text-slate-400" />
                                                <span className="font-mono text-xs">{formatDuration(log.durationInSeconds)}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className="text-xs text-slate-400">
                                                {new Date(log.createDate).toLocaleDateString()}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4">
                                            {log.recordingUrl ? (
                                                <audio controls className="h-8 w-48" preload="none">
                                                    <source src={getFullUrl(log.recordingUrl)} type="audio/mp4" />
                                                    Your browser does not support the audio element.
                                                </audio>
                                            ) : (
                                                <span className="text-xs text-slate-400 italic">No recording</span>
                                            )}
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}
