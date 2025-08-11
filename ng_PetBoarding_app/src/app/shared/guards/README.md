# Guards

Ce dossier contient les guards de sécurité pour l'application.

## AuthGuard

Le `authGuard` protège les routes qui nécessitent un utilisateur authentifié.

### Fonctionnalités

- ✅ Vérifie que `currentUser()` du `AuthService` n'est pas null
- ✅ Redirige automatiquement vers `/login` avec une URL propre
- ✅ Solution simple et légère sans gestion d'état complexe

### Utilisation

```typescript
// Dans app.routes.ts
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'profile',
    loadComponent: () => import('./features/profile/components/profile.component'),
    canActivate: [authGuard] // 👈 Protection de la route
  }
];
```

### Routes protégées

- `/profile` - Page de profil utilisateur
- `/reservations` - Page des réservations

### Comportement

1. **Utilisateur connecté** → Accès autorisé à la route
2. **Utilisateur non connecté** → Redirection vers `/login` (URL propre)
