import { Injectable } from "@angular/core";
import { User } from "../models/user.model";

@Injectable({
    providedIn: 'root'
})

export class LocalStoreManager {
    public static readonly ACCESS_TOKEN = 'access_token';
    public static readonly USER_PERMISSIONS = 'user_permissions';
    public static readonly CURRENT_USER = 'current_user';

    public setAccessToken(accessToken: string): void {
        if (typeof accessToken !== 'string') {
            throw new Error('Access token must be a string');
        }
        localStorage.setItem(LocalStoreManager.ACCESS_TOKEN, accessToken);
    }

    public getAccessToken(): string {
        const token = localStorage.getItem(LocalStoreManager.ACCESS_TOKEN);
        if (!token) {
            return '';  // or throw an error depending on your needs
        }
        return token;
    }

    public setCurrentUser(user: User) {
        if (user)
            throw new Error('Current user not existed.');
        localStorage.setItem(LocalStoreManager.CURRENT_USER, JSON.stringify(user));
    }

    public getCurrentUser(): User {
        var currentUser = localStorage.getItem(LocalStoreManager.CURRENT_USER);
        if (!currentUser)
            return new User;
        return JSON.parse(currentUser);
    }

    public removeStorage() {
        localStorage.clear();
    }
}
