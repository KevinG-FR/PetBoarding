import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, of, tap } from 'rxjs';
import { RegisterRequestDto } from '../../../shared/contracts/auth/register-request.dto';
import { RegisterResponseDto } from '../../../shared/contracts/auth/register-response.dto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  // Signals pour l'état d'authentification
  private readonly _isAuthenticated = signal(false);
  private readonly _isLoading = signal(false);

  // API base URL (à configurer selon votre backend)
  private readonly apiUrl = '/api/auth';

  // Getters publics
  isAuthenticated = this._isAuthenticated.asReadonly();
  isLoading = this._isLoading.asReadonly();

  constructor() {
    // Vérifier s'il y a un token stocké au démarrage
    this.checkStoredAuth();
  }

  /**
   * Inscription d'un nouvel utilisateur
   */
  register(registerData: RegisterRequestDto): Observable<RegisterResponseDto> {
    this._isLoading.set(true);

    return this.http.post<RegisterResponseDto>(`${this.apiUrl}/register`, registerData).pipe(
      tap((response) => {
        if (response.success && response.token) {
          // Stocker le token
          localStorage.setItem('auth_token', response.token);
          this._isAuthenticated.set(true);
        }
      }),
      catchError((error) => {
        // eslint-disable-next-line no-console
        console.error("Erreur lors de l'inscription:", error);
        return of({
          success: false,
          message: "Une erreur est survenue lors de l'inscription"
        });
      }),
      tap(() => this._isLoading.set(false))
    );
  }

  /**
   * Connexion utilisateur
   */
  login(
    _email: string,
    _password: string
  ): Observable<{ success: boolean; token?: string; message?: string }> {
    this._isLoading.set(true);

    // TODO: Implémenter la connexion avec l'API
    // Simulation pour le développement
    return of({ success: true, token: 'mock-jwt-token' }).pipe(
      tap((response) => {
        if (response.success && response.token) {
          localStorage.setItem('auth_token', response.token);
          this._isAuthenticated.set(true);
        }
      }),
      tap(() => this._isLoading.set(false))
    );
  }

  /**
   * Déconnexion utilisateur
   */
  logout(): void {
    localStorage.removeItem('auth_token');
    this._isAuthenticated.set(false);

    this.router.navigate(['/home']);
  }

  /**
   * Obtenir le token d'authentification
   */
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  /**
   * Vérifier l'authentification stockée
   */
  private checkStoredAuth(): void {
    const token = localStorage.getItem('auth_token');
    if (token) {
      this._isAuthenticated.set(true);
    }
  }

  /**
   * Simulation pour basculer l'état d'authentification (développement)
   */
  toggleAuthForTesting(): void {
    const isAuth = this._isAuthenticated();
    if (isAuth) {
      this.logout();
    } else {
      this._isAuthenticated.set(true);
      localStorage.setItem('auth_token', 'mock-jwt-token');
    }
  }
}
