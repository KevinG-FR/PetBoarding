export interface RegisterRequestDto {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phone: string;
  acceptTerms: boolean;
  acceptNewsletter?: boolean;
}
