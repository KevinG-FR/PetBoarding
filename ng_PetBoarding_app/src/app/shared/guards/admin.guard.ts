import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';
import { RoleService } from '../services/role.service';

export const AdminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const roleService = inject(RoleService);
  const router = inject(Router);

  // Vérifier si l'utilisateur est connecté
  if (!authService.isAuthenticated()) {
    router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }

  // Vérifier si l'utilisateur est administrateur
  if (!roleService.isAdmin()) {
    // Rediriger vers une page d'erreur 403 ou la page d'accueil
    router.navigate(['/home']);
    return false;
  }

  return true;
};