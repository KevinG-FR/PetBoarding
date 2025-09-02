import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { DurationPipe } from '../../../shared/pipes/duration.pipe';
import { AuthService } from '../../auth/services/auth.service';
import { BasketService } from '../../basket/services/basket.service';
import { PetType } from '../../pets/models/pet.model';
import { ReservationsService } from '../../reservations/services/reservations.service';
import { Prestation } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';
import { ReservationCompleteDialogComponent } from './reservation-complete-dialog.component';

@Component({
  selector: 'app-prestation-item',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatDialogModule,
    MatIconModule,
    MatChipsModule,
    DurationPipe
  ],
  templateUrl: './prestation-item.component.html',
  styleUrl: './prestation-item.component.scss'
})
export class PrestationItemComponent {
  prestation = input.required<Prestation>();

  viewDetails = output<Prestation>();

  private isProcessing = false;

  private prestationsService = inject(PrestationsService);
  private basketService = inject(BasketService);
  private reservationsService = inject(ReservationsService);
  private authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  categoryInfo = computed(() =>
    this.prestationsService.getCategoryInfo(this.prestation().categorieAnimal)
  );

  getCategoryChipClass(): string {
    switch (this.prestation().categorieAnimal) {
      case PetType.DOG:
        return 'chip-chien';
      case PetType.CAT:
        return 'chip-chat';
      default:
        return '';
    }
  }

  getPrestationIcon(): string {
    const libelle = this.prestation().libelle.toLowerCase();
    if (libelle.includes('pension')) return 'hotel';
    if (libelle.includes('garderie')) return 'groups';
    if (libelle.includes('toilettage')) return 'content_cut';
    if (libelle.includes('promenade')) return 'directions_walk';
    if (libelle.includes('domicile')) return 'home';
    if (libelle.includes('consultation')) return 'psychology';
    return 'room_service';
  }

  onViewDetails(): void {
    this.viewDetails.emit(this.prestation());
  }

  onReserve(): void {
    if (this.isProcessing) {
      return;
    }

    const prestation = this.prestation();
    const dialogRef = this.dialog.open(ReservationCompleteDialogComponent, {
      data: { prestation },
      width: '1000px',
      maxWidth: '95vw',
      height: 'auto',
      maxHeight: '90vh'
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result?.action === 'reserve' && result.pet && result.dates) {
        this.isProcessing = true;
        const currentUser = this.authService.currentUser();

        if (!currentUser) {
          this.snackBar.open('Vous devez être connecté pour faire une réservation', 'Fermer', {
            duration: 5000
          });
          return;
        }

        const reservationRequest = {
          userId: currentUser.id,
          prestationId: prestation.id,
          animalId: result.pet.id,
          animalName: result.pet.name,
          dateDebut: result.dates.dateDebut,
          dateFin: result.dates.dateFin,
          commentaires: ''
        };

        this.reservationsService.creerReservationAvecPlanning(reservationRequest).subscribe({
          next: (reservation) => {
            if (!reservation) {
              this.isProcessing = false;
              this.snackBar.open('Impossible de créer la réservation', 'Fermer', {
                duration: 5000
              });
              return;
            }

            this.basketService.addItemToBasket(reservation.id).subscribe({
              next: () => {
                this.isProcessing = false;
                const snackBarRef = this.snackBar.open(
                  'Réservation créée et ajoutée au panier !',
                  'Voir le panier',
                  { duration: 5000 }
                );

                snackBarRef.onAction().subscribe(() => {
                  this.router.navigate(['/basket']);
                });
              },
              error: (basketError) => {
                this.isProcessing = false;

                // Gérer spécifiquement les erreurs d'ajout au panier
                const errorMessage =
                  basketError?.error?.error || basketError?.message || 'Erreur inconnue';

                if (
                  errorMessage.includes('already in basket') ||
                  errorMessage.includes('similaire')
                ) {
                  this.snackBar.open(
                    'Une réservation similaire existe déjà dans votre panier',
                    'Fermer',
                    { duration: 6000 }
                  );
                } else {
                  this.snackBar.open(
                    "Erreur lors de l'ajout au panier: " + errorMessage,
                    'Fermer',
                    { duration: 5000 }
                  );
                }
              }
            });
          },
          error: (error) => {
            this.isProcessing = false;
            this.snackBar.open(
              error.message || 'Erreur lors de la création de la réservation',
              'Fermer',
              { duration: 5000 }
            );
          }
        });
      } else {
        // L'utilisateur a annulé la réservation, remettre le flag à false
        this.isProcessing = false;
      }
    });
  }
}
