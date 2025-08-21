import { Address } from '../../auth/models/user.model';

export interface UpdateProfileRequestDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address?: Address;
}

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
