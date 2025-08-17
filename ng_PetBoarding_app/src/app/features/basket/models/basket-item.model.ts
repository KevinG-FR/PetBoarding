import { Prestation } from '../../../shared/models/prestation.model';
import { PetType } from '../../pets/models/pet.model';

export interface BasketItem {
  id: string;
  prestation: Prestation;
  // Optionnel: animal associé à la prestation (id ou snapshot minimal)
  pet?: {
    id: string;
    name: string;
    type: PetType;
  };
  quantity: number;
  // Nouveau système de dates avec période
  dateDebut?: Date;
  dateFin?: Date;
  nombreJours?: number;
  // Ancienne propriété conservée pour rétrocompatibilité
  dateReservation?: Date;
  notes?: string;
}

export interface BasketSummary {
  totalItems: number;
  totalPrice: number;
  totalDuration: number; // en minutes
}
