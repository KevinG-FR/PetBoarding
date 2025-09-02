import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, catchError, map, of, tap } from 'rxjs';
import { BasketApiService } from '../../../shared/services/basket-api.service';
import {
  AddItemToBasketRequest,
  BasketResponse,
  UpdateBasketItemRequest
} from '../contracts/basket.dto';
import { Basket, BasketItem, BasketStatus, BasketSummary } from '../models/basket-item.model';
import { PaymentMethod, PaymentReceipt, ReceiptItem } from '../models/payment-receipt.model';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private readonly basketApiService = inject(BasketApiService);
  private readonly _basket = signal<Basket | null>(null);
  private readonly _loading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);
  private readonly _paymentReceipt = signal<PaymentReceipt | null>(null);

  readonly basket = this._basket.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly paymentReceipt = this._paymentReceipt.asReadonly();

  readonly items = computed(() => this._basket()?.items || []);

  readonly summary = computed<BasketSummary>(() => {
    const basket = this._basket();
    if (!basket) {
      return { totalItems: 0, totalAmount: 0, isEmpty: true };
    }

    return {
      totalItems: basket.totalItemCount,
      totalAmount: basket.totalAmount,
      isEmpty: basket.items.length === 0
    };
  });

  readonly isEmpty = computed(() => this.summary().isEmpty);

  loadBasket(): Observable<Basket | null> {
    this._loading.set(true);
    this._error.set(null);

    return this.basketApiService.getMyBasket().pipe(
      map((response) => {
        if (response) {
          const basket = this.mapResponseToBasket(response);
          this._basket.set(basket);
          this._loading.set(false);
          return basket;
        } else {
          this._basket.set(null);
          this._loading.set(false);
          return null;
        }
      }),
      catchError((error) => {
        this._error.set('Erreur lors du chargement du panier');
        this._loading.set(false);
        console.error('Error loading basket:', error);
        return of(null);
      })
    );
  }

  addItemToBasket(reservationId: string): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    const request: AddItemToBasketRequest = { ReservationId: reservationId };

    return this.basketApiService.addItemToBasket(request).pipe(
      tap(() => {
        this._loading.set(false);
        this.loadBasket().subscribe();
      }),
      catchError((error) => {
        this._error.set("Erreur lors de l'ajout au panier");
        this._loading.set(false);
        console.error('Error adding item to basket:', error);
        return of();
      })
    );
  }

  updateBasketItem(prestationId: string, quantity: number): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    const request: UpdateBasketItemRequest = { quantity };

    return this.basketApiService.updateBasketItem(prestationId, request).pipe(
      tap(() => {
        this._loading.set(false);
        this.loadBasket().subscribe();
      }),
      catchError((error) => {
        this._error.set('Erreur lors de la mise à jour du panier');
        this._loading.set(false);
        console.error('Error updating basket item:', error);
        return of();
      })
    );
  }

  removeItemFromBasket(basketItemId: string): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    return this.basketApiService.removeItemFromBasket(basketItemId).pipe(
      tap(() => {
        this._loading.set(false);
        this.loadBasket().subscribe();
      }),
      catchError((error) => {
        this._error.set("Erreur lors de la suppression de l'élément");
        this._loading.set(false);
        console.error('Error removing item from basket:', error);
        return of();
      })
    );
  }

  clearBasket(): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    const currentBasket = this._basket();
    if (!currentBasket) {
      this._loading.set(false);
      return of();
    }

    // Si le panier est déjà annulé, on le vide juste côté frontend sans appeler l'API
    if (currentBasket.status === BasketStatus.Cancelled) {
      this._basket.set(null);
      this._loading.set(false);
      return of();
    }

    return this.basketApiService.clearBasket(currentBasket.id).pipe(
      tap(() => {
        this._basket.set(null);
        this._loading.set(false);
      }),
      catchError((error) => {
        // Si l'erreur indique que le panier est déjà annulé, on le vide côté frontend
        if (error?.error?.includes?.('Cannot clear basket with status Cancelled')) {
          this._basket.set(null);
          this._loading.set(false);
          return of();
        }

        this._error.set('Erreur lors de la suppression du panier');
        this._loading.set(false);
        console.error('Error clearing basket:', error);
        return of();
      })
    );
  }

  getBasketItem(itemId: string): BasketItem | undefined {
    return this.items().find((item) => item.id === itemId);
  }

  processPaymentSuccess(): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    const currentBasket = this._basket();
    if (!currentBasket) {
      this._loading.set(false);
      return of();
    }

    // Créer le reçu avant de traiter le paiement
    const receipt = this.createPaymentReceipt(currentBasket);

    return this.basketApiService.processPaymentSuccess(currentBasket.id).pipe(
      tap(() => {
        this._paymentReceipt.set(receipt);
        this._loading.set(false);
        this.loadBasket().subscribe();
      }),
      catchError((error) => {
        this._error.set('Erreur lors du traitement du paiement');
        this._loading.set(false);
        console.error('Error processing payment success:', error);
        return of();
      })
    );
  }

  processPaymentFailure(): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    const currentBasket = this._basket();
    if (!currentBasket) {
      this._loading.set(false);
      return of();
    }

    return this.basketApiService.processPaymentFailure(currentBasket.id).pipe(
      tap(() => {
        this._loading.set(false);
        this.loadBasket().subscribe();
      }),
      catchError((error) => {
        this._error.set('Erreur lors du traitement de l\'échec de paiement');
        this._loading.set(false);
        console.error('Error processing payment failure:', error);
        return of();
      })
    );
  }

  private mapResponseToBasket(response: BasketResponse): Basket {
    return {
      id: response.id,
      userId: response.userId,
      status: this.mapStringToBasketStatus(response.status),
      totalAmount: response.totalAmount,
      totalItemCount: response.totalItemCount,
      paymentFailureCount: response.paymentFailureCount,
      paymentId: response.paymentId,
      createdAt: new Date(response.createdAt),
      updatedAt: new Date(response.updatedAt),
      items: response.items.map((item) => ({
        id: item.id,
        reservationId: item.reservationId,
        serviceName: item.serviceName,
        reservationPrice: item.reservationPrice,
        reservationDates: item.reservationDates
      }))
    };
  }

  private mapStringToBasketStatus(status: string): BasketStatus {
    switch (status) {
      case 'Created':
        return BasketStatus.Created;
      case 'Cancelled':
        return BasketStatus.Cancelled;
      case 'PaymentFailure':
        return BasketStatus.PaymentFailure;
      case 'Paid':
        return BasketStatus.Paid;
      default:
        return BasketStatus.Created;
    }
  }

  private createPaymentReceipt(basket: Basket): PaymentReceipt {
    const receiptItems: ReceiptItem[] = basket.items.map(item => ({
      id: item.id,
      serviceName: item.serviceName,
      reservationPrice: item.reservationPrice,
      reservationDates: item.reservationDates
    }));

    return {
      basketId: basket.id,
      totalAmount: basket.totalAmount,
      totalItems: basket.totalItemCount,
      paymentMethod: PaymentMethod.Test, // Pour la simulation
      paymentId: basket.paymentId || `TEST-${Date.now()}`,
      items: receiptItems,
      paidAt: new Date()
    };
  }

  clearPaymentReceipt(): void {
    this._paymentReceipt.set(null);
  }
}
