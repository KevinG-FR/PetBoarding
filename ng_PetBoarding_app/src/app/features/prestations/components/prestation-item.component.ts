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
import { BasketService } from '../../basket/services/basket.service';
import { Pet, PetType } from '../../pets/models/pet.model';
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

  private prestationsService = inject(PrestationsService);
  private basketService = inject(BasketService);
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
    const prestation = this.prestation();
    const dialogRef = this.dialog.open(ReservationCompleteDialogComponent, {
      data: { prestation },
      width: '1000px',
      maxWidth: '95vw',
      height: 'auto',
      maxHeight: '90vh'
    });

    dialogRef.afterClosed().subscribe((result: Pet) => {
      const pet = result;

      if (pet) {
        this.basketService.addItem(prestation, 1, undefined, undefined, pet);

        const snackBarRef = this.snackBar.open(
          `${prestation.libelle} ajouté au panier`,
          'Voir le panier',
          {
            duration: 5000
          }
        );

        snackBarRef.onAction().subscribe(() => {
          this.router.navigate(['/basket']);
        });
      }
    });
  }
}
