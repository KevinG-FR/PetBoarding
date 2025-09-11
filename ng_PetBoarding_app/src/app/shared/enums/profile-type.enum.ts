export enum ProfileType {
  Administrator = 'Administrator',
  Employee = 'Employee', 
  Customer = 'Customer'
}

export const ProfileTypeMapping = {
  [ProfileType.Administrator]: 'ADMIN',
  [ProfileType.Employee]: 'EMPLOYEE', 
  [ProfileType.Customer]: 'CLIENT'
} as const;

export const BackendToFrontendProfileType = {
  'Administrator': ProfileType.Administrator,
  'Employee': ProfileType.Employee,
  'Customer': ProfileType.Customer
} as const;

// Mapping vers les valeurs numériques attendues par le backend
export const ProfileTypeToBackendValue = {
  [ProfileType.Administrator]: 1,
  [ProfileType.Employee]: 2,
  [ProfileType.Customer]: 3
} as const;

// Mapping inverse pour récupérer les valeurs depuis le backend
export const BackendValueToProfileType = {
  1: ProfileType.Administrator,
  2: ProfileType.Employee,
  3: ProfileType.Customer
} as const;

// Helper function pour convertir les valeurs du backend
export function mapBackendProfileType(backendValue: any): ProfileType {
  if (typeof backendValue === 'number' && backendValue in BackendValueToProfileType) {
    return BackendValueToProfileType[backendValue as keyof typeof BackendValueToProfileType];
  }
  return backendValue as ProfileType; // Fallback si déjà au bon format
}