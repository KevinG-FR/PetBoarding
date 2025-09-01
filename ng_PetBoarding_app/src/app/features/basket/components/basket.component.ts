import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
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
    MatProgressSpinnerModule,
    RouterModule,
    BasketItemComponent
  ],
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})
export class BasketComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  basketService = inject(BasketService);

  ngOnInit(): void {
    this.basketService.loadBasket().subscribe();
  }

  onRemoveItem(itemId: string): void {
    const item = this.basketService.getBasketItem(itemId);
    if (item) {
      this.basketService.removeItemFromBasket(itemId).subscribe(() => {
        this.snackBar.open(`${item.serviceName} retiré du panier`, 'Fermer', {
          duration: 3000
        });
      });
    }
  }

  onPrestationClick(reservationId: string): void {
    this.router.navigate(['/reservations', reservationId]);
  }

  onClearBasket(): void {
    this.basketService.clearBasket().subscribe(() => {
      this.snackBar.open('Panier vidé', 'Fermer', { duration: 3000 });
    });
  }

  onProcessPaymentSuccess(): void {
    this.basketService.processPaymentSuccess().subscribe(() => {
      this.snackBar.open('Paiement traité avec succès !', 'Fermer', {
        duration: 3000
      });
    });
  }

  onProcessPaymentFailure(): void {
    this.basketService.processPaymentFailure().subscribe(() => {
      this.snackBar.open('Échec de paiement simulé', 'Fermer', {
        duration: 3000
      });
    });
  }

  goToPrestations(): void {
    this.router.navigate(['/prestations']);
  }
}
