import api from './axiosInstance';
import { ApiResponse } from '@/types/api.types';

export interface DeviceInformationDTO {
    deviceId: string;
    employeeName: string;
    manufacturer: string;
    model: string;
    osVersion: string;
    appVersion: string;
    platform: string;
    batteryPercentage: number;
    timeZone: string;
    lastSyncTime: string;
}

export const deviceManagementService = {
    getDevices: () => 
        api.get<ApiResponse<DeviceInformationDTO[]>>(`/DeviceManagement/List`),
};
