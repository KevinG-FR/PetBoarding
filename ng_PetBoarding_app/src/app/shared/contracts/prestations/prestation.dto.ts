import { PetType } from '../../../features/pets/models/pet.model';

export interface PrestationDto {
  id: string;
  libelle: string;
  description: string;
  categorieAnimal: PetType;
  prix: number;
  dureeEnMinutes: number;
  estDisponible: boolean;
  dateCreation: string;
  dateModification?: string;
}

export interface CreatePrestationRequest {
  libelle: string;
  description: string;
  categorieAnimal: PetType;
  prix: number;
  dureeEnMinutes: number;
  estDisponible: boolean;
}

export interface UpdatePrestationRequest {
  libelle?: string;
  description?: string;
  categorieAnimal?: PetType;
  prix?: number;
  dureeEnMinutes?: number;
  estDisponible?: boolean;
}

export interface GetAllPrestationsResponse {
  prestations: PrestationDto[];
  totalCount: number;
}

export interface GetPrestationResponse {
  prestation: PrestationDto;
}

export interface CreatePrestationResponse {
  prestation: PrestationDto;
}

export interface UpdatePrestationResponse {
  prestation: PrestationDto;
}

export interface DeletePrestationResponse {
  success: boolean;
  message: string;
}
