import { Routes } from '@angular/router';

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
    path: 'services',
    loadComponent: () => import('./components/home/home.component').then((m) => m.HomeComponent) // Temporaire
  },
  {
    path: 'reservations',
    loadComponent: () => import('./components/home/home.component').then((m) => m.HomeComponent) // Temporaire
  },
  {
    path: 'about',
    loadComponent: () => import('./components/home/home.component').then((m) => m.HomeComponent) // Temporaire
  },
  {
    path: 'contact',
    loadComponent: () => import('./components/home/home.component').then((m) => m.HomeComponent) // Temporaire
  },
  {
    path: '**',
    redirectTo: '/home'
  }
];
