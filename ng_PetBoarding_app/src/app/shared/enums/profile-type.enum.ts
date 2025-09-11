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