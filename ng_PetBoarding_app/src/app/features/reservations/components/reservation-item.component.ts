import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Reservation } from '../models/reservation.model';
import { ReservationsService } from '../services/reservations.service';

@Component({
  selector: 'app-reservation-item',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, DatePipe],
  templateUrl: './reservation-item.component.html',
  styleUrl: './reservation-item.component.scss'
})
export class ReservationItemComponent {
  private reservationsService = inject(ReservationsService);

  reservation = input.required<Reservation>();

  // Méthodes utilitaires
  getStatutInfo() {
    return this.reservationsService.getStatutInfo(this.reservation().statut);
  }

  getAnimalIcon() {
    return this.reservationsService.getAnimalIcon(this.reservation().animalType);
  }

  getAnimalColor() {
    return this.reservationsService.getAnimalColor(this.reservation().animalType);
  }

  getDureeSejourEnJours(): number {
    const debut = new Date(this.reservation().dateDebut);
    const fin = new Date(this.reservation().dateFin);
    const diffTime = Math.abs(fin.getTime() - debut.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays === 0 ? 1 : diffDays; // Minimum 1 jour
  }

  isPast(): boolean {
    return new Date(this.reservation().dateFin) < new Date();
  }

  isCurrent(): boolean {
    const now = new Date();
    const debut = new Date(this.reservation().dateDebut);
    const fin = new Date(this.reservation().dateFin);
    return debut <= now && now <= fin;
  }

  isFuture(): boolean {
    return new Date(this.reservation().dateDebut) > new Date();
  }
}
