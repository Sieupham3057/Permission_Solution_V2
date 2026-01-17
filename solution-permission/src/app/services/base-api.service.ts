import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, Observable, throwError } from "rxjs";
import { environment } from "../../environments/environment";
import { PagedResponse } from "../models/pagination.model";
import { LocalStoreManager } from "./local-store-manager.service";
import { Router } from "@angular/router";

// Standalone Injectable Service with HttpClient injection
@Injectable({
    providedIn: 'root',  // Make sure it's globally available
})
export class BaseApiService {

    private baseUrl = environment.apiUrl;
    private http = inject(HttpClient); // Inject HttpClient without HttpClientModule
    private localStorage = inject(LocalStoreManager);
    private router = inject(Router);

    protected get requestHeaders(): { headers: HttpHeaders | Record<string, string | string[]> } {
        const headers = new HttpHeaders({
            Authorization: `Bearer ${this.localStorage.getAccessToken()}`,
            'Content-Type': 'application/json',
            Accept: 'application/json, text/plain, */*',
        });

        return { headers };
    }

    get<T>(endpoint: string): Observable<T> {
        return this.http.get<T>(`${this.baseUrl}/${endpoint}`, { headers: this.requestHeaders.headers })
            .pipe(catchError(this.handleError));
    }

    getPaged<T>(endpoint: string, params?: any): Observable<PagedResponse<T>> {
        return this.http.get<PagedResponse<T>>(`${this.baseUrl}/${endpoint}`, { headers: this.requestHeaders.headers, params });
    }

    post<T>(endpoint: string, data: any | null): Observable<T> {
        console.log(`======== ${this.baseUrl}/${endpoint}`);
        return this.http.post<T>(`${this.baseUrl}/${endpoint}`, data, { headers: this.requestHeaders.headers })
            .pipe(catchError(this.handleError));
    }

    put<T>(endpoint: string, data: any): Observable<T> {
        return this.http.put<T>(`${this.baseUrl}/${endpoint}`, data, { headers: this.requestHeaders.headers })
            .pipe(catchError(this.handleError));
    }

    patch<T>(endpoint: string, data: any): Observable<T> {
        return this.http.patch<T>(`${this.baseUrl}/${endpoint}`, data, { headers: this.requestHeaders.headers })
            .pipe(catchError(this.handleError));
    }

    delete<T>(endpoint: string): Observable<T> {
        return this.http.delete<T>(`${this.baseUrl}/${endpoint}`, { headers: this.requestHeaders.headers })
            .pipe(catchError(this.handleError));
    }

    protected handleError(error: HttpErrorResponse): Observable<never> {
        if (error.status === 401) {
            console.error('Unauthorized request. Redirecting to login...');
            this.router.navigate(['/login']);
        } else if (error.status === 500) {
            console.error('Internal server error:', error);
        } else {
            console.error('API Error:', error);
        }

        return throwError(() => new Error(error.message || 'An unknown error occurred.'));
    }
}
