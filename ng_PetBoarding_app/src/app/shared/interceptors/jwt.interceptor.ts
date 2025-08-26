import { HttpClient, HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenRefreshResponseDto } from '../contracts/auth/login-response.dto';
import { TokenService } from '../services/token.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const httpClient = inject(HttpClient);
  const publicEndpoints = [
    { url: '/api/auth/login', method: 'POST' },
    { url: '/api/users', method: 'POST', exact: true }
  ];

  const isPublicEndpoint = publicEndpoints.some((endpoint) => {
    const urlMatches = endpoint.exact
      ? req.url.endsWith(endpoint.url)
      : req.url.includes(endpoint.url);

    return urlMatches && req.method === endpoint.method;
  });

  if (isPublicEndpoint) {
    return next(req);
  }

  const token = tokenService.getToken();

  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // Si 401, rememberMe actif et refreshToken dispo, tenter le refresh
      if (
        error.status === 401 &&
        tokenService.getRememberMe() &&
        tokenService.getRefreshToken() &&
        !req.url.endsWith('/refreshToken')
      ) {
        // Appel API refresh
        const refreshTokenUrl = `${environment.apiUrl}/api/auth/refreshToken`;

        return httpClient
          .post<TokenRefreshResponseDto>(refreshTokenUrl, {
            RefreshToken: tokenService.getRefreshToken()
          })
          .pipe(
            switchMap((refreshResponse) => {
              if (refreshResponse?.success && refreshResponse.token) {
                tokenService.setToken(refreshResponse.token);
                // Note: Le backend ne retourne pas de nouveau refresh token
                // donc on garde l'ancien refresh token

                // Rejouer la requête initiale avec le nouveau token
                const retriedReq = req.clone({
                  setHeaders: {
                    Authorization: `Bearer ${tokenService.getToken()}`
                  }
                });
                return next(retriedReq);
              } else {
                tokenService.clearAll();
                return throwError(() => error);
              }
            }),
            catchError(() => {
              tokenService.clearAll();
              return throwError(() => error);
            })
          );
      }

      // Sinon, comportement normal
      if (error.status === 401 || error.status === 403) {
        tokenService.clearAll();
      }
      return throwError(() => error);
    })
  );
};
