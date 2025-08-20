import { Address } from '../../auth/models/user.model';

/**
 * DTO pour la mise à jour du profil utilisateur
 */
export interface UpdateProfileRequestDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address?: Address;
}

/**
 * DTO pour la réponse de mise à jour du profil
 */
export interface UpdateProfileResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address?: Address;
  createdAt: string;
  updatedAt: string;
  isActive: boolean;
}
