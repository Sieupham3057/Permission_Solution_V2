import { inject, Injectable } from "@angular/core";
import { BehaviorSubject, Observable, tap } from "rxjs";
import { BaseApiService } from "./base-api.service";
import { UserLogin } from "../models/user-login.model";
import { LocalStoreManager } from "./local-store-manager.service";
import { Router } from "@angular/router";
import { AccessToken, LoginResponse } from "../models/login-response.model";
import { JwtHelper } from "./jwt-helper";
import { PermissionValues } from "../models/permission.model";
import { User } from "../models/user.model";

@Injectable({
    providedIn: 'root',
})
export class AuthService {

    private baseApiService = inject(BaseApiService);
    private localStorage = inject(LocalStoreManager);
    private router = inject(Router);

    private endpoint = 'Authorization';
    private authStatus = new BehaviorSubject<boolean>(this.hasToken()); // Track auth status

    login(userLogin: UserLogin): Observable<any> {
        return this.baseApiService.post<LoginResponse>(`${this.endpoint}/login`, userLogin).pipe(
            tap((response) => {
                this.localStorage.removeStorage();
                const accessToken = response.accessToken;
                this.localStorage.setAccessToken(accessToken);
                this.authStatus.next(true);
            })
        );
    }

    refreshLogin(): Observable<any> {
        return this.baseApiService.post<LoginResponse>(`${this.endpoint}/refreshLogin`, null).pipe(
            tap((response) => {
                this.localStorage.removeStorage();
                const accessToken = response.accessToken;
                this.localStorage.setAccessToken(accessToken);
                this.authStatus.next(true);
            })
        );
    }

    logout() {
        this.localStorage.removeStorage();
        this.authStatus.next(false); // Update auth status
        this.router.navigate(['/login']); // Redirect to login page
    }

    get currentUser(): User | null {
        const user = this.localStorage.getCurrentUser();
        return user;
    }

    get userPermissions(): PermissionValues[] {
        const accessToken = this.accessToken;
        if ((accessToken === null || accessToken === ''))
            throw new Error('AccessToken is empty.');

        const jwtHelper = new JwtHelper();
        const decodedIdToken = jwtHelper.decodeJWT(accessToken).payload as AccessToken;

        const permissions: PermissionValues[] = Array.isArray(decodedIdToken.permission) ?
            decodedIdToken.permission : [decodedIdToken.permission];

        return permissions ?? [];
    }

    private hasToken(): boolean {
        return !!this.localStorage.getAccessToken();
    }

    get accessToken(): string | null {
        return this.localStorage.getAccessToken();
    }
    get isLoggedIn(): boolean {
        return this.currentUser != null;
    }
}