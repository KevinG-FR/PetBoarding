export interface Vaccination {
  id: string;
  name: string;
  date: Date;
  expiryDate?: Date;
  veterinarian: string;
  batchNumber?: string;
}
