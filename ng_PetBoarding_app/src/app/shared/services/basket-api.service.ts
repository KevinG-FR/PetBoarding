import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AddItemToBasketRequest,
  BasketResponse,
  UpdateBasketItemRequest
} from '../../features/basket/contracts/basket.dto';

@Injectable({
  providedIn: 'root'
})
export class BasketApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/baskets`;

  getMyBasket(): Observable<BasketResponse | null> {
    return this.http.get<BasketResponse>(`${this.baseUrl}/my-basket`);
  }

  addItemToBasket(request: AddItemToBasketRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/items`, request);
  }

  updateBasketItem(prestationId: string, request: UpdateBasketItemRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/items/${prestationId}`, request);
  }

  removeItemFromBasket(prestationId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/items/${prestationId}`);
  }

  clearBasket(basketId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${basketId}/clear`);
  }

  processPaymentSuccess(basketId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${basketId}/payment/success`, {});
  }

  processPaymentFailure(basketId: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${basketId}/payment/failure`, {});
  }
}
