// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

import { PermissionValues } from './permission.model';


export interface LoginResponse {
    accessToken: string;
    refreshToken: string;
}


export interface AccessToken {
    nameidentifier: string;
    emailaddress: string;
    name: string;
    role: string | string[];
    permission: PermissionValues | PermissionValues[];
    exp: number;
    iss: string;
    aud: string | string[];
}
