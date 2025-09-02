export interface PaymentReceipt {
  basketId: string;
  totalAmount: number;
  totalItems: number;
  paymentMethod: PaymentMethod;
  paymentId?: string;
  items: ReceiptItem[];
  paidAt: Date;
}

export interface ReceiptItem {
  id: string;
  serviceName: string;
  reservationPrice: number;
  reservationDates: string;
}

export enum PaymentMethod {
  CreditCard = 'Carte de crédit',
  PayPal = 'PayPal',
  BankTransfer = 'Virement bancaire',
  Test = 'Paiement de test'
}