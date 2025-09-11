import { Routes } from '@angular/router';

import { adminRoutes } from './features/admin/admin.routes';
import { authGuard } from './shared/guards/auth.guard';
import { AdminGuard } from './shared/guards/admin.guard';
import { CustomerGuard } from './shared/guards/customer.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('./components/home/home.component').then((m) => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/components/register.component').then((m) => m.RegisterComponent)
  },
  {
    path: 'profile',
    loadComponent: () =>
      import('./features/profile/components/profile.component').then((m) => m.ProfileComponent),
    canActivate: [authGuard]
  },
  {
    path: 'profile/edit',
    loadComponent: () =>
      import('./features/profile/components/profile-edit.component').then(
        (m) => m.ProfileEditComponent
      ),
    canActivate: [authGuard]
  },
  {
    path: 'profile/pets/add',
    loadComponent: () =>
      import('./features/pets/components/pet-add.component').then((m) => m.PetAddComponent),
    canActivate: [CustomerGuard]
  },
  {
    path: 'profile/pets/:id',
    loadComponent: () =>
      import('./features/pets/components/pet-details.component').then((m) => m.PetDetailsComponent),
    canActivate: [CustomerGuard]
  },
  {
    path: 'profile/pets/:id/edit',
    loadComponent: () =>
      import('./features/pets/components/pet-details.component').then((m) => m.PetDetailsComponent),
    canActivate: [CustomerGuard]
  },
  {
    path: 'prestations',
    loadComponent: () =>
      import('./features/prestations/components/prestations.component').then(
        (m) => m.PrestationsComponent
      )
  },
  {
    path: 'basket',
    loadComponent: () =>
      import('./features/basket/components/basket.component').then((m) => m.BasketComponent),
    canActivate: [CustomerGuard]
  },
  {
    path: 'reservations',
    loadComponent: () =>
      import('./features/reservations/components/reservations.component').then(
        (m) => m.ReservationsComponent
      ),
    canActivate: [CustomerGuard]
  },
  {
    path: 'about',
    loadComponent: () =>
      import('./features/about/components/about.component').then((m) => m.AboutComponent)
  },
  {
    path: 'contact',
    loadComponent: () =>
      import('./features/contact/components/contact.component').then((m) => m.ContactComponent)
  },
  // Routes d'administration (protégées par AdminGuard)
  ...adminRoutes,
  // TODO: Créer les composants d'administration suivants :
  // - AdminPrestationsComponent pour gérer les prestations
  // - AdminPlanningComponent pour gérer le planning
  {
    path: '**',
    redirectTo: '/home'
  }
];
