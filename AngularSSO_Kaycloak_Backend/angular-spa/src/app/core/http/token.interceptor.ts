import { HttpInterceptorFn } from '@angular/common/http';
import { from, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { keycloak } from '../auth/keycloak.config';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // ✅ skip static assets + config json
  if (req.url.startsWith('/assets/')) {
    return next(req);
  }

  // ✅ keycloak chưa init xong thì cho request đi bình thường (đừng crash app)
  if (!keycloak) {
    return next(req);
  }

  // ✅ chưa authenticated thì cho đi
  if (!keycloak.authenticated) {
    return next(req);
  }

  // ✅ refresh token an toàn
  return from(keycloak.updateToken(30)).pipe(
    catchError(err => {
      console.warn('updateToken failed:', err);
      return of(false);
    }),
    switchMap(() => {
      const token = keycloak.token;
      if (!token) return next(req);

      return next(
        req.clone({
          setHeaders: { Authorization: `Bearer ${token}` },
        })
      );
    })
  );
};
