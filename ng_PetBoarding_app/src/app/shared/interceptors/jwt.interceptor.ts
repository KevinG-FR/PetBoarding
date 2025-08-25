import { HttpClient, HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { TokenService } from '../services/token.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const publicEndpoints = [
    { url: '/api/users/login', method: 'POST' },
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
        !req.url.endsWith('/refresh')
      ) {
        // Appel API refresh
        const httpClient = inject(HttpClient);

        return httpClient
          .post<any>('/api/users/refresh', {
            refreshToken: tokenService.getRefreshToken()
          })
          .pipe(
            switchMap((refreshResponse) => {
              if (refreshResponse?.success && refreshResponse.token) {
                tokenService.setToken(refreshResponse.token);
                if (refreshResponse.refreshToken) {
                  tokenService.setRefreshToken(refreshResponse.refreshToken);
                }
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
