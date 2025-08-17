import { PetType } from '../../pets/models/pet.model';

export interface Prestation {
  id: string;
  libelle: string;
  description: string;
  categorieAnimal: PetType;
  prix: number;
  duree: number; // en minutes
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
  creneaux: CreneauDisponible[];
  dateCreation: Date;
  dateModification?: Date;
}

export interface CreneauDisponible {
  date: Date;
  capaciteMax: number;
  capaciteReservee: number;
  capaciteDisponible: number;
}

export interface DisponibiliteQuery {
  prestationId: string;
  dateDebut: Date;
  dateFin?: Date;
  quantite?: number;
}

export interface DisponibiliteResponse {
  prestationId: string;
  estDisponible: boolean;
  creneauxDisponibles: CreneauDisponible[];
  message?: string;
}
