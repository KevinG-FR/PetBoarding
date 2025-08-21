import { PetType } from '../../pets/models/pet.model';
import { AvailableSlot } from './Slot';

export interface Prestation {
  id: string;
  libelle: string;
  description: string;
  categorieAnimal: PetType;
  prix: number;
  duree: number;
  disponible: boolean;
  planning?: PlanningPrestation;
}

export interface PrestationFilters {
  categorieAnimal?: PetType;
  searchText?: string;
}

export interface PlanningPrestation {
  id: string;
  prestationId: string;
  nom: string;
  description?: string;
  estActif: boolean;
  creneaux: AvailableSlot[];
  dateCreation: Date;
  dateModification?: Date;
}
