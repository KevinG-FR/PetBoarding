export interface Address {
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phone: string;
  address?: Address;
  dateOfBirth?: Date;
  createdAt: Date;
  updatedAt: Date;
  isActive: boolean;
}
