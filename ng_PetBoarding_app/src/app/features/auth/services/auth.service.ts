import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, of, tap } from 'rxjs';
import { RegisterRequestDto } from '../../../shared/contracts/auth/register-request.dto';
import { RegisterResponseDto } from '../../../shared/contracts/auth/register-response.dto';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  // Signals pour l'état d'authentification
  private readonly _isAuthenticated = signal(false);
  private readonly _currentUser = signal<User | null>(null);
  private readonly _isLoading = signal(false);

  // API base URL (à configurer selon votre backend)
  private readonly apiUrl = '/api/auth';

  // Getters publics
  isAuthenticated = this._isAuthenticated.asReadonly();
  currentUser = this._currentUser.asReadonly();
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

          // Optionnel : récupérer les données utilisateur
          this.loadUserProfile();
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
   * Connexion utilisateur (pour plus tard)
   */
  login(email: string, password: string): Observable<any> {
    // TODO: Implémenter la connexion
    return of({ success: false, message: 'Non implémenté' });
  }

  /**
   * Déconnexion utilisateur
   */
  logout(): void {
    localStorage.removeItem('auth_token');
    this._isAuthenticated.set(false);
    this._currentUser.set(null);
    this.router.navigate(['/home']);
  }

  /**
   * Vérifier l'authentification stockée
   */
  private checkStoredAuth(): void {
    const token = localStorage.getItem('auth_token');
    if (token) {
      this._isAuthenticated.set(true);
      this.loadUserProfile();
    }
  }

  /**
   * Charger le profil utilisateur
   */
  private loadUserProfile(): void {
    // TODO: Implémenter le chargement du profil depuis l'API
    // Pour l'instant, on simule avec des données fictives
    setTimeout(() => {
      this._currentUser.set({
        id: '1',
        email: 'user@example.com',
        firstName: 'John',
        lastName: 'Doe',
        phone: '+33 6 12 34 56 78',
        createdAt: new Date(),
        isActive: true
      });
    }, 1000);
  }

  /**
   * Mettre à jour les données de l'utilisateur actuel
   */
  updateCurrentUser(user: User): void {
    this._currentUser.set(user);
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
      this.loadUserProfile();
    }
  }
}
