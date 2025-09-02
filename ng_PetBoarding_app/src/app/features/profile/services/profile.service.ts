import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { effect, inject, Injectable, signal } from '@angular/core';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { TokenService } from '../../../shared/services/token.service';
import { User } from '../../auth/models/user.model';
import { AuthService } from '../../auth/services/auth.service';
import {
  GetProfileResponseDto,
  UpdateProfileRequestDto,
  UpdateProfileResponseDto
} from '../contracts/update-profile.dto';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
  private readonly tokenService = inject(TokenService);
  private readonly baseApiUrl = `${environment.apiUrl}/api/users`;

  // Signals pour l'état du profil utilisateur
  private readonly _currentUser = signal<User | null>(null);
  private readonly _isLoading = signal(false);

  // Getters publics
  currentUser = this._currentUser.asReadonly();
  isLoading = this._isLoading.asReadonly();

  constructor() {
    // Réagir aux changements d'authentification avec effect()
    effect(() => {
      const isAuthenticated = this.authService.isAuthenticated();
      const currentUser = this.authService.currentUser();

      if (isAuthenticated && currentUser) {
        // Utilisateur connecté → utiliser les données du AuthService
        this._currentUser.set(currentUser);
        this._currentUser.set(currentUser);
        this._isLoading.set(false);
      } else if (isAuthenticated) {
        // Utilisateur connecté mais pas de données user → charger depuis l'API
        const token = this.tokenService.getToken();
        this._isLoading.set(true);
        if (token && !this._currentUser()) {
          this.loadUserProfile();
        }
      } else {
        // Utilisateur déconnecté → nettoyer les données
        this.clearUserData();
      }
    });
  }

  private clearUserData(): void {
    this._currentUser.set(null);
    this._isLoading.set(false);
  }

  loadUserProfile(): void {
    this._isLoading.set(true);

    const currentUser = this._currentUser();
    if (!currentUser) {
      this._isLoading.set(false);
      return;
    }

    const apiUrl = `${this.baseApiUrl}/${currentUser.id}`;
    this.http
      .get<GetProfileResponseDto>(apiUrl)
      .pipe(
        map((response: GetProfileResponseDto) => this.mapBackendUserToUser(response)),
        tap((user: User) => {
          this._currentUser.set(user);
          this._isLoading.set(false);
        }),
        catchError((_error: HttpErrorResponse) => {
          this._isLoading.set(false);
          return throwError(() => new Error('Impossible de charger le profil utilisateur'));
        })
      )
      .subscribe();
  }

  updateUserProfile(updates: Partial<User>): Observable<User> {
    const currentUser = this._currentUser();
    if (!currentUser) {
      throw new Error('Aucun utilisateur connecté');
    }
    const profileData: UpdateProfileRequestDto = {
      firstname: updates.firstName || currentUser.firstName,
      lastname: updates.lastName || currentUser.lastName,
      email: updates.email || currentUser.email,
      phoneNumber: updates.phoneNumber || currentUser.phoneNumber,
      address: updates.address
        ? {
            streetNumber: updates.address.streetNumber,
            streetName: updates.address.streetName,
            city: updates.address.city,
            postalCode: updates.address.postalCode,
            country: updates.address.country,
            complement: updates.address.complement
          }
        : currentUser.address
    };

    const apiUrl = `${this.baseApiUrl}/${currentUser.id}/profile`;

    return this.http.put<UpdateProfileResponseDto>(apiUrl, profileData).pipe(
      map((response: UpdateProfileResponseDto) => this.mapBackendUserToUser(response)),
      tap((user: User) => {
        this._currentUser.set(user);
      }),
      catchError(this.handleError)
    );
  }

  private mapBackendUserToUser(response: UpdateProfileResponseDto): User {
    if (!response?.user) {
      throw new Error('Données utilisateur manquantes');
    }

    return {
      id: response.user.id ?? '',
      firstName: response.user.firstName ?? '',
      lastName: response.user.lastName ?? '',
      email: response.user.email ?? '',
      phoneNumber: response.user.phoneNumber ?? '',
      profileType: response.user.profileType ?? '',
      status: response.user.status ?? '',
      address: response.user.address
        ? {
            streetNumber: response.user.address.streetNumber ?? '',
            streetName: response.user.address.streetName ?? '',
            city: response.user.address.city ?? '',
            postalCode: response.user.address.postalCode ?? '',
            country: response.user.address.country ?? '',
            complement: response.user.address.complement ?? ''
          }
        : undefined,
      createdAt: new Date(response.user.createdAt ?? Date.now()),
      updatedAt: new Date(response.user.updatedAt ?? Date.now())
    };
  }

  private handleError = (error: HttpErrorResponse): Observable<never> => {
    let errorMessage = "Une erreur inattendue s'est produite";

    if (error.error instanceof ErrorEvent) {
      // Erreur côté client
      errorMessage = `Erreur: ${error.error.message}`;
    } else {
      // Erreur côté serveur
      switch (error.status) {
        case 400:
          errorMessage = 'Données invalides. Veuillez vérifier vos informations.';
          break;
        case 401:
          errorMessage = 'Vous devez être connecté pour effectuer cette action.';
          break;
        case 403:
          errorMessage = "Vous n'avez pas les permissions nécessaires.";
          break;
        case 404:
          errorMessage = 'Profil non trouvé.';
          break;
        case 409:
          errorMessage = 'Cette adresse email est déjà utilisée par un autre compte.';
          break;
        case 500:
          errorMessage = 'Erreur du serveur. Veuillez réessayer plus tard.';
          break;
        default:
          errorMessage = `Erreur ${error.status}: ${error.message}`;
      }
    }

    return throwError(() => new Error(errorMessage));
  };
}
