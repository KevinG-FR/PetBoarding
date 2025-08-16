import { PetType } from '../../pets/models/pet.model';

export interface Prestation {
  id: string;
  libelle: string;
  description: string;
  categorieAnimal: PetType;
  prix: number;
  duree: number; // en minutes
  disponible: boolean;
}

export interface PrestationFilters {
  categorieAnimal?: PetType;
  searchText?: string;
}
