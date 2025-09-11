import { ProfileType } from '../../../shared/enums/profile-type.enum';

export interface Address {
  id?: string;
  streetNumber: string;
  streetName: string;
  city: string;
  postalCode: string;
  country: string;
  complement?: string;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  profileType: ProfileType;
  status: string;
  address?: {
    streetNumber: string;
    streetName: string;
    city: string;
    postalCode: string;
    country: string;
    complement?: string;
  };
  createdAt: Date;
  updatedAt: Date;
}
