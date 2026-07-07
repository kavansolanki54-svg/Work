"use client";

import { useQuery, useMutation } from "@tanstack/react-query";
import { Activity, RefreshCw, AlertCircle, CheckCircle2, ServerCrash, Clock } from "lucide-react";
import api from '@/services/api/axiosInstance';
import { toast } from "sonner";

interface SyncHealth {
    totalRegisteredDevices: number;
    activeDevices24H: number;
    healthPercentage: number;
    lastChecked: string;
}

export default function SyncMonitorPage() {
    const { data: health, isLoading, refetch, isRefetching } = useQuery({
        queryKey: ["syncHealth"],
        queryFn: async () => {
            const res = await api.get('/SyncManagement/Health');
            return res.data?.data as SyncHealth;
        }
    });

    const forceSyncMutation = useMutation({
        mutationFn: async (deviceId: string) => {
            await api.post(`/SyncManagement/ForceSync/${deviceId}`);
        },
        onSuccess: () => {
            toast.success("Force sync command issued successfully!");
        },
        onError: () => {
            toast.error("Failed to issue force sync command.");
        }
    });

    return (
        <div className="flex flex-col gap-6 font-sans pb-10 animate-in fade-in duration-500">
            {/* Header */}
            <div className="w-full bg-white/90 backdrop-blur-md border border-slate-200 shadow-sm rounded-2xl p-6">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center text-white">
                            <Activity className="w-5 h-5" />
                        </div>
                        <div>
                            <h1 className="text-xl font-bold text-slate-800">Sync Monitor</h1>
                            <p className="text-sm text-slate-500">Real-time background sync health across the organization</p>
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

            {/* Health Overview */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col gap-2">
                    <span className="text-slate-500 font-semibold text-sm">System Health</span>
                    {isLoading ? (
                        <div className="h-10 w-24 bg-slate-100 animate-pulse rounded-lg"></div>
                    ) : (
                        <div className="flex items-center gap-3">
                            <span className="text-4xl font-bold text-slate-800 tracking-tight">
                                {health?.healthPercentage?.toFixed(0) || 0}%
                            </span>
                            {health?.healthPercentage && health.healthPercentage > 80 ? (
                                <div className="bg-emerald-100 text-emerald-700 p-1.5 rounded-full"><CheckCircle2 className="w-5 h-5" /></div>
                            ) : health?.healthPercentage && health.healthPercentage > 50 ? (
                                <div className="bg-orange-100 text-orange-700 p-1.5 rounded-full"><AlertCircle className="w-5 h-5" /></div>
                            ) : (
                                <div className="bg-red-100 text-red-700 p-1.5 rounded-full"><ServerCrash className="w-5 h-5" /></div>
                            )}
                        </div>
                    )}
                </div>

                <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col gap-2">
                    <span className="text-slate-500 font-semibold text-sm">Active Devices (24h)</span>
                    {isLoading ? (
                        <div className="h-10 w-24 bg-slate-100 animate-pulse rounded-lg"></div>
                    ) : (
                        <span className="text-4xl font-bold text-slate-800 tracking-tight">
                            {health?.activeDevices24H || 0} <span className="text-xl text-slate-400 font-medium">/ {health?.totalRegisteredDevices || 0}</span>
                        </span>
                    )}
                </div>

                <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col gap-2">
                    <span className="text-slate-500 font-semibold text-sm">Last Checked</span>
                    {isLoading ? (
                        <div className="h-10 w-24 bg-slate-100 animate-pulse rounded-lg"></div>
                    ) : (
                        <div className="flex items-center gap-2 text-slate-700 h-full">
                            <Clock className="w-5 h-5 text-slate-400" />
                            <span className="font-medium text-lg">
                                {health?.lastChecked ? new Date(health.lastChecked).toLocaleTimeString() : 'N/A'}
                            </span>
                        </div>
                    )}
                </div>
            </div>

            {/* Test Force Sync */}
            <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col items-center justify-center min-h-[250px] text-center">
                <div className="w-16 h-16 bg-slate-50 rounded-full flex items-center justify-center mb-4 border border-slate-100 shadow-sm">
                    <RefreshCw className="w-8 h-8 text-indigo-500" />
                </div>
                <h3 className="text-lg font-bold text-slate-800 mb-2">Manual Intervention</h3>
                <p className="text-slate-500 max-w-md mb-6 text-sm">
                    If a device is experiencing severe sync delays, you can issue a Force Sync command. This will prompt the device to immediately flush its offline analytics cache to the cloud on its next network ping.
                </p>
                <div className="flex items-center gap-3">
                    <input 
                        type="text" 
                        id="forceDeviceId"
                        placeholder="Enter Device ID..." 
                        className="border border-slate-200 rounded-lg px-4 py-2 w-64 focus:ring-2 focus:ring-indigo-100 focus:border-indigo-400 outline-none transition-all text-sm"
                    />
                    <button 
                        onClick={() => {
                            const input = document.getElementById('forceDeviceId') as HTMLInputElement;
                            if (input && input.value) {
                                forceSyncMutation.mutate(input.value);
                            } else {
                                toast.error("Please enter a valid Device ID.");
                            }
                        }}
                        disabled={forceSyncMutation.isPending}
                        className="bg-indigo-600 text-white px-5 py-2 rounded-lg font-bold hover:bg-indigo-700 transition-colors shadow-sm disabled:opacity-50"
                    >
                        {forceSyncMutation.isPending ? 'Issuing...' : 'Force Sync'}
                    </button>
                </div>
            </div>
        </div>
    );
}
