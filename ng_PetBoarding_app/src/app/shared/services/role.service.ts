import { Injectable, inject, computed } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { ProfileType } from '../enums/profile-type.enum';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly authService = inject(AuthService);

  // Computed signals pour les vérifications de rôles
  readonly isAdmin = computed(() => {
    const user = this.authService.currentUser();
    return user?.profileType === ProfileType.Administrator;
  });

  readonly isEmployee = computed(() => {
    const user = this.authService.currentUser();
    return user?.profileType === ProfileType.Employee;
  });

  readonly isCustomer = computed(() => {
    const user = this.authService.currentUser();
    return user?.profileType === ProfileType.Customer;
  });

  readonly isStaff = computed(() => {
    return this.isAdmin() || this.isEmployee();
  });

  readonly canAccessAdministration = computed(() => {
    return this.isAdmin();
  });

  readonly canMakeReservations = computed(() => {
    return this.isCustomer();
  });
}