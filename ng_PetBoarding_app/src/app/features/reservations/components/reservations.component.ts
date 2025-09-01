import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';
import { ReservationFilters } from '../models/reservation.model';
import { ReservationsService } from '../services/reservations.service';
import { ReservationFiltersComponent } from './reservation-filters.component';
import { ReservationsListComponent } from './reservations-list.component';

@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    ReservationFiltersComponent,
    ReservationsListComponent
  ],
  templateUrl: './reservations.component.html',
  styleUrl: './reservations.component.scss'
})
export class ReservationsComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly reservationsService = inject(ReservationsService);

  // Gestion centralisée des filtres
  filters = signal<ReservationFilters>({});

  ngOnInit() {
    this.loadUserReservations();
  }

  private loadUserReservations() {
    const currentUser = this.authService.currentUser();
    if (currentUser?.id) {
      this.reservationsService.loadUserReservations(currentUser.id).subscribe();
    }
  }

  onFiltersChanged(newFilters: ReservationFilters) {
    this.filters.set(newFilters);
  }
}
