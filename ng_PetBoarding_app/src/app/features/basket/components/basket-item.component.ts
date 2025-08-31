import { CommonModule } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { BasketItem } from '../models/basket-item.model';

@Component({
  selector: 'app-basket-item',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './basket-item.component.html',
  styleUrls: ['./basket-item.component.scss']
})
export class BasketItemComponent {
  item = input.required<BasketItem>();

  remove = output<string>();
  prestationClick = output<string>();

  onRemove(): void {
    this.remove.emit(this.item().id);
  }

  onPrestationClick(): void {
    this.prestationClick.emit(this.item().reservationId);
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
