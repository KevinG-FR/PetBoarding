import { Injectable, computed, signal } from '@angular/core';
import { Prestation } from '../../../shared/models/prestation.model';
import { PetType } from '../../pets/models/pet.model';
import { BasketItem, BasketSummary } from '../models/basket-item.model';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private _items = signal<BasketItem[]>([]);

  // Signals publics en lecture seule
  readonly items = this._items.asReadonly();

  readonly summary = computed<BasketSummary>(() => {
    const items = this._items();
    return {
      totalItems: items.reduce((sum, item) => sum + item.quantity, 0),
      totalPrice: items.reduce((sum, item) => sum + item.prestation.prix * item.quantity, 0),
      totalDuration: items.reduce((sum, item) => sum + item.prestation.duree * item.quantity, 0)
    };
  });

  readonly isEmpty = computed(() => this._items().length === 0);

  addItem(
    prestation: Prestation,
    quantity: number = 1,
    dateReservation?: Date,
    notes?: string,
    pet?: { id: string; name: string; type: PetType }
  ): void {
    const currentItems = this._items();
    const existingItemIndex = currentItems.findIndex(
      (item) => item.prestation.id === prestation.id
    );

    if (existingItemIndex >= 0) {
      // Mise à jour de la quantité si l'item existe déjà
      const updatedItems = [...currentItems];
      updatedItems[existingItemIndex] = {
        ...updatedItems[existingItemIndex],
        quantity: updatedItems[existingItemIndex].quantity + quantity,
        dateReservation: dateReservation || updatedItems[existingItemIndex].dateReservation,
        notes: notes || updatedItems[existingItemIndex].notes
        // ne remplace pas le pet existant si déjà présent
      };
      this._items.set(updatedItems);
    } else {
      // Ajout d'un nouvel item
      const newItem: BasketItem = {
        id: this.generateId(),
        prestation,
        pet,
        quantity,
        dateReservation,
        notes
      };
      this._items.set([...currentItems, newItem]);
    }
  }

  removeItem(itemId: string): void {
    const currentItems = this._items();
    this._items.set(currentItems.filter((item) => item.id !== itemId));
  }

  updateQuantity(itemId: string, quantity: number): void {
    if (quantity <= 0) {
      this.removeItem(itemId);
      return;
    }

    const currentItems = this._items();
    const itemIndex = currentItems.findIndex((item) => item.id === itemId);

    if (itemIndex >= 0) {
      const updatedItems = [...currentItems];
      updatedItems[itemIndex] = {
        ...updatedItems[itemIndex],
        quantity
      };
      this._items.set(updatedItems);
    }
  }

  updateItem(itemId: string, updates: Partial<Omit<BasketItem, 'id' | 'prestation'>>): void {
    const currentItems = this._items();
    const itemIndex = currentItems.findIndex((item) => item.id === itemId);

    if (itemIndex >= 0) {
      const updatedItems = [...currentItems];
      updatedItems[itemIndex] = {
        ...updatedItems[itemIndex],
        ...updates
      };
      this._items.set(updatedItems);
    }
  }

  clear(): void {
    this._items.set([]);
  }

  getItem(itemId: string): BasketItem | undefined {
    return this._items().find((item) => item.id === itemId);
  }

  private generateId(): string {
    return `basket-item-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }
}
