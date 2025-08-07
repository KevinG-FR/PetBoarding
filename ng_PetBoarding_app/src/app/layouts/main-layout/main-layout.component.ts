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
  sidenavOpened = signal(false);
  currentYear = signal(new Date().getFullYear());

  toggleSidenav() {
    this.sidenavOpened.update((value) => !value);
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
