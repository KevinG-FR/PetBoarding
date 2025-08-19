import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';

/**
 * Interceptor pour gérer les erreurs d'authentification
 */
export const authErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Si erreur 401 (non autorisé), déconnecter l'utilisateur
      if (error.status === 401) {
        // Éviter la déconnexion automatique sur les endpoints de login
        const isLoginAttempt = req.url.includes('/login') || req.url.includes('/authentification');

        if (!isLoginAttempt) {
          // eslint-disable-next-line no-console
          console.warn('Token expiré ou invalide. Déconnexion automatique.');
          authService.logout();
        }
      }

      // Relancer l'erreur pour que les composants puissent la traiter
      return throwError(() => error);
    })
  );
};
