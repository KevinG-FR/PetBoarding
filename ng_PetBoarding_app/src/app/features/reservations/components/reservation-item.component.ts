import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
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
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  reservation = input.required<Reservation>();
  reservationCancelled = output<string>();

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
    const dateFin = this.reservation().dateFin;
    if (!dateFin) return 1;

    const fin = new Date(dateFin);
    const diffTime = Math.abs(fin.getTime() - debut.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays === 0 ? 1 : diffDays; // Minimum 1 jour
  }

  isPast(): boolean {
    const dateFin = this.reservation().dateFin;
    if (!dateFin) return false;
    return new Date(dateFin) < new Date();
  }

  isCurrent(): boolean {
    const now = new Date();
    const debut = new Date(this.reservation().dateDebut);
    const dateFin = this.reservation().dateFin;

    if (!dateFin) {
      // Pour les réservations d'une journée, vérifie si c'est aujourd'hui
      return debut.toDateString() === now.toDateString();
    }

    const fin = new Date(dateFin);
    return debut <= now && now <= fin;
  }

  isFuture(): boolean {
    return new Date(this.reservation().dateDebut) > new Date();
  }

  /**
   * Annule la réservation après confirmation
   */
  annulerReservation(): void {
    const reservation = this.reservation();

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        title: "Confirmer l'annulation",
        message: `Êtes-vous sûr de vouloir annuler la réservation pour ${reservation.animalNom} ?`,
        confirmText: 'Annuler la réservation',
        cancelText: 'Conserver',
        confirmButtonColor: 'warn'
      },
      maxWidth: '400px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result === true) {
        this.proceedWithCancellation();
      }
    });
  }

  /**
   * Procède à l'annulation de la réservation
   */
  private proceedWithCancellation(): void {
    const reservation = this.reservation();

    this.reservationsService.annulerReservationAvecPlanning(reservation.id).subscribe({
      next: (success) => {
        if (success) {
          this.snackBar.open(
            `Réservation pour ${reservation.animalNom} annulée avec succès`,
            'Fermer',
            {
              duration: 4000,
              panelClass: ['success-snackbar']
            }
          );
          // Émettre l'événement pour informer le parent
          this.reservationCancelled.emit(reservation.id);
        } else {
          this.showErrorMessage("Impossible d'annuler la réservation. Veuillez réessayer.");
        }
      },
      error: (error) => {
        console.error("Erreur lors de l'annulation:", error);
        this.showErrorMessage("Une erreur est survenue lors de l'annulation de la réservation.");
      }
    });
  }

  /**
   * Affiche un message d'erreur via snackbar
   */
  private showErrorMessage(message: string): void {
    this.snackBar.open(message, 'Fermer', {
      duration: 5000,
      panelClass: ['error-snackbar']
    });
  }
}
