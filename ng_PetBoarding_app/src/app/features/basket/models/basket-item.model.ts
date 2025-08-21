import { Prestation } from '../../../shared/models/prestation.model';
import { PetType } from '../../pets/models/pet.model';

export interface BasketItem {
  id: string;
  prestation: Prestation;
  pet?: {
    id: string;
    name: string;
    type: PetType;
  };
  quantity: number;
  dateDebut?: Date;
  dateFin?: Date;
  nombreJours?: number;
  dateReservation?: Date;
  notes?: string;
}

export interface BasketSummary {
  totalItems: number;
  totalPrice: number;
  totalDuration: number;
}
