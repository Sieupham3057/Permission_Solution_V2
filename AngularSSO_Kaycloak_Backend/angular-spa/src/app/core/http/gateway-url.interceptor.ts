import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AppConfigService } from '../config/app-config.service';

export const gatewayUrlInterceptor: HttpInterceptorFn = (req, next) => {
  // skip assets
  if (req.url.startsWith('/assets/')) return next(req);

  // chỉ xử lý URL relative
  if (!req.url.startsWith('/')) return next(req);

  const cfg = inject(AppConfigService);

  const services = cfg.config.services ?? {};
  const prefixes = Object.values(services)
    .filter(v => typeof v === 'string' && v.startsWith('/'))
    // ưu tiên prefix dài trước để tránh case /product match trước /product-service (nếu có)
    .sort((a, b) => b.length - a.length);

  const matchedPrefix = prefixes.find(p => req.url === p || req.url.startsWith(p + '/'));
  if (!matchedPrefix) return next(req);

  const base = cfg.gatewayBaseUrl.replace(/\/+$/, '');
  return next(req.clone({ url: `${base}${req.url}` }));
};
