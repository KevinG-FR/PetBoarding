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

export interface SelectionDatesResult {
  dateDebut: Date;
  dateFin?: Date;
  estValide: boolean;
  creneauxSelectionnes: CreneauDisponible[];
  nombreJours: number;
  prixTotal: number;
}

@Component({
  selector: 'app-selection-dates',
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
export class SelectionDatesComponent implements OnInit {
  // Entrées
  prestation = input.required<Prestation>();

  // Sorties
  selectionChange = output<SelectionDatesResult>();

  // Services
  private planningService = inject(PlanningService);

  // État du composant
  dateDebut = signal<Date | null>(null);
  dateFin = signal<Date | null>(null);
  estPeriode = signal(false);
  creneauxDisponibles = signal<CreneauDisponible[]>([]);
  isLoading = signal(false);

  // Propriétés pour ngModel (nécessaires pour le datepicker)
  private _dateDebutValue: Date | null = null;
  private _dateFinValue: Date | null = null;

  get dateDebutValue(): Date | null {
    return this._dateDebutValue;
  }
  set dateDebutValue(value: Date | null) {
    if (value && !this.estDateDisponible(value)) {
      // Date non disponible - ignorer la sélection
      return;
    }

    this._dateDebutValue = value;
    this.dateDebut.set(value);

    // Si on change la date de début et qu'on est en mode période,
    // réinitialiser la date de fin si elle est antérieure
    if (value && this.estPeriode() && this.dateFin() && this.dateFin()! < value) {
      this.dateFin.set(null);
      this._dateFinValue = null;
    }

    this.emettreSelection();
  }

  get dateFinValue(): Date | null {
    return this._dateFinValue;
  }
  set dateFinValue(value: Date | null) {
    if (value && !this.estDateDisponible(value)) {
      // Date non disponible - ignorer la sélection
      return;
    }

    this._dateFinValue = value;
    this.dateFin.set(value);
    this.emettreSelection();
  }

  // Date minimum (aujourd'hui)
  minDate = new Date();

  // Computed properties
  selection = computed(() => {
    const debut = this.dateDebut();
    const fin = this.dateFin();
    const prestation = this.prestation();

    if (!debut) {
      return null;
    }

    const dateFinal = this.estPeriode() ? fin : null;
    const creneaux = this.obtenirCreneauxPourPeriode(debut, dateFinal);
    const nombreJours = this.calculerNombreJours(debut, dateFinal);
    const estValide = this.verifierValidite(creneaux);

    return {
      dateDebut: debut,
      dateFin: dateFinal,
      estValide,
      creneauxSelectionnes: creneaux,
      nombreJours,
      prixTotal: nombreJours * prestation.prix
    };
  });

  ngOnInit(): void {
    this.chargerCreneauxDisponibles();
  }

  private async chargerCreneauxDisponibles(): Promise<void> {
    this.isLoading.set(true);

    try {
      const planning = await this.planningService
        .getPlanningParPrestation(this.prestation().id)
        .toPromise();

      if (planning?.creneaux) {
        // Filtrer les créneaux futurs uniquement
        const creneauxFuturs = planning.creneaux.filter(
          (c) => c.date >= this.minDate && c.capaciteDisponible > 0
        );
        this.creneauxDisponibles.set(creneauxFuturs);
      }
    } catch (_error) {
      // Erreur lors du chargement des créneaux
    } finally {
      this.isLoading.set(false);
    }
  }

  private obtenirCreneauxPourPeriode(debut: Date, fin: Date | null): CreneauDisponible[] {
    const creneaux = this.creneauxDisponibles();
    const dateFin = fin || debut;

    const creneauxPeriode: CreneauDisponible[] = [];
    const dateCourante = new Date(debut);

    while (dateCourante <= dateFin) {
      const creneau = creneaux.find((c) => c.date.getTime() === dateCourante.getTime());

      if (creneau) {
        creneauxPeriode.push(creneau);
      }

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    return creneauxPeriode;
  }

  private calculerNombreJours(debut: Date, fin: Date | null): number {
    if (!fin) return 1;
    return Math.max(1, Math.ceil((fin.getTime() - debut.getTime()) / (1000 * 60 * 60 * 24)) + 1);
  }

  private verifierValidite(creneaux: CreneauDisponible[]): boolean {
    const debut = this.dateDebut();
    const fin = this.dateFin();

    if (!debut) return false;

    const dateFin = this.estPeriode() ? fin : null;
    const nombreJoursRequis = this.calculerNombreJours(debut, dateFin);

    // Vérifier que tous les jours de la période ont des créneaux disponibles
    return creneaux.length === nombreJoursRequis && creneaux.every((c) => c.capaciteDisponible > 0);
  }

  onDateDebutChange(date: Date | null): void {
    this.dateDebut.set(date);

    // Si on change la date de début et qu'on est en mode période,
    // réinitialiser la date de fin si elle est antérieure
    if (date && this.estPeriode() && this.dateFin() && this.dateFin()! < date) {
      this.dateFin.set(null);
    }

    this.emettreSelection();
  }

  onDateFinChange(date: Date | null): void {
    this.dateFin.set(date);
    this.emettreSelection();
  }

  onModeChange(estPeriode: boolean): void {
    this.estPeriode.set(estPeriode);
    if (!estPeriode) {
      this.dateFin.set(null);
    }
    this.emettreSelection();
  }

  private emettreSelection(): void {
    const selection = this.selection();
    if (selection) {
      this.selectionChange.emit({
        ...selection,
        dateFin: selection.dateFin || undefined
      });
    }
  }

  onDateClick(date: Date): void {
    if (!this.estPeriode()) {
      // Mode date unique - sélectionner directement cette date
      this.dateDebut.set(date);
      this._dateDebutValue = date;
      this.dateFin.set(null);
      this._dateFinValue = null;
    } else {
      // Mode période - logique de sélection de période
      const debut = this.dateDebut();

      if (!debut) {
        // Pas de date de début - définir cette date comme début
        this.dateDebut.set(date);
        this._dateDebutValue = date;
      } else if (!this.dateFin()) {
        // Date de début existe mais pas de fin - définir la fin
        if (date >= debut) {
          this.dateFin.set(date);
          this._dateFinValue = date;
        } else {
          // La nouvelle date est antérieure - redéfinir le début
          this.dateDebut.set(date);
          this._dateDebutValue = date;
          this.dateFin.set(null);
          this._dateFinValue = null;
        }
      } else {
        // Les deux dates sont définies - recommencer avec cette date
        this.dateDebut.set(date);
        this._dateDebutValue = date;
        this.dateFin.set(null);
        this._dateFinValue = null;
      }
    }

    this.emettreSelection();
  }

  // Fonction pour le DatePicker - permettre seulement les dates programmées
  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;

    // Empêcher les dates passées
    if (date < this.minDate) return false;

    // Vérifier si la date a des créneaux programmés
    const creneaux = this.creneauxDisponibles();
    const aDesCreneaux = creneaux.some((c) => {
      const creneauDate = new Date(c.date);
      const targetDate = new Date(date);
      return creneauDate.toDateString() === targetDate.toDateString();
    });

    // Permettre seulement les dates qui ont des créneaux programmés
    return aDesCreneaux;
  };

  // Fonction pour obtenir la classe CSS d'une date dans le calendrier
  dateClass = (cellDate: Date, view: 'month' | 'year' | 'multi-year'): string => {
    if (view !== 'month') return '';

    const creneaux = this.creneauxDisponibles();
    const creneau = creneaux.find((c) => {
      const creneauDate = new Date(c.date);
      const targetDate = new Date(cellDate);
      return creneauDate.toDateString() === targetDate.toDateString();
    });

    let className = '';
    if (!creneau) {
      className = 'date-unavailable';
    } else if (creneau.capaciteDisponible === 0) {
      className = 'date-full';
    } else if (creneau.capaciteDisponible <= 2) {
      className = 'date-limited';
    } else {
      className = 'date-available';
    }

    // Debug logs
    console.log(
      `[DateClass] Date: ${cellDate.toDateString()}, Classe: "${className}", Créneau:`,
      creneau
    );

    return className;
  };

  estDateDisponible(date: Date): boolean {
    const creneaux = this.creneauxDisponibles();
    const creneau = creneaux.find((c) => c.date.getTime() === date.getTime());
    return creneau ? creneau.capaciteDisponible > 0 : false;
  }

  getDisponibiliteInfo(date: Date): string {
    const creneau = this.creneauxDisponibles().find((c) => c.date.getTime() === date.getTime());

    if (!creneau) return 'Non disponible';

    return `${creneau.capaciteDisponible}/${creneau.capaciteMax} places`;
  }

  estDateSelectionnee(date: Date): boolean {
    const debut = this.dateDebut();
    const fin = this.dateFin();

    if (!debut) return false;

    if (!this.estPeriode()) {
      return debut.getTime() === date.getTime();
    }

    if (!fin) return debut.getTime() === date.getTime();

    return date >= debut && date <= fin;
  }

  // Méthode de debug pour vérifier les créneaux
  debugCreneaux(): void {
    const creneaux = this.creneauxDisponibles();
    console.log('=== DEBUG CRÉNEAUX ===');
    console.log('Nombre de créneaux:', creneaux.length);

    // Debug spécial pour le 30 août 2025
    const date30Aout = new Date(2025, 7, 30); // mois 7 = août (0-indexé)
    console.log('=== DEBUG 30 AOÛT 2025 ===');
    console.log('Date créée:', date30Aout.toDateString());
    console.log('Jour de la semaine:', date30Aout.getDay(), '(6=samedi)');

    const creneaux30Aout = creneaux.filter((c) => {
      const creneauDate = new Date(c.date);
      return creneauDate.toDateString() === date30Aout.toDateString();
    });
    console.log('Créneaux pour le 30 août:', creneaux30Aout);

    // Test avec le service planning directement
    this.planningService.debugDate(date30Aout);

    // Analyser les différents types de disponibilité
    const stats = {
      disponibles: 0,
      limites: 0,
      complets: 0,
      total: creneaux.length
    };

    creneaux.forEach((creneau) => {
      if (creneau.capaciteDisponible === 0) {
        stats.complets++;
      } else if (creneau.capaciteDisponible <= 2) {
        stats.limites++;
      } else {
        stats.disponibles++;
      }
    });

    console.log('Statistiques:', stats);
    console.log('Premiers créneaux:', creneaux.slice(0, 10));

    // Test de la fonction dateClass avec les prochains jours
    const today = new Date();
    for (let i = 0; i < 10; i++) {
      const testDate = new Date(today);
      testDate.setDate(today.getDate() + i);
      const className = this.dateClass(testDate, 'month');
      const isSelectable = this.dateFilter(testDate);
      console.log(
        `J+${i} (${testDate.toLocaleDateString()}): classe="${className}", sélectionnable=${isSelectable}`
      );
    }

    // Forcer un rechargement des créneaux
    this.chargerCreneauxDisponibles();
  }
}
