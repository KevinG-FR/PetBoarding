import { Address, User } from '../../auth/models/user.model';

export interface UpdateProfileRequestDto {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address?: Address;
}

export interface UpdateProfileResponseDto {
  user: User;
}
