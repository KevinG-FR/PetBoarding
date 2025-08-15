import { Component } from '@angular/core';

@Component({
  selector: 'app-basket',
  standalone: true,
  imports: [],
  template: `
    <div class="basket-page">
      <h1>Page du panier</h1>
      <p>Le composant basket est maintenant configuré et prêt à être utilisé !</p>
    </div>
  `,
  styles: [
    `
      .basket-page {
        padding: 2rem;
        text-align: center;
      }
    `
  ]
})
export class BasketComponent {}
