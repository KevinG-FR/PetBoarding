import { CommonModule } from '@angular/common';
import {
  Component,
  computed,
  inject,
  input,
  OnInit,
  output,
  signal,
  ViewEncapsulation
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { CreneauDisponible, Prestation } from '../../models/prestation.model';
import { PlanningService } from '../../services/planning.service';

export interface DateSelectionResult {
  startDate: Date;
  endDate?: Date;
  isValid: boolean;
  status: 'valid' | 'incomplete' | 'error';
  selectedSlots: CreneauDisponible[];
  numberOfDays: number;
  totalPrice: number;
  errors?: {
    type: 'incomplete_period' | 'unavailable_dates';
    message: string;
    problematicDates?: Date[];
  };
}

@Component({
  selector: 'app-date-selection',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatSnackBarModule,
    MatChipsModule,
    MatBadgeModule,
    MatNativeDateModule
  ],
  templateUrl: './selection-dates.component.html',
  styleUrl: './selection-dates.component.scss',
  encapsulation: ViewEncapsulation.None
})
export class DateSelectionComponent implements OnInit {
  // Inputs
  prestation = input.required<Prestation>();

  // Outputs
  selectionChange = output<DateSelectionResult>();

  // Services
  private planningService = inject(PlanningService);

  // Component state
  startDate = signal<Date | null>(null);
  endDate = signal<Date | null>(null);
  isPeriodMode = signal(false);
  availableSlots = signal<CreneauDisponible[]>([]);
  allSlots = signal<CreneauDisponible[]>([]);
  isLoading = signal(false);

  // Properties for ngModel (required for datepicker)
  private _startDateValue: Date | null = null;
  private _endDateValue: Date | null = null;

  get startDateValue(): Date | null {
    return this._startDateValue;
  }
  set startDateValue(value: Date | null) {
    if (value && !this.isDateAvailable(value)) {
      return;
    }

    this._startDateValue = value;
    this.startDate.set(value);

    if (value && this.isPeriodMode() && this.endDate() && this.endDate()! < value) {
      this.endDate.set(null);
      this._endDateValue = null;
    }

    this.emitSelection();
  }

  get endDateValue(): Date | null {
    return this._endDateValue;
  }
  set endDateValue(value: Date | null) {
    if (value && !this.isDateAvailable(value)) {
      return;
    }

    this._endDateValue = value;
    this.endDate.set(value);
    this.emitSelection();
  }

  minDate = new Date();

  // Available slots filtered by mode and start date
  filteredAvailableSlots = computed(() => {
    const slots = this.availableSlots();
    const isInPeriodMode = this.isPeriodMode();
    const startDate = this.startDate();

    if (!isInPeriodMode) {
      return slots;
    }

    if (!startDate) {
      return slots;
    }

    // In period mode with start date:
    // Show continuous sequence of available dates after start date
    // until first full/unprogrammed date
    const allSlots = this.allSlots();
    const continuousSlots: CreneauDisponible[] = [];

    const currentDate = new Date(startDate);
    currentDate.setDate(currentDate.getDate() + 1);

    // Iterate day by day until finding a "gap"
    while (true) {
      const foundSlot = allSlots.find((slot: CreneauDisponible) => {
        const slotDate = new Date(slot.date);
        const targetDate = new Date(currentDate);
        return slotDate.toDateString() === targetDate.toDateString();
      });

      if (!foundSlot) {
        break;
      }

      if (foundSlot.capaciteDisponible === 0) {
        break;
      }

      continuousSlots.push(foundSlot);

      currentDate.setDate(currentDate.getDate() + 1);

      if (continuousSlots.length >= 30) {
        break;
      }
    }

    return continuousSlots;
  });

  selection = computed(() => {
    const start = this.startDate();
    const end = this.endDate();
    const prestation = this.prestation();

    if (!start) {
      return null;
    }

    const finalDate = this.isPeriodMode() ? end : null;
    const slots = this.getSlotsForPeriod(start, finalDate);
    const numberOfDays = this.calculateNumberOfDays(start, finalDate);
    const isValid = this.checkValidity(slots);
    const errors = this.analyzePeriodErrors(start, finalDate, slots);

    let status: 'valid' | 'incomplete' | 'error';
    if (isValid) {
      status = 'valid';
    } else if (errors?.type === 'incomplete_period') {
      status = 'incomplete';
    } else {
      status = 'error';
    }

    return {
      startDate: start,
      endDate: finalDate,
      isValid,
      status,
      selectedSlots: slots,
      numberOfDays,
      totalPrice: numberOfDays * prestation.prix,
      errors
    };
  });

  ngOnInit(): void {
    this.loadAvailableSlots();
  }

  private async loadAvailableSlots(): Promise<void> {
    this.isLoading.set(true);

    try {
      const planning = await this.planningService
        .getPlanningParPrestation(this.prestation().id)
        .toPromise();

      if (planning?.creneaux) {
        const allFutureSlots = planning.creneaux.filter(
          (slot: CreneauDisponible) => slot.date >= this.minDate
        );
        this.allSlots.set(allFutureSlots);

        const slotsWithAvailability = allFutureSlots.filter(
          (slot: CreneauDisponible) => slot.capaciteDisponible > 0
        );
        this.availableSlots.set(slotsWithAvailability);
      }
    } catch (_error) {
      // Error loading slots
    } finally {
      this.isLoading.set(false);
    }
  }

  private getSlotsForPeriod(start: Date, end: Date | null): CreneauDisponible[] {
    const allSlots = this.allSlots();
    const endDate = end || start;

    const periodSlots: CreneauDisponible[] = [];
    const currentDate = new Date(start);

    while (currentDate <= endDate) {
      const slot = allSlots.find((slot: CreneauDisponible) => {
        const slotDate = new Date(slot.date);
        const targetDate = new Date(currentDate);
        return slotDate.toDateString() === targetDate.toDateString();
      });

      if (slot) {
        periodSlots.push(slot);
      }

      currentDate.setDate(currentDate.getDate() + 1);
    }

    return periodSlots;
  }

  private calculateNumberOfDays(start: Date, end: Date | null): number {
    if (!end) return 1;
    return Math.max(1, Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1);
  }

  private checkValidity(slots: CreneauDisponible[]): boolean {
    const start = this.startDate();
    const end = this.endDate();

    if (!start) return false;

    if (this.isPeriodMode() && !end) {
      return false;
    }

    const endDate = this.isPeriodMode() ? end : null;
    const requiredDays = this.calculateNumberOfDays(start, endDate);

    return (
      slots.length === requiredDays &&
      slots.every((slot: CreneauDisponible) => slot.capaciteDisponible > 0)
    );
  }

  private analyzePeriodErrors(
    start: Date,
    end: Date | null,
    _slots: CreneauDisponible[]
  ):
    | {
        type: 'incomplete_period' | 'unavailable_dates';
        message: string;
        problematicDates?: Date[];
      }
    | undefined {
    if (this.isPeriodMode() && !end) {
      return {
        type: 'incomplete_period',
        message: 'Please select an end date for your period.'
      };
    }

    const endDate = this.isPeriodMode() ? end : null;

    const problematicDates: Date[] = [];
    const currentDate = new Date(start);
    const finalDate = endDate || start;

    while (currentDate <= finalDate) {
      const allSlots = this.allSlots();
      const slot = allSlots.find((slot: CreneauDisponible) => {
        const slotDate = new Date(slot.date);
        const targetDate = new Date(currentDate);
        return slotDate.toDateString() === targetDate.toDateString();
      });

      if (!slot || slot.capaciteDisponible === 0) {
        problematicDates.push(new Date(currentDate));
      }

      currentDate.setDate(currentDate.getDate() + 1);
    }

    if (problematicDates.length > 0) {
      const datesStr = problematicDates.map((d) => d.toLocaleDateString('en-US')).join(', ');

      return {
        type: 'unavailable_dates',
        message: `The following dates are not available: ${datesStr}`,
        problematicDates
      };
    }

    return undefined;
  }

  onStartDateChange(date: Date | null): void {
    // Use setter that includes validation
    this.startDateValue = date;
  }

  onEndDateChange(date: Date | null): void {
    // Use setter that includes validation
    this.endDateValue = date;
  }

  onModeChange(isPeriodMode: boolean): void {
    this.isPeriodMode.set(isPeriodMode);
    if (!isPeriodMode) {
      this.endDate.set(null);
    }
    this.emitSelection();
  }

  private emitSelection(): void {
    const selection = this.selection();
    if (selection) {
      this.selectionChange.emit({
        ...selection,
        endDate: selection.endDate || undefined
      });
    }
  }

  onDateClick(date: Date): void {
    if (!this.isDateAvailable(date)) {
      return;
    }

    if (!this.isPeriodMode()) {
      this.startDateValue = date;
      this.endDateValue = null;
    } else {
      const start = this.startDate();

      if (!start) {
        this.startDateValue = date;
      } else if (!this.endDate()) {
        if (date >= start) {
          this.endDateValue = date;
        } else {
          this.startDateValue = date;
          this.endDateValue = null;
        }
      } else {
        this.startDateValue = date;
        this.endDateValue = null;
      }
    }

    this.emitSelection();
  }

  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;

    if (date < this.minDate) return false;

    const allSlots = this.allSlots();
    const slotWithAvailability = allSlots.find((slot: CreneauDisponible) => {
      const slotDate = new Date(slot.date);
      const targetDate = new Date(date);
      return slotDate.toDateString() === targetDate.toDateString();
    });

    return slotWithAvailability ? slotWithAvailability.capaciteDisponible > 0 : false;
  };

  endDateFilter = (date: Date | null): boolean => {
    if (!date) return false;

    if (!this.dateFilter(date)) return false;

    const startDate = this.startDate();
    if (!startDate) return false;

    return date > startDate;
  };

  dateClass = (cellDate: Date, view: 'month' | 'year' | 'multi-year'): string => {
    if (view !== 'month') return '';

    const allSlots = this.allSlots();
    const slot = allSlots.find((slot: CreneauDisponible) => {
      const slotDate = new Date(slot.date);
      const targetDate = new Date(cellDate);
      return slotDate.toDateString() === targetDate.toDateString();
    });

    let className = '';
    if (!slot) {
      className = 'date-unavailable';
    } else if (slot.capaciteDisponible === 0) {
      className = 'date-full';
    } else if (slot.capaciteDisponible <= 2) {
      className = 'date-limited';
    } else {
      className = 'date-available';
    }

    return className;
  };

  isDateAvailable(date: Date): boolean {
    const allSlots = this.allSlots();
    const slot = allSlots.find((slot: CreneauDisponible) => {
      const slotDate = new Date(slot.date);
      const targetDate = new Date(date);
      return slotDate.toDateString() === targetDate.toDateString();
    });
    return slot ? slot.capaciteDisponible > 0 : false;
  }

  getAvailabilityInfo(date: Date): string {
    const allSlots = this.allSlots();
    const slot = allSlots.find((slot: CreneauDisponible) => slot.date.getTime() === date.getTime());

    if (!slot) return 'Not scheduled';

    if (slot.capaciteDisponible === 0) {
      return `Full (${slot.capaciteMax} spots)`;
    }

    return `${slot.capaciteDisponible}/${slot.capaciteMax} spots`;
  }

  isDateSelected(date: Date): boolean {
    const start = this.startDate();
    const end = this.endDate();

    if (!start) return false;

    if (!this.isPeriodMode()) {
      const startDate = new Date(start);
      const targetDate = new Date(date);
      return startDate.toDateString() === targetDate.toDateString();
    }

    if (!end) {
      const startDate = new Date(start);
      const targetDate = new Date(date);
      return startDate.toDateString() === targetDate.toDateString();
    }

    return date >= start && date <= end;
  }
}
