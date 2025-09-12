import { Routes } from '@angular/router';
import { AdminGuard } from '../../shared/guards/admin.guard';

export const adminRoutes: Routes = [
  // User management routes
  {
    path: 'admin/users',
    loadComponent: () =>
      import('./components/user-list/user-list.component').then((m) => m.UserListComponent),
    canActivate: [AdminGuard]
  },
  {
    path: 'admin/users/add',
    loadComponent: () =>
      import('./components/add-user/add-user.component').then((m) => m.AddUserComponent),
    canActivate: [AdminGuard]
  },
  {
    path: 'admin/users/:id',
    loadComponent: () =>
      import('./components/user-detail/user-detail.component').then((m) => m.UserDetailComponent),
    canActivate: [AdminGuard]
  },
  {
    path: 'admin/users/:id/edit',
    loadComponent: () =>
      import('./components/user-detail/user-detail.component').then((m) => m.UserDetailComponent),
    canActivate: [AdminGuard]
  },
  
  // Prestation management routes
  {
    path: 'admin/prestations',
    loadComponent: () =>
      import('./components/prestation-list/admin-prestation-list.component').then((m) => m.AdminPrestationListComponent),
    canActivate: [AdminGuard]
  },
  {
    path: 'admin/prestations/add',
    loadComponent: () =>
      import('./components/prestation-form/admin-prestation-form.component').then((m) => m.AdminPrestationFormComponent),
    canActivate: [AdminGuard]
  },
  {
    path: 'admin/prestations/:id',
    loadComponent: () =>
      import('./components/prestation-detail/admin-prestation-detail.component').then((m) => m.AdminPrestationDetailComponent),
    canActivate: [AdminGuard]
  },
  {
    path: 'admin/prestations/:id/edit',
    loadComponent: () =>
      import('./components/prestation-form/admin-prestation-form.component').then((m) => m.AdminPrestationFormComponent),
    canActivate: [AdminGuard]
  }
];