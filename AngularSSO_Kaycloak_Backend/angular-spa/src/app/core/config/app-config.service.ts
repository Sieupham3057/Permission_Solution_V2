import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { AppConfig } from './app-config.model';

@Injectable({ providedIn: 'root' })
export class AppConfigService {
  private _config: AppConfig | null = null;

  constructor(private http: HttpClient) {}

  async load(): Promise<void> {
    const cfg = await firstValueFrom(
      this.http.get<AppConfig>('/assets/config/app-config.json')
    );

    // ===== Validate tối thiểu cho production =====
    if (!cfg?.gatewayBaseUrl) {
      throw new Error('Missing gatewayBaseUrl in /assets/config/app-config.json');
    }
    if (!cfg?.services || Object.keys(cfg.services).length === 0) {
      throw new Error('Missing services map in /assets/config/app-config.json');
    }
    for (const [k, v] of Object.entries(cfg.services)) {
      if (!v || typeof v !== 'string') {
        throw new Error(`Invalid services["${k}"] in config`);
      }
      if (!v.startsWith('/')) {
        throw new Error(`services["${k}"] must start with "/" (got: ${v})`);
      }
    }
    if (!cfg?.keycloak?.url || !cfg?.keycloak?.realm || !cfg?.keycloak?.clientId) {
      throw new Error('Missing keycloak config in /assets/config/app-config.json');
    }

    this._config = cfg;
  }

  get config(): AppConfig {
    if (!this._config) {
      throw new Error(
        'AppConfig not loaded yet. Ensure provideAppInitializer() calls AppConfigService.load().'
      );
    }
    return this._config;
  }

  /** Base gateway URL */
  get gatewayBaseUrl(): string {
    return this.config.gatewayBaseUrl;
  }

  /** Lấy service prefix theo key: customer/product/... */
  servicePrefix(serviceKey: string): string {
    const prefix = this.config.services?.[serviceKey];
    if (!prefix) {
      throw new Error(`Service key "${serviceKey}" not found in config.services`);
    }
    return prefix;
  }

  /**
   * Build URL chuẩn:
   * gw + servicePrefix + path
   * ví dụ: apiUrl('customer','/api/customers')
   */
  apiUrl(serviceKey: string, path: string): string {
    const base = this.gatewayBaseUrl.replace(/\/+$/, '');
    const prefix = this.servicePrefix(serviceKey).replace(/\/+$/, '');
    const p = (path ?? '').startsWith('/') ? path : `/${path}`;
    return `${base}${prefix}${p}`;
  }
}
