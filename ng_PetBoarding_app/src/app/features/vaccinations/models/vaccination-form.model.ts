export interface VaccinationFormData {
  name: string;
  date: Date;
  expiryDate?: Date;
  veterinarian: string;
  batchNumber?: string;
}

export interface VaccinationFormErrors {
  name?: string;
  date?: string;
  expiryDate?: string;
  veterinarian?: string;
  batchNumber?: string;
}
