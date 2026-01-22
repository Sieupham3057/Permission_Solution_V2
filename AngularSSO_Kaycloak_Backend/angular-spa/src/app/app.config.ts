import { ApplicationConfig, provideAppInitializer, inject } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/http/token.interceptor';
import { gatewayUrlInterceptor } from './core/http/gateway-url.interceptor';
import { AppConfigService } from './core/config/app-config.service';
import { initializeKeycloak } from './core/auth/keycloak.initializer';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(withInterceptors([
      gatewayUrlInterceptor,  // ✅ rewrite trước
      authInterceptor         // ✅ attach token sau
    ])),

    provideAppInitializer(async () => {
      const cfgSvc = inject(AppConfigService);
      await cfgSvc.load();                 // ✅ injection context OK ở đây
      await initializeKeycloak(cfgSvc.config); // ✅ truyền config vào
    }),

    provideRouter(routes),
  ],
};
