import { CommonModule } from '@angular/common';
import { Component, computed, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ReservationFilters, StatutReservation } from '../models/reservation.model';
import { ReservationsService, StatutInfo } from '../services/reservations.service';

@Component({
  selector: 'app-reservation-filters',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './reservation-filters.component.html',
  styleUrl: './reservation-filters.component.scss'
})
export class ReservationFiltersComponent {
  private reservationsService = inject(ReservationsService);
  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  filtersChanged = output<ReservationFilters>();

  // Signals pour les filtres
  dateDebut = signal<Date | null>(null);
  dateFin = signal<Date | null>(null);
  selectedStatut = signal<StatutReservation | null>(null);
  selectedAnimalType = signal<'CHIEN' | 'CHAT' | null>(null);

  // Données pour les sélecteurs
  statuts = this.reservationsService.getStatuts();
  animalTypes = ['CHIEN', 'CHAT'] as const;

  // Signal computed pour les informations de statut
  statutsInfo = computed(() => {
    const infos = new Map<StatutReservation, StatutInfo>();
    this.statuts.forEach((statut) => {
      infos.set(statut, this.reservationsService.getStatutInfo(statut));
    });
    return infos;
  });

  onDateDebutChange(date: Date | null) {
    this.dateDebut.set(date);
    this.emitFilters();
  }

  onDateFinChange(date: Date | null) {
    this.dateFin.set(date);
    this.emitFilters();
  }

  onStatutChange(statut: StatutReservation | null) {
    this.selectedStatut.set(statut);
    this.emitFilters();
  }

  onAnimalTypeChange(type: 'CHIEN' | 'CHAT' | null) {
    this.selectedAnimalType.set(type);
    this.emitFilters();
  }

  clearFilters() {
    this.dateDebut.set(null);
    this.dateFin.set(null);
    this.selectedStatut.set(null);
    this.selectedAnimalType.set(null);

    // Annuler le timeout en cours si nécessaire
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
      this.searchTimeout = null;
    }

    this.emitFilters();
  }

  hasActiveFilters(): boolean {
    return (
      !!this.dateDebut() ||
      !!this.dateFin() ||
      !!this.selectedStatut() ||
      !!this.selectedAnimalType()
    );
  }

  getStatutLabel(statut: StatutReservation): string {
    return this.statutsInfo().get(statut)?.label || statut;
  }

  getStatutIcon(statut: StatutReservation): string {
    return this.statutsInfo().get(statut)?.icon || 'fas fa-question-circle';
  }

  getStatutColor(statut: StatutReservation): string {
    return this.statutsInfo().get(statut)?.color || '#666666';
  }

  getAnimalTypeLabel(type: 'CHIEN' | 'CHAT'): string {
    return type === 'CHIEN' ? 'Chien' : 'Chat';
  }

  getAnimalTypeIcon(type: 'CHIEN' | 'CHAT'): string {
    return this.reservationsService.getAnimalIcon(type);
  }

  getAnimalTypeColor(type: 'CHIEN' | 'CHAT'): string {
    return this.reservationsService.getAnimalColor(type);
  }

  // Méthodes pour supprimer les filtres individuellement
  removeFilterDateDebut() {
    this.dateDebut.set(null);
    this.emitFilters();
  }

  removeFilterDateFin() {
    this.dateFin.set(null);
    this.emitFilters();
  }

  removeFilterStatut() {
    this.selectedStatut.set(null);
    this.emitFilters();
  }

  removeFilterAnimalType() {
    this.selectedAnimalType.set(null);
    this.emitFilters();
  }

  private emitFilters() {
    const filters: ReservationFilters = {
      dateDebut: this.dateDebut() || undefined,
      dateFin: this.dateFin() || undefined,
      statut: this.selectedStatut() || undefined,
      animalType: this.selectedAnimalType() || undefined
    };
    this.filtersChanged.emit(filters);
  }
}
