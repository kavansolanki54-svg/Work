"use client";

import { useQuery } from "@tanstack/react-query";
import { Smartphone, Battery, BatteryMedium, BatteryLow, Activity, Clock, RefreshCw } from "lucide-react";
import { deviceManagementService, DeviceInformationDTO } from "@/services/api/deviceManagement.service";

export default function DeviceManagementPage() {
    const { data: devices = [], isLoading, refetch, isRefetching } = useQuery({
        queryKey: ["deviceManagement"],
        queryFn: () => deviceManagementService.getDevices(),
        select: (res) => res.data?.data || []
    });

    const getBatteryIcon = (level: number) => {
        if (level > 60) return <Battery className="w-5 h-5 text-emerald-500" />;
        if (level > 20) return <BatteryMedium className="w-5 h-5 text-orange-500" />;
        return <BatteryLow className="w-5 h-5 text-red-500" />;
    };

    const getStatusColor = (lastSync: string) => {
        const syncDate = new Date(lastSync);
        const now = new Date();
        const diffHours = (now.getTime() - syncDate.getTime()) / (1000 * 60 * 60);

        if (diffHours < 24) return "bg-emerald-100 text-emerald-700"; // Active
        if (diffHours < 72) return "bg-orange-100 text-orange-700"; // Warning
        return "bg-red-100 text-red-700"; // Inactive
    };

    const getStatusText = (lastSync: string) => {
        const syncDate = new Date(lastSync);
        const now = new Date();
        const diffHours = (now.getTime() - syncDate.getTime()) / (1000 * 60 * 60);

        if (diffHours < 24) return "Healthy";
        if (diffHours < 72) return "Delayed Sync";
        return "Stale";
    };

    return (
        <div className="flex flex-col gap-6 font-sans pb-10 animate-in fade-in duration-500">
            {/* Header */}
            <div className="w-full bg-white/90 backdrop-blur-md border border-slate-200 shadow-sm rounded-2xl p-6">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center text-white">
                            <Smartphone className="w-5 h-5" />
                        </div>
                        <div>
                            <h1 className="text-xl font-bold text-slate-800">Device Management</h1>
                            <p className="text-sm text-slate-500">Monitor mobile device health and sync status across the company</p>
                        </div>
                    </div>
                    <button 
                        onClick={() => refetch()}
                        disabled={isRefetching}
                        className="flex items-center gap-2 bg-indigo-50 text-indigo-600 px-4 py-2 rounded-lg font-semibold hover:bg-indigo-100 transition-colors disabled:opacity-50"
                    >
                        <RefreshCw className={`w-4 h-4 ${isRefetching ? 'animate-spin' : ''}`} />
                        Refresh
                    </button>
                </div>
            </div>

            {/* Devices Table */}
            <div className="w-full bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full text-left border-collapse">
                        <thead>
                            <tr className="bg-slate-50 border-b border-slate-200 text-[11px] uppercase tracking-widest text-slate-500 font-bold">
                                <th className="px-6 py-4">Employee</th>
                                <th className="px-6 py-4">Device Info</th>
                                <th className="px-6 py-4">App Version</th>
                                <th className="px-6 py-4">Battery</th>
                                <th className="px-6 py-4">Last Sync</th>
                                <th className="px-6 py-4">Status</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100 text-sm text-slate-700">
                            {isLoading ? (
                                <tr>
                                    <td colSpan={6} className="px-6 py-12 text-center text-slate-400 font-medium">
                                        Loading devices...
                                    </td>
                                </tr>
                            ) : devices.length === 0 ? (
                                <tr>
                                    <td colSpan={6} className="px-6 py-12 text-center text-slate-400 font-medium">
                                        No devices registered yet.
                                    </td>
                                </tr>
                            ) : (
                                devices.map((device: DeviceInformationDTO) => (
                                    <tr key={device.deviceId} className="hover:bg-slate-50/50 transition-colors">
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-3">
                                                <div className="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center text-slate-600 font-bold text-xs uppercase">
                                                    {device.employeeName?.substring(0, 2) || "U"}
                                                </div>
                                                <span className="font-bold text-slate-700">{device.employeeName || "Unknown"}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex flex-col">
                                                <span className="font-bold text-slate-800">{device.manufacturer} {device.model}</span>
                                                <span className="text-xs text-slate-500">{device.platform} {device.osVersion}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className="font-mono text-xs bg-slate-100 text-slate-600 px-2 py-1 rounded-md">
                                                v{device.appVersion || "1.0.0"}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex items-center gap-2">
                                                {getBatteryIcon(device.batteryPercentage)}
                                                <span className="font-bold text-slate-700">{device.batteryPercentage}%</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="flex flex-col text-slate-600">
                                                <span className="font-medium">{new Date(device.lastSyncTime).toLocaleDateString()}</span>
                                                <span className="text-xs text-slate-400 flex items-center gap-1">
                                                    <Clock className="w-3 h-3" />
                                                    {new Date(device.lastSyncTime).toLocaleTimeString()}
                                                </span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold ${getStatusColor(device.lastSyncTime)}`}>
                                                <Activity className="w-3 h-3" />
                                                {getStatusText(device.lastSyncTime)}
                                            </div>
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
