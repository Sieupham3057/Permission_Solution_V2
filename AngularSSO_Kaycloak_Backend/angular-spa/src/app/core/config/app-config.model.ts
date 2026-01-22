export interface AppConfig {
  gatewayBaseUrl: string;
  services: Record<string, string>; // customer-service/product-service/order-service...
  keycloak: {
    url: string;
    realm: string;
    clientId: string;
  };
}
