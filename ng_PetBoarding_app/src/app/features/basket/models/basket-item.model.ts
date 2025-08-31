export interface BasketItem {
  id: string;
  reservationId: string;
  serviceName: string;
  reservationPrice: number;
  reservationDates: string;
}

export interface Basket {
  id: string;
  userId: string;
  status: BasketStatus;
  totalAmount: number;
  totalItemCount: number;
  paymentFailureCount: number;
  paymentId?: string;
  createdAt: Date;
  updatedAt: Date;
  items: BasketItem[];
}

export interface BasketSummary {
  totalItems: number;
  totalAmount: number;
  isEmpty: boolean;
}

export enum BasketStatus {
  Active = 'Active',
  PendingPayment = 'PendingPayment',
  Completed = 'Completed',
  Abandoned = 'Abandoned'
}
