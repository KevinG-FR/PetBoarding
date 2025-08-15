import { Prestation } from '../../../shared/models/prestation.model';

export interface BasketItem {
  id: string;
  prestation: Prestation;
  quantity: number;
  dateReservation?: Date;
  notes?: string;
}

export interface BasketSummary {
  totalItems: number;
  totalPrice: number;
  totalDuration: number; // en minutes
}
