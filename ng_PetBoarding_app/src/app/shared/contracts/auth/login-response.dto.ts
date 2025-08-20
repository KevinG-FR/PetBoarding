export interface LoginResponseDto {
  success: boolean;
  message: string;
  token?: string;
  user?: UserDto;
}

export interface AddressDto {
  id?: string;
  streetNumber: string;
  streetName: string;
  city: string;
  postalCode: string;
  country: string;
  complement?: string;
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
  address?: AddressDto;
  createdAt: string;
  updatedAt: string;
}
