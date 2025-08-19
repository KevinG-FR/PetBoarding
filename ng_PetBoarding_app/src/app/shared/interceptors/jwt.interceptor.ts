import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';

/**
 * Interceptor pour ajouter automatiquement le token JWT aux requêtes HTTP
 */
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  // URLs qui ne nécessitent pas de token (routes publiques)
  const publicEndpoints = [
    { url: '/api/users/login', method: 'POST' },
    { url: '/api/users', method: 'POST', exact: true } // Création d'utilisateur uniquement
  ];

  // Vérifier si cette requête est vers un endpoint public
  const isPublicEndpoint = publicEndpoints.some((endpoint) => {
    const urlMatches = endpoint.exact
      ? req.url.endsWith(endpoint.url) // Match exact pour éviter les faux positifs
      : req.url.includes(endpoint.url);

    return urlMatches && req.method === endpoint.method;
  });

  // Si c'est un endpoint public, ne pas ajouter le token
  if (isPublicEndpoint) {
    return next(req);
  }

  // Pour tous les autres endpoints, ajouter le token si disponible
  const token = authService.getToken();

  if (token) {
    // Cloner la requête et ajouter l'en-tête Authorization
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next(authReq);
  }

  // Si pas de token pour un endpoint protégé, continuer sans token
  // L'API retournera une erreur 401 si nécessaire
  return next(req);
};
