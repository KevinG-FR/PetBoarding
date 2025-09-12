import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import {
  PrestationScheduleResponse,
  PrestationsService,
  ScheduleDay
} from '../../../features/prestations/services/prestations.service';

type ViewMode = 'monthly' | 'yearly';

@Component({
  selector: 'app-prestation-schedule',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTooltipModule,
    FormsModule
  ],
  templateUrl: './prestation-schedule.component.html',
  styleUrl: './prestation-schedule.component.scss'
})
export class PrestationScheduleComponent implements OnInit {
  private readonly prestationsService = inject(PrestationsService);
  private readonly snackBar = inject(MatSnackBar);

  @Input() prestationId: string = '';
  @Input() prestationName: string = '';

  // Signaux pour l'état du composant
  viewMode = signal<ViewMode>('monthly');
  selectedTabIndex = signal(0); // 0 pour mensuel, 1 pour annuel
  currentYear = signal(new Date().getFullYear());
  currentMonth = signal(new Date().getMonth() + 1);
  isLoading = signal(false);
  error = signal<string | null>(null);
  scheduleData = signal<PrestationScheduleResponse | null>(null);

  // Options pour les selects
  years = signal(this.generateYearOptions());
  months = signal([
    { value: 1, name: 'Janvier' },
    { value: 2, name: 'Février' },
    { value: 3, name: 'Mars' },
    { value: 4, name: 'Avril' },
    { value: 5, name: 'Mai' },
    { value: 6, name: 'Juin' },
    { value: 7, name: 'Juillet' },
    { value: 8, name: 'Août' },
    { value: 9, name: 'Septembre' },
    { value: 10, name: 'Octobre' },
    { value: 11, name: 'Novembre' },
    { value: 12, name: 'Décembre' }
  ]);

  // Computed pour organiser les données du calendrier mensuel
  monthlyCalendar = computed(() => {
    const data = this.scheduleData();
    if (!data || this.viewMode() !== 'monthly' || !data.month) return [];

    const year = data.year;
    const month = data.month;
    const firstDay = new Date(year, month - 1, 1);
    const lastDay = new Date(year, month, 0);
    const startDate = new Date(firstDay);
    startDate.setDate(startDate.getDate() - firstDay.getDay()); // Commencer par dimanche

    const calendar: CalendarDay[] = [];
    const currentDate = new Date(startDate);

    // Générer 6 semaines (42 jours)
    for (let i = 0; i < 42; i++) {
      const day = this.scheduleData()?.scheduleDays.find(
        (d) => new Date(d.date).toDateString() === currentDate.toDateString()
      );

      calendar.push({
        date: new Date(currentDate),
        isCurrentMonth: currentDate.getMonth() === month - 1,
        scheduleDay: day || null
      });

      currentDate.setDate(currentDate.getDate() + 1);
    }

    return this.chunkArray(calendar, 7); // Grouper par semaines
  });

  constructor() {
    // Effet pour recharger les données quand les paramètres changent
    effect(() => {
      if (this.prestationId) {
        this.loadSchedule();
      }
    });
  }

  ngOnInit(): void {
    if (this.prestationId) {
      this.loadSchedule();
    }
  }

  loadSchedule(): void {
    if (!this.prestationId) return;

    this.isLoading.set(true);
    this.error.set(null);

    const request = {
      prestationId: this.prestationId,
      year: this.currentYear(),
      month: this.viewMode() === 'monthly' ? this.currentMonth() : undefined
    };

    this.prestationsService.getPrestationSchedule(request).subscribe({
      next: (data) => {
        this.scheduleData.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erreur lors du chargement du planning:', error);
        this.error.set('Erreur lors du chargement du planning');
        this.isLoading.set(false);
        this.snackBar.open('Erreur lors du chargement du planning', 'Fermer', {
          duration: 5000
        });
      }
    });
  }

  onTabChange(index: number): void {
    this.selectedTabIndex.set(index);
    const mode: ViewMode = index === 0 ? 'monthly' : 'yearly';
    this.viewMode.set(mode);
    this.loadSchedule();
  }

  onViewModeChange(mode: ViewMode): void {
    this.viewMode.set(mode);
    this.selectedTabIndex.set(mode === 'monthly' ? 0 : 1);
    this.loadSchedule();
  }

  onYearChange(year: number): void {
    this.currentYear.set(year);
    this.loadSchedule();
  }

  onMonthChange(month: number): void {
    this.currentMonth.set(month);
    this.loadSchedule();
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'validated':
        return '#4caf50';
      case 'inprogress':
        return '#2196f3';
      case 'completed':
        return '#9c27b0';
      case 'cancelled':
      case 'cancelauto':
        return '#f44336';
      case 'created':
        return '#ff9800';
      default:
        return '#666666';
    }
  }

  getStatusLabel(status: string): string {
    switch (status.toLowerCase()) {
      case 'validated':
        return 'Validé';
      case 'inprogress':
        return 'En cours';
      case 'completed':
        return 'Terminé';
      case 'cancelled':
        return 'Annulé';
      case 'cancelauto':
        return 'Annulé auto';
      case 'created':
        return 'Créé';
      default:
        return status;
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit'
    });
  }

  formatPrice(price: number | undefined): string {
    return price ? `${price.toFixed(2)} €` : 'N/A';
  }

  private generateYearOptions(): number[] {
    const currentYear = new Date().getFullYear();
    const years = [];
    for (let i = currentYear - 2; i <= currentYear + 2; i++) {
      years.push(i);
    }
    return years;
  }

  private chunkArray<T>(array: T[], size: number): T[][] {
    const chunks: T[][] = [];
    for (let i = 0; i < array.length; i += size) {
      chunks.push(array.slice(i, i + size));
    }
    return chunks;
  }
}

interface CalendarDay {
  date: Date;
  isCurrentMonth: boolean;
  scheduleDay: ScheduleDay | null;
}
