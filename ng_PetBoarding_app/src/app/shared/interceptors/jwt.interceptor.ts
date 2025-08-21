import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

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

  const token = authService.getToken();

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
