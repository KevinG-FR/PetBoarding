import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { DurationPipe } from '../../../shared/pipes/duration.pipe';
import { BasketService } from '../../basket/services/basket.service';
import { CategorieAnimal, Prestation } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';

@Component({
  selector: 'app-prestation-item',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    DurationPipe
  ],
  templateUrl: './prestation-item.component.html',
  styleUrl: './prestation-item.component.scss'
})
export class PrestationItemComponent {
  prestation = input.required<Prestation>();

  // Événements émis par le composant
  viewDetails = output<Prestation>();

  private prestationsService = inject(PrestationsService);
  private basketService = inject(BasketService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  // Computed pour optimiser les appels répétés
  categoryInfo = computed(() =>
    this.prestationsService.getCategoryInfo(this.prestation().categorieAnimal)
  );

  getCategoryChipClass(): string {
    switch (this.prestation().categorieAnimal) {
      case CategorieAnimal.CHIEN:
        return 'chip-chien';
      case CategorieAnimal.CHAT:
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
    this.basketService.addItem(prestation);

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
}
