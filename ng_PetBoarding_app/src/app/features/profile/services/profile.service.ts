import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { effect, inject, Injectable, signal } from '@angular/core';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { User } from '../../auth/models/user.model';
import { AuthService } from '../../auth/services/auth.service';
import { UpdateProfileResponseDto } from '../contracts/update-profile.dto';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
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
        this._isLoading.set(false);
      } else if (isAuthenticated) {
        // Utilisateur connecté mais pas de données user → charger depuis l'API
        const token = localStorage.getItem('auth_token');
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

  /**
   * Charger le profil de l'utilisateur connecté
   */
  loadUserProfile(): void {
    this._isLoading.set(true);

    // En mode développement, utiliser des données mock
    if (!environment.production) {
      this.getMockUserProfile();
      return;
    }

    const currentUser = this._currentUser();
    if (!currentUser) {
      this._isLoading.set(false);
      return;
    }

    // En production, utiliser l'API
    const apiUrl = `${this.baseApiUrl}/${currentUser.id}`;
    this.http
      .get<UpdateProfileResponseDto>(apiUrl)
      .pipe(
        map((response) => this.mapBackendUserToUser(response)),
        tap((user) => {
          this._currentUser.set(user);
          this._isLoading.set(false);
        }),
        catchError((error) => {
          this._isLoading.set(false);
          // eslint-disable-next-line no-console
          console.error('Erreur lors du chargement du profil:', error);
          return throwError(() => new Error('Impossible de charger le profil utilisateur'));
        })
      )
      .subscribe();
  }

  /**
   * Obtenir des données utilisateur mock pour le développement
   */
  private getMockUserProfile(): void {
    const mockUser: User = {
      id: '1',
      email: 'user@example.com',
      firstName: 'John',
      lastName: 'Doe',
      phone: '+33 6 12 34 56 78',
      address: {
        streetNumber: '123',
        streetName: 'Rue de la Paix',
        city: 'Paris',
        postalCode: '75001',
        country: 'France',
        complement: 'Appartement 4B'
      },
      dateOfBirth: new Date('1990-05-15'),
      createdAt: new Date('2023-01-15'),
      updatedAt: new Date('2024-08-14'),
      isActive: true
    };

    this._currentUser.set(mockUser);
    this._isLoading.set(false);
  }

  /**
   * Mettre à jour le profil utilisateur
   */
  updateUserProfile(updates: Partial<User>): Observable<User> {
    const currentUser = this._currentUser();
    if (!currentUser) {
      throw new Error('Aucun utilisateur connecté');
    }

    // Pour tester l'API, utilisons directement l'appel backend même en développement
    // Commenté temporairement pour tester l'API
    /*
    // En mode développement, simuler la mise à jour
    if (!environment.production) {
      const updatedUser = {
        ...currentUser,
        ...updates,
        updatedAt: new Date()
      };

      this._currentUser.set(updatedUser);
      return of(updatedUser).pipe(delay(500));
    }
    */

    // En production (et maintenant aussi en développement), utiliser l'API
    const profileData = {
      firstname: updates.firstName || currentUser.firstName,
      lastname: updates.lastName || currentUser.lastName,
      phoneNumber: updates.phone || currentUser.phone,
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

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    return this.http.put<any>(apiUrl, profileData).pipe(
      map((response) => this.mapBackendUserToUser(response)),
      tap((user) => {
        this._currentUser.set(user);
      }),
      catchError(this.handleError)
    );
  }

  /**
   * Mapper la réponse API de notre backend .NET vers le modèle User
   */
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private mapBackendUserToUser(user: any): User {
    return {
      id: user?.id?.value || user?.id || '',
      firstName: user?.firstname?.value || user?.firstname || user?.firstName || '',
      lastName: user?.lastname?.value || user?.lastname || user?.lastName || '',
      email: user?.email?.value || user?.email || '',
      phone: user?.phoneNumber?.value || user?.phoneNumber || user?.phone || '',
      address: user?.address
        ? {
            id: user.address.id,
            streetNumber: user.address.streetNumber.value || '',
            streetName: user.address.streetName.value || '',
            city: user.address.city.value || '',
            postalCode: user.address.postalCode.value || '',
            country: user.address.country.value || '',
            complement: user.address.complement?.value || ''
          }
        : undefined,
      createdAt: new Date(user?.createdAt || Date.now()),
      updatedAt: new Date(user?.updatedAt || Date.now()),
      isActive: user?.status === 'Confirmed' || user?.status === 'Created'
    };
  }

  /**
   * Mapper la réponse API vers le modèle User (ancien format)
   */
  private mapToUser(response: UpdateProfileResponseDto): User {
    return {
      id: response.id,
      firstName: response.firstName,
      lastName: response.lastName,
      email: response.email,
      phone: response.phone,
      createdAt: new Date(response.createdAt),
      updatedAt: new Date(response.updatedAt),
      isActive: response.isActive
    };
  }

  /**
   * Gérer les erreurs HTTP
   */
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
