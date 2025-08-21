import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';

export const authErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        const isLoginAttempt = req.url.includes('/login') || req.url.includes('/authentification');

        if (!isLoginAttempt) {
          authService.logout();
        }
      }

      return throwError(() => error);
    })
  );
};
