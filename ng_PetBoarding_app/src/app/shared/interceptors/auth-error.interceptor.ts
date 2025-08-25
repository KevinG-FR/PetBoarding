import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';

export const authErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        const isLoginAttempt = req.url.includes('/login') || req.url.includes('/authentification');

        if (!isLoginAttempt) {
          // Nettoyer les tokens
          tokenService.clearAll();

          // Redirection simple via window.location pour éviter les dépendances circulaires
          setTimeout(() => {
            window.location.href = '/login';
          }, 100);
        }
      }

      return throwError(() => error);
    })
  );
};
