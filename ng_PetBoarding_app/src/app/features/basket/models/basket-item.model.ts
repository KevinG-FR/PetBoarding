import { Prestation } from '../../../shared/models/prestation.model';

export interface BasketItem {
  id: string;
  prestation: Prestation;
  // Optionnel: animal associé à la prestation (id ou snapshot minimal)
  pet?: {
    id: string;
    name: string;
    type: string;
  };
  quantity: number;
  dateReservation?: Date;
  notes?: string;
}

export interface BasketSummary {
  totalItems: number;
  totalPrice: number;
  totalDuration: number; // en minutes
}
