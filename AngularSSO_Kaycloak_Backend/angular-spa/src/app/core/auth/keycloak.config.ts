import Keycloak from 'keycloak-js';
import type { AppConfig } from '../config/app-config.model';

export let keycloak: Keycloak;

export function initKeycloakInstance(cfg: AppConfig): void {
  keycloak = new Keycloak({
    url: cfg.keycloak.url,
    realm: cfg.keycloak.realm,
    clientId: cfg.keycloak.clientId,
  });
}
