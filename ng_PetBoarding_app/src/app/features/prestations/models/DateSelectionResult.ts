import { AvailableSlot } from './Slot';

export interface DateSelectionResult {
  startDate: Date;
  endDate?: Date;
  isValid: boolean;
  status: 'valid' | 'incomplete' | 'error';
  selectedSlots: AvailableSlot[];
  numberOfDays: number;
  totalPrice: number;
  errors?: {
    type: 'incomplete_period' | 'unavailable_dates';
    message: string;
    problematicDates?: Date[];
  };
}
