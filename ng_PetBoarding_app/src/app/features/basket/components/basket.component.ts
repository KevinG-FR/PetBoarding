import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';

import { BasketService } from '../services/basket.service';
import { BasketItemComponent } from './basket-item.component';

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatSnackBarModule,
    RouterModule,
    BasketItemComponent
  ],
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})
export class BasketComponent {
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  basketService = inject(BasketService);

  onQuantityChange(event: { itemId: string; quantity: number }): void {
    this.basketService.updateQuantity(event.itemId, event.quantity);
  }

  onRemoveItem(itemId: string): void {
    const item = this.basketService.getItem(itemId);
    if (item) {
      this.basketService.removeItem(itemId);
      this.snackBar.open(`${item.prestation.libelle} retiré du panier`, 'Fermer', {
        duration: 3000
      });
    }
  }

  onPrestationClick(prestationId: string): void {
    this.router.navigate(['/prestations', prestationId]);
  }

  onClearBasket(): void {
    this.basketService.clear();
    this.snackBar.open('Panier vidé', 'Fermer', { duration: 3000 });
  }

  onProceedToCheckout(): void {
    // TODO: Implémenter la logique de checkout
    this.snackBar.open('Fonctionnalité en cours de développement', 'Fermer', {
      duration: 3000
    });
  }

  goToPrestations(): void {
    this.router.navigate(['/prestations']);
  }

  formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes} min`;
    }
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}min` : `${hours}h`;
  }
}
