import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly REMEMBER_ME_KEY = 'remember_me';

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  setRefreshToken(refreshToken: string): void {
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  removeRefreshToken(): void {
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
  }

  getRememberMe(): boolean {
    const value = localStorage.getItem(this.REMEMBER_ME_KEY);
    return value === 'true';
  }

  setRememberMe(remember: boolean): void {
    localStorage.setItem(this.REMEMBER_ME_KEY, remember.toString());
  }

  removeRememberMe(): void {
    localStorage.removeItem(this.REMEMBER_ME_KEY);
  }

  clearAll(): void {
    this.removeToken();
    this.removeRefreshToken();
    this.removeRememberMe();
  }

  hasValidSession(): boolean {
    const token = this.getToken();
    const refreshToken = this.getRefreshToken();
    const rememberMe = this.getRememberMe();

    // Si rememberMe est activé, on a besoin d'un refresh token
    if (rememberMe) {
      return !!token && !!refreshToken;
    }

    // Sinon, juste le token suffit
    return !!token;
  }
}
