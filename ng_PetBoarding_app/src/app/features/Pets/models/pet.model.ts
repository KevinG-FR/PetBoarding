export interface Pet {
  id: string;
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
  vaccinations: Vaccination[];
  emergencyContact?: EmergencyContact;
  createdAt: Date;
  updatedAt: Date;
}

export interface Vaccination {
  id: string;
  name: string;
  date: Date;
  expiryDate?: Date;
  veterinarian: string;
  batchNumber?: string;
}

export interface EmergencyContact {
  name: string;
  phone: string;
  relationship: string;
}

export enum PetType {
  DOG = 'dog',
  CAT = 'cat',
  BIRD = 'bird',
  RABBIT = 'rabbit',
  HAMSTER = 'hamster'
}

export enum PetGender {
  MALE = 'male',
  FEMALE = 'female'
}

export const PetTypeLabels: Record<PetType, string> = {
  [PetType.DOG]: 'Chien',
  [PetType.CAT]: 'Chat',
  [PetType.BIRD]: 'Oiseau',
  [PetType.RABBIT]: 'Lapin',
  [PetType.HAMSTER]: 'Hamster'
};

export const PetGenderLabels: Record<PetGender, string> = {
  [PetGender.MALE]: 'Mâle',
  [PetGender.FEMALE]: 'Femelle'
};
