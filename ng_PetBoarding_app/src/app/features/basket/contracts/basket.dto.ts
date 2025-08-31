export interface BasketResponse {
  id: string;
  userId: string;
  status: string;
  totalAmount: number;
  totalItemCount: number;
  paymentFailureCount: number;
  paymentId?: string;
  createdAt: Date;
  updatedAt: Date;
  items: BasketItemResponse[];
}

export interface BasketItemResponse {
  id: string;
  reservationId: string;
  serviceName: string;
  reservationPrice: number;
  reservationDates: string;
}

export interface AddItemToBasketRequest {
  ReservationId: string;
}

export interface UpdateBasketItemRequest {
  quantity: number;
}