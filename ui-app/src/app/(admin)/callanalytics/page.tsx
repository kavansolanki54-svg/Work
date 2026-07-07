"use client";

import { useQuery } from "@tanstack/react-query";
import { PhoneCall, Clock, Users, Percent, PhoneIncoming, PhoneOutgoing, PhoneMissed } from "lucide-react";
import { callAnalyticsService } from "@/services/api/callAnalytics.service";
import { useState } from "react";
import { PieChart, Pie, Cell, Tooltip, ResponsiveContainer, BarChart, Bar, XAxis, YAxis, CartesianGrid, Legend } from "recharts";

export default function CallAnalyticsPage() {
    const [dateRange, setDateRange] = useState({ from: "", to: "" });

    // Fetch Summary
    const { data: summary, isLoading: isLoadingSummary } = useQuery({
        queryKey: ["callAnalyticsSummary", dateRange],
        queryFn: () => callAnalyticsService.getSummary(dateRange.from || undefined, dateRange.to || undefined),
        select: (res) => res.data?.data
    });

    // Fetch Distribution (Using Summary for demo since distribution API wasn't explicitly modeled in backend yet, wait, we do have dailyTrends!)
    // Actually, I'll use the dailyTrends endpoint to generate the distribution and trends.
    const { data: dailyTrends = [], isLoading: isLoadingTrends } = useQuery({
        queryKey: ["callAnalyticsTrends", dateRange],
        queryFn: () => callAnalyticsService.getDailyTrends(
            dateRange.from || new Date(new Date().setDate(new Date().getDate() - 30)).toISOString(), 
            dateRange.to || new Date().toISOString()
        ),
        select: (res) => res.data?.data || []
    });

    // Fetch Top Contacts
    const { data: topContacts = [], isLoading: isLoadingTopContacts } = useQuery({
        queryKey: ["callAnalyticsTopContacts", dateRange],
        queryFn: () => callAnalyticsService.getTopContacts(10, dateRange.from || undefined, dateRange.to || undefined),
        select: (res) => res.data?.data || []
    });

    const formatDuration = (seconds: number) => {
        if (!seconds) return "0s";
        const h = Math.floor(seconds / 3600);
        const m = Math.floor((seconds % 3600) / 60);
        const s = seconds % 60;
        
        if (h > 0) return `${h}h ${m}m`;
        if (m > 0) return `${m}m ${s}s`;
        return `${s}s`;
    };

    // Calculate aggregated totals for the Pie Chart
    const totalIncoming = dailyTrends.reduce((acc, curr) => acc + curr.incomingCalls, 0);
    const totalOutgoing = dailyTrends.reduce((acc, curr) => acc + curr.outgoingCalls, 0);
    const totalMissed = dailyTrends.reduce((acc, curr) => acc + curr.missedCalls, 0);
    const totalRejected = dailyTrends.reduce((acc, curr) => acc + curr.rejectedCalls, 0);

    const pieData = [
        { name: "Incoming", value: totalIncoming, color: "#22c55e" },
        { name: "Outgoing", value: totalOutgoing, color: "#3b82f6" },
        { name: "Missed", value: totalMissed, color: "#ef4444" },
        { name: "Rejected", value: totalRejected, color: "#f97316" },
    ].filter(x => x.value > 0);

    return (
        <div className="flex flex-col gap-6 font-sans pb-10 animate-in fade-in duration-500">
            {/* Header */}
            <div className="w-full bg-white/90 backdrop-blur-md border border-slate-200 shadow-sm rounded-2xl p-6">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center text-white">
                            <PhoneCall className="w-5 h-5" />
                        </div>
                        <div>
                            <h1 className="text-xl font-bold text-slate-800">Call Analytics</h1>
                            <p className="text-sm text-slate-500">Overview of organizational calling metrics</p>
                        </div>
                    </div>
                </div>
            </div>

            {/* Stat Cards */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <StatCard 
                    title="Total Calls" 
                    value={summary?.totalCalls?.toLocaleString() || "0"} 
                    icon={<PhoneCall className="w-5 h-5 text-indigo-500" />} 
                    isLoading={isLoadingSummary} 
                />
                <StatCard 
                    title="Total Duration" 
                    value={formatDuration(summary?.totalDuration || 0)} 
                    icon={<Clock className="w-5 h-5 text-emerald-500" />} 
                    isLoading={isLoadingSummary} 
                />
                <StatCard 
                    title="Unique Contacts" 
                    value={summary?.uniqueContacts?.toLocaleString() || "0"} 
                    icon={<Users className="w-5 h-5 text-blue-500" />} 
                    isLoading={isLoadingSummary} 
                />
                <StatCard 
                    title="Answer Rate" 
                    value={`${summary?.answerRate || 0}%`} 
                    icon={<Percent className="w-5 h-5 text-orange-500" />} 
                    isLoading={isLoadingSummary} 
                />
            </div>

            {/* Charts Section */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Distribution Pie Chart */}
                <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm flex flex-col items-center justify-center min-h-[350px]">
                    <h2 className="text-lg font-bold text-slate-800 self-start w-full mb-4">Call Distribution</h2>
                    {pieData.length > 0 ? (
                        <div className="w-full h-[250px]">
                            <ResponsiveContainer width="100%" height="100%">
                                <PieChart>
                                    <Pie
                                        data={pieData}
                                        cx="50%"
                                        cy="50%"
                                        innerRadius={60}
                                        outerRadius={80}
                                        paddingAngle={5}
                                        dataKey="value"
                                    >
                                        {pieData.map((entry, index) => (
                                            <Cell key={`cell-${index}`} fill={entry.color} />
                                        ))}
                                    </Pie>
                                    <Tooltip contentStyle={{ borderRadius: '12px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }} />
                                    <Legend verticalAlign="bottom" height={36}/>
                                </PieChart>
                            </ResponsiveContainer>
                        </div>
                    ) : (
                        <div className="text-slate-400 text-sm flex items-center justify-center h-full">No distribution data available</div>
                    )}
                </div>

                {/* Daily Trends Bar Chart */}
                <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-sm lg:col-span-2 min-h-[350px]">
                    <h2 className="text-lg font-bold text-slate-800 mb-4">Daily Call Trends</h2>
                    {dailyTrends.length > 0 ? (
                        <div className="w-full h-[250px]">
                            <ResponsiveContainer width="100%" height="100%">
                                <BarChart data={dailyTrends.map(x => ({ ...x, date: new Date(x.date).toLocaleDateString(undefined, { month: 'short', day: 'numeric' }) }))} margin={{ top: 10, right: 10, left: -20, bottom: 0 }}>
                                    <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#f1f5f9" />
                                    <XAxis dataKey="date" axisLine={false} tickLine={false} tick={{ fontSize: 12, fill: '#64748b' }} dy={10} />
                                    <YAxis axisLine={false} tickLine={false} tick={{ fontSize: 12, fill: '#64748b' }} />
                                    <Tooltip 
                                        cursor={{ fill: '#f8fafc' }}
                                        contentStyle={{ borderRadius: '12px', border: 'none', boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)' }}
                                    />
                                    <Legend verticalAlign="top" height={36} iconType="circle"/>
                                    <Bar dataKey="incomingCalls" name="Incoming" stackId="a" fill="#22c55e" radius={[0, 0, 4, 4]} />
                                    <Bar dataKey="outgoingCalls" name="Outgoing" stackId="a" fill="#3b82f6" />
                                    <Bar dataKey="missedCalls" name="Missed" stackId="a" fill="#ef4444" radius={[4, 4, 0, 0]} />
                                </BarChart>
                            </ResponsiveContainer>
                        </div>
                    ) : (
                        <div className="text-slate-400 text-sm flex items-center justify-center h-full">No trend data available</div>
                    )}
                </div>
            </div>

            {/* Top Contacts Table */}
            <div className="w-full bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden mt-2">
                <div className="px-6 py-5 border-b border-slate-100">
                    <h2 className="text-lg font-bold text-slate-800">Top Contacts</h2>
                </div>
                <div className="overflow-x-auto">
                    <table className="w-full text-left border-collapse">
                        <thead>
                            <tr className="bg-slate-50 border-b border-slate-200 text-[11px] uppercase tracking-widest text-slate-500 font-bold">
                                <th className="px-6 py-4">Rank</th>
                                <th className="px-6 py-4">Contact</th>
                                <th className="px-6 py-4">Total Calls</th>
                                <th className="px-6 py-4">Total Duration</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100 text-sm text-slate-700">
                            {isLoadingTopContacts ? (
                                <tr>
                                    <td colSpan={4} className="px-6 py-8 text-center text-slate-400">Loading top contacts...</td>
                                </tr>
                            ) : topContacts.length === 0 ? (
                                <tr>
                                    <td colSpan={4} className="px-6 py-8 text-center text-slate-400">No top contacts found.</td>
                                </tr>
                            ) : (
                                topContacts.map((contact) => (
                                    <tr key={contact.phoneNumber} className="hover:bg-slate-50/50 transition-colors">
                                        <td className="px-6 py-4 font-bold text-slate-500">#{contact.rank}</td>
                                        <td className="px-6 py-4">
                                            <div className="flex flex-col">
                                                <span className="font-bold text-slate-800">{contact.contactName || "Unknown"}</span>
                                                <span className="text-xs text-slate-500">{contact.phoneNumber}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 font-bold text-indigo-600">{contact.totalCalls}</td>
                                        <td className="px-6 py-4 text-slate-600 font-mono text-xs">{formatDuration(contact.totalDuration)}</td>
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

function StatCard({ title, value, icon, isLoading }: { title: string, value: string, icon: React.ReactNode, isLoading: boolean }) {
    return (
        <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm flex flex-col gap-4">
            <div className="flex items-center justify-between">
                <span className="text-sm font-semibold text-slate-500">{title}</span>
                <div className="p-2 bg-slate-50 rounded-lg">{icon}</div>
            </div>
            {isLoading ? (
                <div className="h-8 w-24 bg-slate-100 animate-pulse rounded-md mt-1"></div>
            ) : (
                <span className="text-3xl font-bold text-slate-800 tracking-tight">{value}</span>
            )}
        </div>
    );
}
