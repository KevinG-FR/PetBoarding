export interface LoginResponseDto {
  success: boolean;
  message: string;
  token?: string;
  user?: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  emailConfirmed: boolean;
  phoneNumberConfirmed: boolean;
  profileType: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}
