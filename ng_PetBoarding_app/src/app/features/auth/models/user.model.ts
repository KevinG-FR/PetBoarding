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
  profileType: string;
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
