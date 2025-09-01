import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { ReservationFilters } from '../models/reservation.model';
import { ReservationsService } from '../services/reservations.service';
import { ReservationItemComponent } from './reservation-item.component';

@Component({
  selector: 'app-reservations-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatPaginatorModule,
    MatSelectModule,
    MatTooltipModule,
    RouterModule,
    ReservationItemComponent
  ],
  templateUrl: './reservations-list.component.html',
  styleUrl: './reservations-list.component.scss'
})
export class ReservationsListComponent {
  private reservationsService = inject(ReservationsService);

  // Input pour recevoir les filtres du parent
  filters = input<ReservationFilters>({});

  // Pagination
  pageSize = signal(5);
  pageIndex = signal(0);
  pageSizeOptions = [5, 10, 20];

  // Données de base
  allReservations = this.reservationsService.getAllReservations();
  loading = this.reservationsService.getLoading();
  error = this.reservationsService.getError();

  // Réservations filtrées basées sur les filtres reçus
  filteredReservations = computed(() => {
    return this.reservationsService.createFilteredReservations(
      this.allReservations(),
      this.filters()
    );
  });

  // Réservations paginées
  paginatedReservations = computed(() => {
    const filtered = this.filteredReservations();
    const startIndex = this.pageIndex() * this.pageSize();
    const endIndex = startIndex + this.pageSize();
    return filtered.slice(startIndex, endIndex);
  });

  // Total des éléments pour la pagination
  totalItems = computed(() => this.filteredReservations().length);

  // Gestion des événements de pagination
  onPageChange(event: PageEvent) {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  // Computed pour les statistiques
  hasResults = computed(() => this.filteredReservations().length > 0);
  isFiltered = computed(() => this.filteredReservations().length < this.allReservations().length);
  resultCount = computed(() => this.filteredReservations().length);
  totalCount = computed(() => this.allReservations().length);

  retryLoad() {
    // Déclencher un nouveau chargement depuis le composant parent
    window.location.reload();
  }
}
