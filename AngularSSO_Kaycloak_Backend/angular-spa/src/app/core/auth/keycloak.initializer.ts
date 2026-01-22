import { AppConfig } from '../config/app-config.model';
import { initKeycloakInstance, keycloak } from './keycloak.config';

export async function initializeKeycloak(cfg: AppConfig): Promise<void> {
  initKeycloakInstance(cfg);

  await keycloak.init({
    onLoad: 'login-required',
    pkceMethod: 'S256',
    checkLoginIframe: false,
  });
}
