import { UserDto } from 'src/app/shared/contracts/auth/login-response.dto';
import { Address, User } from '../../auth/models/user.model';

export interface UpdateProfileRequestDto {
  firstname: string;
  lastname: string;
  email: string;
  phoneNumber: string;
  address?: Address;
}

export interface UpdateProfileResponseDto {
  user: User;
}

export interface GetProfileResponseDto {
  user: UserDto;
}
