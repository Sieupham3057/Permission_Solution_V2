// import { Inject, Injectable } from '@angular/core';
// import { HttpInterceptor, HttpRequest, HttpHandler, HttpErrorResponse, HttpEvent } from '@angular/common/http';
// import { BehaviorSubject, catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
// import { Router } from '@angular/router';
// import { LocalStoreManager } from '../services/local-store-manager.service';
// import { AuthService } from '../services/auth.service';

// @Injectable()
// export class AuthInterceptor implements HttpInterceptor {
//     private isRefreshing = false;
//     private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

//     private localStorage = Inject(LocalStoreManager);

//     constructor(private router: Router, private authService: AuthService) { }

//     intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//         const accessToken = this.localStorage.getAccessToken();
//         let request = req;

//         // Add Authorization header if access token exists
//         if (accessToken) {
//             request = this.addToken(req, accessToken);
//         }

//         return next.handle(request).pipe(
//             catchError((error: HttpErrorResponse) => {
//                 if (error.status === 401 && !request.url.includes('/users/refresh-token')) {
//                     return this.handle401Error(request, next);
//                 } else if (error.status === 403) {
//                     this.router.navigate(['access-denied']);
//                 } else if (error.status === 500) {
//                     this.router.navigate(['error']);
//                 }
//                 return throwError(() => error);
//             })
//         );
//     }

//     private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//         if (!this.isRefreshing) {
//             this.isRefreshing = true;
//             this.refreshTokenSubject.next(null);

//             return this.authService.refreshToken().pipe(
//                 switchMap((token: any) => {
//                     this.isRefreshing = false;
//                     this.refreshTokenSubject.next(token.accessToken);

//                     // Remove old tokens
//                     this.localStorage.removeStorage();

//                     // Store new tokens in localStorage
//                     this.localStorage.setAccessToken(token);

//                     return next.handle(this.addToken(request, token.accessToken));
//                 }),
//                 catchError((err) => {
//                     this.isRefreshing = false;
//                     this.authService.logout(); // Handle logout if refresh fails
//                     return throwError(() => err);
//                 })
//             );
//         } else {
//             return this.refreshTokenSubject.pipe(
//                 filter(token => token !== null),
//                 take(1),
//                 switchMap(token => next.handle(this.addToken(request, token!)))
//             );
//         }
//     }

//     private addToken(request: HttpRequest<any>, token: string) {
//         return request.clone({
//             setHeaders: { Authorization: `Bearer ${token}` }
//         });
//     }
// }
