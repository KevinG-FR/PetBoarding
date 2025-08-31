import { Injectable, computed, signal, inject } from '@angular/core';
import { Observable, BehaviorSubject, tap, catchError, of, map } from 'rxjs';
import { BasketApiService } from '../../../shared/services/basket-api.service';
import { Basket, BasketItem, BasketSummary, BasketStatus } from '../models/basket-item.model';
import { BasketResponse, AddItemToBasketRequest, UpdateBasketItemRequest } from '../contracts/basket.dto';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private readonly basketApiService = inject(BasketApiService);
  private readonly _basket = signal<Basket | null>(null);
  private readonly _loading = signal<boolean>(false);
  private readonly _error = signal<string | null>(null);

  readonly basket = this._basket.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();

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
      map(response => {
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
      catchError(error => {
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

    const request: AddItemToBasketRequest = { reservationId };

    return this.basketApiService.addItemToBasket(request).pipe(
      tap(() => {
        this._loading.set(false);
        this.loadBasket().subscribe();
      }),
      catchError(error => {
        this._error.set('Erreur lors de l\'ajout au panier');
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
      catchError(error => {
        this._error.set('Erreur lors de la mise à jour du panier');
        this._loading.set(false);
        console.error('Error updating basket item:', error);
        return of();
      })
    );
  }

  removeItemFromBasket(prestationId: string): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    return this.basketApiService.removeItemFromBasket(prestationId).pipe(
      tap(() => {
        this._loading.set(false);
        this.loadBasket().subscribe();
      }),
      catchError(error => {
        this._error.set('Erreur lors de la suppression de l\'élément');
        this._loading.set(false);
        console.error('Error removing item from basket:', error);
        return of();
      })
    );
  }

  clearBasket(): Observable<void> {
    this._loading.set(true);
    this._error.set(null);

    return this.basketApiService.clearBasket().pipe(
      tap(() => {
        this._basket.set(null);
        this._loading.set(false);
      }),
      catchError(error => {
        this._error.set('Erreur lors de la suppression du panier');
        this._loading.set(false);
        console.error('Error clearing basket:', error);
        return of();
      })
    );
  }

  getBasketItem(itemId: string): BasketItem | undefined {
    return this.items().find(item => item.id === itemId);
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
      items: response.items.map(item => ({
        id: item.id,
        reservationId: item.reservationId,
        serviceName: item.serviceName,
        reservationPrice: item.reservationPrice,
        addedAt: new Date(item.addedAt)
      }))
    };
  }

  private mapStringToBasketStatus(status: string): BasketStatus {
    switch (status) {
      case 'Active': return BasketStatus.Active;
      case 'PendingPayment': return BasketStatus.PendingPayment;
      case 'Completed': return BasketStatus.Completed;
      case 'Abandoned': return BasketStatus.Abandoned;
      default: return BasketStatus.Active;
    }
  }
}
