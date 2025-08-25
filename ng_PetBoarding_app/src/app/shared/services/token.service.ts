import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly TOKEN_KEY = 'auth_token';
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

  getRememberMe(): boolean {
    const value = localStorage.getItem(this.REMEMBER_ME_KEY);
    return value === null || value === 'true';
  }

  setRememberMe(remember: boolean): void {
    localStorage.setItem(this.REMEMBER_ME_KEY, remember.toString());
  }

  removeRememberMe(): void {
    localStorage.removeItem(this.REMEMBER_ME_KEY);
  }

  clearAll(): void {
    this.removeToken();
    this.removeRememberMe();
  }

  hasValidToken(): boolean {
    const token = this.getToken();
    return token !== null && token.trim() !== '';
  }
}
