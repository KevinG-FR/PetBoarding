export interface Prestation {
  id: string;
  libelle: string;
  description: string;
  categorieAnimal: CategorieAnimal;
  prix: number;
  duree: number; // en minutes
  disponible: boolean;
}

export enum CategorieAnimal {
  CHIEN = 'chien',
  CHAT = 'chat'
}

export interface PrestationFilters {
  categorieAnimal?: CategorieAnimal;
  searchText?: string;
}
