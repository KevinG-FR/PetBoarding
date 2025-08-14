import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { environment } from '../../../../environments/environment';
import { UpdateProfileRequestDto, UpdateProfileResponseDto } from '../contracts/update-profile.dto';
import { User } from '../../auth/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/profile`;

  /**
   * Mettre à jour le profil utilisateur
   */
  updateProfile(profileData: UpdateProfileRequestDto): Observable<User> {
    return this.http.put<UpdateProfileResponseDto>(`${this.apiUrl}`, profileData).pipe(
      map((response) => this.mapToUser(response)),
      catchError(this.handleError)
    );
  }

  /**
   * Obtenir le profil utilisateur actuel
   */
  getCurrentProfile(): Observable<User> {
    return this.http.get<UpdateProfileResponseDto>(`${this.apiUrl}`).pipe(
      map((response) => this.mapToUser(response)),
      catchError(this.handleError)
    );
  }

  /**
   * Mapper la réponse API vers le modèle User
   */
  private mapToUser(response: UpdateProfileResponseDto): User {
    return {
      id: response.id,
      firstName: response.firstName,
      lastName: response.lastName,
      email: response.email,
      phone: response.phone,
      createdAt: new Date(response.createdAt),
      isActive: response.isActive
    };
  }

  /**
   * Gérer les erreurs HTTP
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
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
  }
}
