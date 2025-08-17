import { CommonModule } from '@angular/common';
import { Component, computed, inject, input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { BasketService } from '../../basket/services/basket.service';
import { Pet } from '../../pets/models/pet.model';
import { Prestation, PrestationFilters } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';
import { PrestationDetailComponent } from './prestation-detail.component';
import { PrestationItemComponent } from './prestation-item.component';

@Component({
  selector: 'app-prestations-list',
  standalone: true,
  imports: [CommonModule, MatIconModule, PrestationItemComponent],
  templateUrl: './prestations-list.component.html',
  styleUrl: './prestations-list.component.scss'
})
export class PrestationsListComponent {
  private prestationsService = inject(PrestationsService);
  private basketService = inject(BasketService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  // Input pour recevoir les filtres du parent
  filters = input<PrestationFilters>({});

  // Données de base
  allPrestations = this.prestationsService.getAllPrestations();

  // Prestations filtrées basées sur les filtres reçus
  filteredPrestations = computed(() => {
    return this.prestationsService.createFilteredPrestations(this.allPrestations(), this.filters());
  });

  // Computed pour les statistiques
  hasResults = computed(() => this.filteredPrestations().length > 0);
  isFiltered = computed(() => this.filteredPrestations().length < this.allPrestations().length);
  resultCount = computed(() => this.filteredPrestations().length);
  totalCount = computed(() => this.allPrestations().length);

  onViewDetails(prestation: Prestation): void {
    const dialogRef = this.dialog.open(PrestationDetailComponent, {
      data: { prestation },
      maxWidth: '90vw',
      width: '1200px',
      maxHeight: '85vh',
      height: 'auto',
      autoFocus: false,
      restoreFocus: true,
      panelClass: 'prestation-detail-dialog'
    });

    // Gérer la fermeture du modal
    dialogRef.afterClosed().subscribe((result) => {
      if (result === 'reserve') {
        this.onReservePrestation(prestation);
      } else if (result && typeof result === 'object' && result.action === 'reserve') {
        // result.pet et result.dates peuvent être présents
        const pet = result.pet as Pet | undefined;
        const dateDebut = result.dateDebut as Date | undefined;
        const dateFin = result.dateFin as Date | undefined;

        this.basketService.addItem(
          prestation,
          1,
          undefined,
          undefined,
          pet ? { id: pet.id, name: pet.name, type: pet.type } : undefined,
          dateDebut,
          dateFin
        );

        const datesText =
          dateDebut && dateFin
            ? ` du ${dateDebut.toLocaleDateString()} au ${dateFin.toLocaleDateString()}`
            : '';

        const snackBarRef = this.snackBar.open(
          `Réservation pour "${prestation.libelle}"${datesText} ajoutée au panier !`,
          'Voir le panier',
          {
            duration: 5000,
            panelClass: ['success-snackbar']
          }
        );

        snackBarRef.onAction().subscribe(() => {
          this.router.navigate(['/basket']);
        });
      }
    });
  }

  onReservePrestation(prestation: Prestation): void {
    // Simuler une réservation
    this.basketService.addItem(prestation);

    const snackBarRef = this.snackBar.open(
      `Réservation pour "${prestation.libelle}" ajoutée au panier !`,
      'Voir le panier',
      {
        duration: 5000,
        panelClass: ['success-snackbar']
      }
    );

    snackBarRef.onAction().subscribe(() => {
      this.router.navigate(['/basket']);
    });
  }
}
