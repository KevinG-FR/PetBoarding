import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../../features/auth/services/auth.service';

/**
 * Guard qui vérifie qu'un utilisateur est connecté
 * Redirige vers la page de connexion si l'utilisateur n'est pas authentifié
 * Sauvegarde l'URL de destination dans le localStorage pour un retour après connexion
 */
export const authGuardWithReturn: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Vérifier si l'utilisateur est connecté
  const currentUser = authService.currentUser();

  if (currentUser) {
    return true;
  }

  // Sauvegarder l'URL de destination dans le localStorage
  localStorage.setItem('returnUrl', state.url);

  // Rediriger vers la page de connexion avec URL propre
  router.navigate(['/login']);

  return false;
};
