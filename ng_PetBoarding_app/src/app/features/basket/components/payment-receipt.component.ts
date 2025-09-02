import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { Router } from '@angular/router';

import { PaymentReceipt } from '../models/payment-receipt.model';

@Component({
  selector: 'app-payment-receipt',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatListModule,
    CurrencyPipe,
    DatePipe
  ],
  template: `
    <div class="receipt-container">
      <mat-card class="receipt-card">
        <!-- Header avec icône de succès -->
        <mat-card-header class="receipt-header">
          <mat-card-title class="text-center">
            <mat-icon class="success-icon">check_circle</mat-icon>
            Paiement réussi !
          </mat-card-title>
          <mat-card-subtitle class="text-center">
            {{ confirmationText }}
          </mat-card-subtitle>
        </mat-card-header>

        <mat-card-content class="receipt-content">
          <!-- Informations de paiement -->
          <div class="payment-info mb-4">
            <h3 class="info-title">
              <mat-icon>payment</mat-icon>
              Détails du paiement
            </h3>
            <div class="info-grid">
              <div class="info-item">
                <span class="info-label">Moyen de paiement :</span>
                <span class="info-value">{{ receipt().paymentMethod }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">Montant payé :</span>
                <span class="info-value amount-highlight">{{ receipt().totalAmount | currency: 'EUR':'symbol':'1.2-2' }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">Date de paiement :</span>
                <span class="info-value">{{ receipt().paidAt | date: 'dd/MM/yyyy à HH:mm' }}</span>
              </div>
              @if (receipt().paymentId) {
                <div class="info-item">
                  <span class="info-label">ID de transaction :</span>
                  <span class="info-value">{{ receipt().paymentId }}</span>
                </div>
              }
            </div>
          </div>

          <mat-divider class="my-4"></mat-divider>

          <!-- Liste des articles -->
          <div class="items-section mb-4">
            <h3 class="info-title">
              <mat-icon>list_alt</mat-icon>
              {{ itemsSectionTitle }}
            </h3>
            <div class="receipt-items">
              @for (item of receipt().items; track item.id) {
                <div class="receipt-item">
                  <div class="item-details">
                    <div class="item-name">{{ item.serviceName }}</div>
                    <div class="item-dates text-muted">{{ item.reservationDates }}</div>
                  </div>
                  <div class="item-price">
                    {{ item.reservationPrice | currency: 'EUR':'symbol':'1.2-2' }}
                  </div>
                </div>
                @if (!$last) {
                  <mat-divider></mat-divider>
                }
              }
            </div>
          </div>

          <mat-divider class="my-4"></mat-divider>

          <!-- Résumé total -->
          <div class="total-section">
            <div class="total-row">
              <span class="total-label">Nombre d'articles :</span>
              <span class="total-value">{{ receipt().totalItems }}</span>
            </div>
            <div class="total-row total-amount">
              <span class="total-label">Montant total payé :</span>
              <span class="total-value">
                {{ receipt().totalAmount | currency: 'EUR':'symbol':'1.2-2' }}
              </span>
            </div>
          </div>
        </mat-card-content>

        <mat-card-actions class="receipt-actions">
          <button mat-raised-button color="primary" (click)="goToReservations()" class="action-btn">
            <mat-icon>event</mat-icon>
            Voir mes réservations
          </button>
          <button mat-stroked-button (click)="goToPrestations()" class="action-btn">
            <mat-icon>add_shopping_cart</mat-icon>
            Continuer mes achats
          </button>
          <button mat-stroked-button (click)="printReceipt()" class="action-btn">
            <mat-icon>print</mat-icon>
            Imprimer le reçu
          </button>
        </mat-card-actions>
      </mat-card>
    </div>
  `,
  styleUrls: ['./payment-receipt.component.scss']
})
export class PaymentReceiptComponent {
  private readonly router = inject(Router);
  
  receipt = input.required<PaymentReceipt>();

  get confirmationText(): string {
    const itemCount = this.receipt().totalItems;
    return itemCount === 1 
      ? 'Votre réservation a été confirmée'
      : 'Vos réservations ont été confirmées';
  }

  get itemsSectionTitle(): string {
    const itemCount = this.receipt().totalItems;
    return itemCount === 1 
      ? 'Réservation payée'
      : 'Réservations payées';
  }

  goToReservations(): void {
    this.router.navigate(['/reservations']);
  }

  goToPrestations(): void {
    this.router.navigate(['/prestations']);
  }

  printReceipt(): void {
    window.print();
  }
}