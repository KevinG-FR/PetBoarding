import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthService } from '../../features/auth/services/auth.service';
import { BasketMiniComponent } from '../../features/basket/components/basket-mini.component';
import { ProfileService } from '../../features/profile/services/profile.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatSidenavModule,
    MatDividerModule,
    BasketMiniComponent
  ],
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
  private readonly authService = inject(AuthService);
  private readonly profileService = inject(ProfileService);

  // Signals pour l'état du layout
  sidenavOpened = signal(false);

  // Getters pour l'authentification
  isAuthenticated = this.authService.isAuthenticated;
  currentUser = this.profileService.currentUser;

  // Année courante pour le footer
  currentYear = signal(new Date().getFullYear());

  /**
   * Basculer l'état du sidenav
   */
  toggleSidenav(): void {
    this.sidenavOpened.update((value) => !value);
  }

  /**
   * Déconnexion utilisateur
   */
  logout(): void {
    this.authService.logout();
  }

  /**
   * Basculer l'authentification (pour tests)
   */
  toggleAuth(): void {
    this.authService.toggleAuthForTesting();
  }
}
