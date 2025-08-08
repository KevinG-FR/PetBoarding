import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { ReservationFilters } from '../models/reservation.model';
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
export class ReservationsComponent {
  // Gestion centralisée des filtres
  filters = signal<ReservationFilters>({});

  onFiltersChanged(newFilters: ReservationFilters) {
    this.filters.set(newFilters);
  }
}
