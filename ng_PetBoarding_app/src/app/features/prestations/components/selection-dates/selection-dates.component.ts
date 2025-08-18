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
  statut: 'valide' | 'incomplete' | 'erreur';
  creneauxSelectionnes: CreneauDisponible[];
  nombreJours: number;
  prixTotal: number;
  erreurs?: {
    type: 'periode_incomplete' | 'dates_indisponibles';
    message: string;
    datesProblematiques?: Date[];
  };
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
  tousLesCreneaux = signal<CreneauDisponible[]>([]); // Tous les créneaux programmés (même complets)
  isLoading = signal(false);

  // Propriétés pour ngModel (nécessaires pour le datepicker)
  private _dateDebutValue: Date | null = null;
  private _dateFinValue: Date | null = null;

  get dateDebutValue(): Date | null {
    return this._dateDebutValue;
  }
  set dateDebutValue(value: Date | null) {
    if (value && !this.estDateDisponible(value)) {
      // Date non disponible - ignorer la sélection et afficher un message
      console.warn('Date non disponible sélectionnée:', value);
      // Optionnel: Afficher un snackbar ou une notification à l'utilisateur
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
      // Date non disponible - ignorer la sélection et afficher un message
      console.warn('Date non disponible sélectionnée:', value);
      // Optionnel: Afficher un snackbar ou une notification à l'utilisateur
      return;
    }

    this._dateFinValue = value;
    this.dateFin.set(value);
    this.emettreSelection();
  }

  // Date minimum (aujourd'hui)
  minDate = new Date();

  // Computed properties
  // Créneaux disponibles filtrés selon le mode et la date de début
  creneauxDisponiblesFiltres = computed(() => {
    const creneaux = this.creneauxDisponibles();
    const estEnModePeriode = this.estPeriode();
    const dateDebut = this.dateDebut();

    // En mode date unique, afficher tous les créneaux disponibles
    if (!estEnModePeriode) {
      return creneaux;
    }

    // En mode période sans date de début, afficher tous les créneaux
    if (!dateDebut) {
      return creneaux;
    }

    // En mode période avec date de début :
    // Afficher la séquence continue de dates disponibles après la date de début
    // jusqu'à la première date complète/non programmée
    const tousLesCreneaux = this.tousLesCreneaux();
    const creneauxContinus: CreneauDisponible[] = [];

    // Créer une date de travail à partir du jour suivant la date de début
    const dateCourante = new Date(dateDebut);
    dateCourante.setDate(dateCourante.getDate() + 1);

    // Parcourir jour par jour jusqu'à trouver un "trou"
    while (true) {
      // Chercher si cette date a un créneau programmé
      const creneauTrouve = tousLesCreneaux.find((c) => {
        const creneauDate = new Date(c.date);
        const targetDate = new Date(dateCourante);
        return creneauDate.toDateString() === targetDate.toDateString();
      });

      // Si pas de créneau programmé pour cette date = trou -> arrêter
      if (!creneauTrouve) {
        break;
      }

      // Si créneau complet (pas de disponibilité) = trou -> arrêter
      if (creneauTrouve.capaciteDisponible === 0) {
        break;
      }

      // Date valide, l'ajouter à la liste continue
      creneauxContinus.push(creneauTrouve);

      // Passer au jour suivant
      dateCourante.setDate(dateCourante.getDate() + 1);

      // Sécurité : limiter à un nombre raisonnable de jours pour éviter les boucles infinies
      if (creneauxContinus.length >= 30) {
        break;
      }
    }

    return creneauxContinus;
  });

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
    const erreurs = this.analyserErreursPeriode(debut, dateFinal, creneaux);

    // Déterminer le statut selon le contexte
    let statut: 'valide' | 'incomplete' | 'erreur';
    if (estValide) {
      statut = 'valide';
    } else if (erreurs?.type === 'periode_incomplete') {
      statut = 'incomplete'; // État informatif, pas d'erreur
    } else {
      statut = 'erreur'; // Vraie erreur (dates indisponibles, etc.)
    }

    return {
      dateDebut: debut,
      dateFin: dateFinal,
      estValide,
      statut,
      creneauxSelectionnes: creneaux,
      nombreJours,
      prixTotal: nombreJours * prestation.prix,
      erreurs
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
        // Tous les créneaux futurs programmés (même complets)
        const tousCreneauxFuturs = planning.creneaux.filter((c) => c.date >= this.minDate);
        this.tousLesCreneaux.set(tousCreneauxFuturs);

        // Créneaux avec disponibilité uniquement
        const creneauxAvecDisponibilite = tousCreneauxFuturs.filter(
          (c) => c.capaciteDisponible > 0
        );
        this.creneauxDisponibles.set(creneauxAvecDisponibilite);
      }
    } catch (_error) {
      // Erreur lors du chargement des créneaux
    } finally {
      this.isLoading.set(false);
    }
  }

  private obtenirCreneauxPourPeriode(debut: Date, fin: Date | null): CreneauDisponible[] {
    const tousCreneaux = this.tousLesCreneaux(); // Utiliser tous les créneaux, pas seulement les disponibles
    const dateFin = fin || debut;

    const creneauxPeriode: CreneauDisponible[] = [];
    const dateCourante = new Date(debut);

    while (dateCourante <= dateFin) {
      const creneau = tousCreneaux.find((c) => {
        const creneauDate = new Date(c.date);
        const targetDate = new Date(dateCourante);
        return creneauDate.toDateString() === targetDate.toDateString();
      });

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

    // En mode période, il faut OBLIGATOIREMENT une date de début ET une date de fin
    if (this.estPeriode() && !fin) {
      return false;
    }

    const dateFin = this.estPeriode() ? fin : null;
    const nombreJoursRequis = this.calculerNombreJours(debut, dateFin);

    // Vérifier que tous les jours de la période ont des créneaux disponibles
    return creneaux.length === nombreJoursRequis && creneaux.every((c) => c.capaciteDisponible > 0);
  }

  private analyserErreursPeriode(
    debut: Date,
    fin: Date | null,
    _creneaux: CreneauDisponible[]
  ):
    | {
        type: 'periode_incomplete' | 'dates_indisponibles';
        message: string;
        datesProblematiques?: Date[];
      }
    | undefined {
    // En mode période, vérifier que la date de fin est sélectionnée
    if (this.estPeriode() && !fin) {
      return {
        type: 'periode_incomplete',
        message: 'Veuillez sélectionner une date de fin pour votre période.'
      };
    }

    const dateFin = this.estPeriode() ? fin : null;

    // Analyser les dates problématiques
    const datesProblematiques: Date[] = [];
    const dateCourante = new Date(debut);
    const dateFinale = dateFin || debut;

    while (dateCourante <= dateFinale) {
      const tousCreneaux = this.tousLesCreneaux();
      const creneau = tousCreneaux.find((c) => {
        const creneauDate = new Date(c.date);
        const targetDate = new Date(dateCourante);
        return creneauDate.toDateString() === targetDate.toDateString();
      });

      if (!creneau || creneau.capaciteDisponible === 0) {
        datesProblematiques.push(new Date(dateCourante));
      }

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    if (datesProblematiques.length > 0) {
      const datesStr = datesProblematiques.map((d) => d.toLocaleDateString('fr-FR')).join(', ');

      return {
        type: 'dates_indisponibles',
        message: `Les dates suivantes ne sont pas disponibles : ${datesStr}`,
        datesProblematiques
      };
    }

    return undefined;
  }

  onDateDebutChange(date: Date | null): void {
    // Utiliser le setter qui inclut la validation
    this.dateDebutValue = date;
  }

  onDateFinChange(date: Date | null): void {
    // Utiliser le setter qui inclut la validation
    this.dateFinValue = date;
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
    // Vérifier d'abord que la date est disponible
    if (!this.estDateDisponible(date)) {
      console.warn("Tentative de sélection d'une date non disponible:", date);
      return;
    }

    if (!this.estPeriode()) {
      // Mode date unique - utiliser les setters qui incluent la validation
      this.dateDebutValue = date;
      this.dateFinValue = null;
    } else {
      // Mode période - logique de sélection de période
      const debut = this.dateDebut();

      if (!debut) {
        // Pas de date de début - définir cette date comme début
        this.dateDebutValue = date;
      } else if (!this.dateFin()) {
        // Date de début existe mais pas de fin - définir la fin
        if (date >= debut) {
          this.dateFinValue = date;
        } else {
          // La nouvelle date est antérieure - redéfinir le début
          this.dateDebutValue = date;
          this.dateFinValue = null;
        }
      } else {
        // Les deux dates sont définies - recommencer avec cette date
        this.dateDebutValue = date;
        this.dateFinValue = null;
      }
    }

    this.emettreSelection();
  }

  // Fonction pour le DatePicker - permettre seulement les dates avec disponibilité
  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;

    // Empêcher les dates passées
    if (date < this.minDate) return false;

    // Vérifier si la date a des créneaux programmés ET avec disponibilité
    const tousCreneaux = this.tousLesCreneaux();
    const creneauAvecDisponibilite = tousCreneaux.find((c) => {
      const creneauDate = new Date(c.date);
      const targetDate = new Date(date);
      return creneauDate.toDateString() === targetDate.toDateString();
    });

    // Permettre seulement les dates qui ont des créneaux avec disponibilité > 0
    return creneauAvecDisponibilite ? creneauAvecDisponibilite.capaciteDisponible > 0 : false;
  };

  // Fonction pour le DatePicker de fin - même logique + vérifier que c'est après la date de début
  dateFilterFin = (date: Date | null): boolean => {
    if (!date) return false;

    // D'abord appliquer le filtre standard
    if (!this.dateFilter(date)) return false;

    // Empêcher les dates antérieures ou égales à la date de début
    const dateDebut = this.dateDebut();
    if (!dateDebut) return false;

    // La date de fin doit être strictement postérieure à la date de début
    return date > dateDebut;
  };

  // Fonction pour obtenir la classe CSS d'une date dans le calendrier
  dateClass = (cellDate: Date, view: 'month' | 'year' | 'multi-year'): string => {
    if (view !== 'month') return '';

    const tousCreneaux = this.tousLesCreneaux();
    const creneau = tousCreneaux.find((c) => {
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
    const tousCreneaux = this.tousLesCreneaux();
    const creneau = tousCreneaux.find((c) => {
      const creneauDate = new Date(c.date);
      const targetDate = new Date(date);
      return creneauDate.toDateString() === targetDate.toDateString();
    });
    return creneau ? creneau.capaciteDisponible > 0 : false;
  }

  getDisponibiliteInfo(date: Date): string {
    const tousCreneaux = this.tousLesCreneaux();
    const creneau = tousCreneaux.find((c) => c.date.getTime() === date.getTime());

    if (!creneau) return 'Non programmé';

    if (creneau.capaciteDisponible === 0) {
      return `Complet (${creneau.capaciteMax} places)`;
    }

    return `${creneau.capaciteDisponible}/${creneau.capaciteMax} places`;
  }

  estDateSelectionnee(date: Date): boolean {
    const debut = this.dateDebut();
    const fin = this.dateFin();

    if (!debut) return false;

    if (!this.estPeriode()) {
      const debutDate = new Date(debut);
      const targetDate = new Date(date);
      return debutDate.toDateString() === targetDate.toDateString();
    }

    if (!fin) {
      const debutDate = new Date(debut);
      const targetDate = new Date(date);
      return debutDate.toDateString() === targetDate.toDateString();
    }

    return date >= debut && date <= fin;
  }

  // Méthode de debug pour vérifier les créneaux
  debugCreneaux(): void {
    const creneaux = this.creneauxDisponibles();
    const tousCreneaux = this.tousLesCreneaux();
    console.log('=== DEBUG CRÉNEAUX ===');
    console.log('Créneaux disponibles:', creneaux.length);
    console.log('Tous les créneaux:', tousCreneaux.length);

    // Debug spécial pour le 30 août 2025
    const date30Aout = new Date(2025, 7, 30); // mois 7 = août (0-indexé)
    console.log('=== DEBUG 30 AOÛT 2025 ===');
    console.log('Date créée:', date30Aout.toDateString());
    console.log('Jour de la semaine:', date30Aout.getDay(), '(6=samedi)');

    const creneaux30Aout = tousCreneaux.filter((c) => {
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
    console.log('=== TEST COHÉRENCE SÉLECTION ===');
    for (let i = 0; i < 10; i++) {
      const testDate = new Date(today);
      testDate.setDate(today.getDate() + i);
      const className = this.dateClass(testDate, 'month');
      const isSelectable = this.dateFilter(testDate);
      const isDisponible = this.estDateDisponible(testDate);

      // Vérifier la cohérence
      const coherent = isSelectable === isDisponible;
      const status = coherent ? '✅' : '❌';

      console.log(
        `${status} J+${i} (${testDate.toLocaleDateString()}): classe="${className}", sélectionnable=${isSelectable}, disponible=${isDisponible}`
      );
    }

    // Forcer un rechargement des créneaux
    this.chargerCreneauxDisponibles();
  }
}
