import api from './axiosInstance';
import { ApiResponse } from '@/types/api.types';

export interface AnalyticsSummaryDTO {
    totalCalls: number;
    totalDuration: number;
    uniqueContacts: number;
    answerRate: number;
}

export interface TopContactDTO {
    phoneNumber: string;
    contactName: string;
    totalCalls: number;
    totalDuration: number;
    rank: number;
}

export interface DailyTrendDTO {
    date: string;
    incomingCalls: number;
    outgoingCalls: number;
    missedCalls: number;
    rejectedCalls: number;
}

export interface CallLogSyncDTO {
    phoneNumber: string;
    contactName: string;
    callType: string;
    startTime: string;
    endTime?: string;
    durationInSeconds: number;
    simId: string;
}

export interface PagedResponse<T> {
    totalCount: number;
    page: number;
    pageSize: number;
    items: T[];
}

export const callAnalyticsService = {
    getSummary: (from?: string, to?: string) => 
        api.get<ApiResponse<AnalyticsSummaryDTO>>(`/CallAnalytics/summary`, { params: { from, to } }),

    getTopContacts: (limit: number = 10, from?: string, to?: string) => 
        api.get<ApiResponse<TopContactDTO[]>>(`/CallAnalytics/top-contacts`, { params: { limit, from, to } }),

    getDailyTrends: (from: string, to: string) => 
        api.get<ApiResponse<DailyTrendDTO[]>>(`/CallAnalytics/daily-trends`, { params: { from, to } }),

    getLogs: (page: number = 1, pageSize: number = 50, search?: string) => 
        api.get<ApiResponse<PagedResponse<CallLogSyncDTO>>>(`/CallAnalytics/logs`, { params: { page, pageSize, search } }),
};
