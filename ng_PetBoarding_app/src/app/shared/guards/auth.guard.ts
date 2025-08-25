import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { TokenService } from '../services/token.service';

export const authGuard: CanActivateFn = (): boolean => {
  const tokenService = inject(TokenService);
  const router = inject(Router);

  const hasToken = tokenService.hasValidToken();

  if (hasToken) {
    return true;
  }

  router.navigate(['/login']);

  return false;
};
