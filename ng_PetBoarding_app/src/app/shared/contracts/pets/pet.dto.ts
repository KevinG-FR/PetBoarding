export interface PetDto {
  id: string;
  name: string;
  type: number; // Backend utilise des integers pour PetType
  breed: string;
  age: number;
  weight?: number;
  color: string;
  gender: number; // Backend utilise des integers pour PetGender
  isNeutered: boolean;
  microchipNumber?: string;
  medicalNotes?: string;
  specialNeeds?: string;
  photoUrl?: string;
  ownerId: string;
  ownerName?: string;
  emergencyContact?: EmergencyContactDto;
  createdAt: string;
  updatedAt: string;
}

export interface EmergencyContactDto {
  name: string;
  phone: string;
  relationship: string;
}

export interface CreatePetRequest {
  name: string;
  type: number;
  breed: string;
  age: number;
  weight?: number;
  color: string;
  gender: number;
  isNeutered: boolean;
  microchipNumber?: string;
  medicalNotes?: string;
  specialNeeds?: string;
  photoUrl?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  emergencyContactRelationship?: string;
}

export interface UpdatePetRequest {
  name: string;
  type: number;
  breed: string;
  age: number;
  weight?: number;
  color: string;
  gender: number;
  isNeutered: boolean;
  microchipNumber?: string;
  medicalNotes?: string;
  specialNeeds?: string;
  photoUrl?: string;
  emergencyContactName?: string;
  emergencyContactPhone?: string;
  emergencyContactRelationship?: string;
}

export interface GetAllPetsResponse {
  pets: PetDto[];
  count: number;
  success: boolean;
  message: string;
}

export interface GetPetResponse {
  pet: PetDto;
  success: boolean;
  message: string;
}

export interface CreatePetResponse {
  pet: PetDto;
  success: boolean;
  message: string;
  location?: string;
}

export interface UpdatePetResponse {
  pet: PetDto;
  success: boolean;
  message: string;
}

export interface DeletePetResponse {
  success: boolean;
  message: string;
  petId?: string;
}