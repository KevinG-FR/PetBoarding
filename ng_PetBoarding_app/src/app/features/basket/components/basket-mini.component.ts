import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { Router } from '@angular/router';

import { BasketService } from '../services/basket.service';

@Component({
  selector: 'app-basket-mini',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatBadgeModule,
    MatMenuModule,
    MatDividerModule
  ],
  templateUrl: './basket-mini.component.html',
  styleUrls: ['./basket-mini.component.scss']
})
export class BasketMiniComponent {
  private router = inject(Router);
  basketService = inject(BasketService);

  goToBasket(): void {
    this.router.navigate(['/basket']);
  }
}
