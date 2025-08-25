import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
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

  if (token) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next(authReq);
  }

  return next(req);
};
