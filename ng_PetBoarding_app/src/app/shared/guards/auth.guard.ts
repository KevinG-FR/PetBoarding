import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../../features/auth/services/auth.service';

/**
 * Guard qui vérifie qu'un utilisateur est connecté
 * Redirige vers la page de connexion si l'utilisateur n'est pas authentifié
 */
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Vérifier si l'utilisateur est connecté (via le token)
  const isAuthenticated = authService.isAuthenticated();

  if (isAuthenticated) {
    return true;
  }

  // Rediriger vers la page de connexion avec URL propre
  router.navigate(['/login']);

  return false;
};
