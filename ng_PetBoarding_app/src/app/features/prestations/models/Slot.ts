export interface AvailableSlot {
  date: Date;
  capaciteMax: number;
  capaciteReservee: number;
  capaciteDisponible: number;
}

export interface DisponibiliteQuery {
  prestationId: string;
  startDate: Date;
  endDate?: Date;
  quantity?: number;
}

export interface DisponibiliteResponse {
  prestationId: string;
  isAvailable: boolean;
  availableSlots: AvailableSlot[];
  message?: string;
}
