import { PetGender, PetType } from './pet.model';

export interface PetFormData {
  name: string;
  type: PetType;
  breed: string;
  age: number;
  weight?: number;
  color: string;
  gender: PetGender;
  isNeutered: boolean;
  microchipNumber?: string;
  medicalNotes?: string;
  specialNeeds?: string;
  photoUrl?: string;
  emergencyContact?: {
    name: string;
    phone: string;
    relationship: string;
  };
}

export interface PetFormErrors {
  name?: string;
  type?: string;
  breed?: string;
  age?: string;
  weight?: string;
  color?: string;
  gender?: string;
  microchipNumber?: string;
  medicalNotes?: string;
  specialNeeds?: string;
  emergencyContact?: {
    name?: string;
    phone?: string;
    relationship?: string;
  };
}
