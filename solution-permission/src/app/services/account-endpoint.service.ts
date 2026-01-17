import { inject, Injectable } from "@angular/core";
import { BaseApiService } from "./base-api.service";
import { catchError, Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class AccountEndpoint extends BaseApiService {
    private usersUrl = 'UserAccount/users';
    private userByUserNameUrl = 'UserAccount/users/username';
    private currentUserUrl = 'UserAccount/users/me';
    private currentUserPreferencesUrl = 'UserAccount/users/me/preferences';
    private unblockUserUrl = 'UserAccount/users/unblock';
    private rolesUrl = 'UserRole/roles';
    private roleByRoleNameUrl = 'UserRole/roles/name';
    private permissionsUrl = 'UserRole/permissions';

    private baseApi = inject(BaseApiService);

    getUserEndpoint<T>(userId?: string): Observable<T> {
        const endpointUrl = userId ? `${this.usersUrl}/${userId}` : this.currentUserUrl;

        return this.baseApi.get<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getUserByUserNameEndpoint<T>(userName: string): Observable<T> {
        const endpointUrl = `${this.userByUserNameUrl}/${userName}`;

        return this.baseApi.get<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getUsersEndpoint<T>(page?: number, pageSize?: number): Observable<T> {
        const endpointUrl = page && pageSize ? `${this.usersUrl}/${page}/${pageSize}` : this.usersUrl;

        return this.baseApi.get<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }
    getNewUserEndpoint<T>(user: object): Observable<T> {
        return this.baseApi.post<T>(this.usersUrl, JSON.stringify(user)).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getUpdateUserEndpoint<T>(user: object, userId?: string): Observable<T> {
        const endpointUrl = userId ? `${this.usersUrl}/${userId}` : this.currentUserUrl;

        return this.baseApi.put<T>(endpointUrl, JSON.stringify(user)).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getPatchUpdateUserEndpoint<T>(patch: object | object[], userId?: string): Observable<T>;
    getPatchUpdateUserEndpoint<T>(value: unknown, op: string, path: string, from?: string, userId?: string): Observable<T>;
    getPatchUpdateUserEndpoint<T>(valueOrPatch: unknown, opOrUserId?: string, path?: string, from?: string, userId?: string): Observable<T> {
        let endpointUrl: string;
        let patchDocument: unknown;

        if (path) {
            endpointUrl = userId ? `${this.usersUrl}/${userId}` : this.currentUserUrl;
            patchDocument = from ? [{ op: opOrUserId, from: from, path: path }] : [{ op: opOrUserId, path: path, value: valueOrPatch }];
        } else {
            endpointUrl = opOrUserId ? `${this.usersUrl}/${opOrUserId}` : this.currentUserUrl;
            patchDocument = valueOrPatch;
        }

        return this.baseApi.patch<T>(endpointUrl, patchDocument).pipe(
            catchError(error => {
                return this.handleError(error);
            })
        );
    }

    getUserPreferencesEndpoint<T>(): Observable<T> {
        return this.baseApi.get<T>(this.currentUserPreferencesUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getUpdateUserPreferencesEndpoint<T>(configuration: string | null): Observable<T> {
        return this.baseApi.put<T>(this.currentUserPreferencesUrl, JSON.stringify(configuration)).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getUnblockUserEndpoint<T>(userId: string): Observable<T> {
        const endpointUrl = `${this.unblockUserUrl}/${userId}`;

        return this.baseApi.put<T>(endpointUrl, null).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getDeleteUserEndpoint<T>(userId: string): Observable<T> {
        const endpointUrl = `${this.usersUrl}/${userId}`;

        return this.baseApi.delete<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }


    getRoleEndpoint<T>(roleId: string): Observable<T> {
        const endpointUrl = `${this.rolesUrl}/${roleId}`;

        return this.baseApi.get<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getRoleByRoleNameEndpoint<T>(roleName: string): Observable<T> {
        const endpointUrl = `${this.roleByRoleNameUrl}/${roleName}`;

        return this.baseApi.get<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getRolesEndpoint<T>(page?: number, pageSize?: number): Observable<T> {
        const endpointUrl = page && pageSize ? `${this.rolesUrl}/${page}/${pageSize}` : this.rolesUrl;

        return this.baseApi.get<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getNewRoleEndpoint<T>(role: object): Observable<T> {
        return this.baseApi.post<T>(this.rolesUrl, JSON.stringify(role)).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getUpdateRoleEndpoint<T>(role: object, roleId: string): Observable<T> {
        const endpointUrl = `${this.rolesUrl}/${roleId}`;

        return this.baseApi.put<T>(endpointUrl, JSON.stringify(role)).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getDeleteRoleEndpoint<T>(roleId: string): Observable<T> {
        const endpointUrl = `${this.rolesUrl}/${roleId}`;

        return this.baseApi.delete<T>(endpointUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

    getPermissionsEndpoint<T>(): Observable<T> {
        return this.baseApi.get<T>(this.permissionsUrl).pipe(
            catchError(error => {
                return this.handleError(error);
            }));
    }

}