import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { PrestationsService } from '../../prestations/services/prestations.service';
import { BasketItem } from '../models/basket-item.model';

@Component({
  selector: 'app-basket-item',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    FormsModule
  ],
  templateUrl: './basket-item.component.html',
  styleUrls: ['./basket-item.component.scss']
})
export class BasketItemComponent {
  item = input.required<BasketItem>();

  quantityChange = output<{ itemId: string; quantity: number }>();
  remove = output<string>();
  prestationClick = output<string>();

  private readonly prestationsService = inject(PrestationsService);

  petCategoryInfo = computed(() => {
    const pet = this.item().pet;
    return pet ? this.prestationsService.getCategoryInfo(pet.type) : null;
  });

  onQuantityChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    const newQuantity = parseInt(target.value, 10);

    if (newQuantity > 0) {
      this.quantityChange.emit({
        itemId: this.item().id,
        quantity: newQuantity
      });
    }
  }

  increaseQuantity(): void {
    this.quantityChange.emit({
      itemId: this.item().id,
      quantity: this.item().quantity + 1
    });
  }

  decreaseQuantity(): void {
    if (this.item().quantity > 1) {
      this.quantityChange.emit({
        itemId: this.item().id,
        quantity: this.item().quantity - 1
      });
    }
  }

  onRemove(): void {
    this.remove.emit(this.item().id);
  }

  onPrestationClick(): void {
    this.prestationClick.emit(this.item().prestation.id);
  }

  getTotalPrice(): number {
    return this.item().prestation.prix * this.item().quantity;
  }

  getCategoryLabel(category: string): string {
    switch (category) {
      case 'chien':
        return 'Chien';
      case 'chat':
        return 'Chat';
      default:
        return category;
    }
  }

  formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes} min`;
    }
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return remainingMinutes > 0 ? `${hours}h ${remainingMinutes}min` : `${hours}h`;
  }

  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('fr-FR', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    }).format(date);
  }
}
