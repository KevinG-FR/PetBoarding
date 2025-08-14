import { Routes } from '@angular/router';

import { authGuard } from './shared/guards/auth.guard';

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
      import('./features/pets/components/pet-form.component').then((m) => m.PetFormComponent),
    canActivate: [authGuard]
  },
  {
    path: 'prestations',
    loadComponent: () =>
      import('./features/prestations/components/prestations.component').then(
        (m) => m.PrestationsComponent
      )
  },
  {
    path: 'reservations',
    loadComponent: () =>
      import('./features/reservations/components/reservations.component').then(
        (m) => m.ReservationsComponent
      ),
    canActivate: [authGuard]
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
  {
    path: '**',
    redirectTo: '/home'
  }
];
