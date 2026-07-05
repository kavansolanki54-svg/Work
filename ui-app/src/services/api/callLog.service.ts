import api from './axiosInstance';
import { ApiResponse } from '@/types/api.types';

export interface PhoneCallLog {
    callLogId: number;
    employeeId: number;
    employeeName: string;
    phoneNumber: string;
    contactName?: string;
    callType: string;
    startTime: string;
    endTime?: string;
    durationInSeconds: number;
    simId?: string;
    createDate: string;
}

export const callLogService = {
    getLogs: (employeeId?: number) => 
        api.get<ApiResponse<PhoneCallLog[]>>(`/CallLogs/List${employeeId ? `/${employeeId}` : ''}`),
};
