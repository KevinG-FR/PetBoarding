import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

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
    MatDividerModule
  ],
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss']
})
export class MainLayoutComponent {
  isAuthenticated = signal(false);
  isMobile = signal(false);
  isTablet = signal(false);
  sidenavOpened = signal(false);
  currentYear = new Date().getFullYear();

  constructor(private breakpointObserver: BreakpointObserver) {
    // Détection responsive améliorée
    this.breakpointObserver
      .observe([
        '(max-width: 768px)', // Mobile et petites tablettes
        Breakpoints.Handset,
        Breakpoints.Small
      ])
      .subscribe((result) => {
        this.isMobile.set(result.matches);
        if (!result.matches) {
          this.sidenavOpened.set(false);
        }
      });

    // Détection tablette séparée
    this.breakpointObserver
      .observe(['(min-width: 769px) and (max-width: 1024px)'])
      .subscribe((result) => {
        this.isTablet.set(result.matches);
      });
  }

  toggleSidenav() {
    this.sidenavOpened.update((value) => !value);
  }

  login() {
    // Navigation vers la page de login
    // TODO: Implémenter la navigation
  }

  logout() {
    this.isAuthenticated.set(false);
    // TODO: Nettoyer la session utilisateur
  }

  toggleAuth() {
    // Pour la démo, on bascule l'état d'authentification
    this.isAuthenticated.update((value) => !value);
  }
}
