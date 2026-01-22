import { Injectable } from '@angular/core';
import { keycloak } from './keycloak.config';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() {}

  logout(): void {
    keycloak.logout({
      redirectUri: window.location.origin
    });
  }
}
