import { Routes } from '@angular/router';
import { AdminGuard } from '../../shared/guards/admin.guard';

export const adminRoutes: Routes = [
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
  }
];