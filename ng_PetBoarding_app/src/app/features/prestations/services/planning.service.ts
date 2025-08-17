import { Injectable, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import {
  CreneauDisponible,
  DisponibiliteQuery,
  DisponibiliteResponse,
  PlanningPrestation
} from '../models/prestation.model';

@Injectable({
  providedIn: 'root'
})
export class PlanningService {
  private plannings = signal<PlanningPrestation[]>([
    // Planning pour Pension complète
    {
      id: 'planning-1',
      prestationId: '1',
      nom: 'Planning Pension Complète',
      description: 'Planning pour la pension complète avec 5 places par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 5, new Date('2025-08-31'))
    },
    // Planning pour Garderie journée
    {
      id: 'planning-2',
      prestationId: '2',
      nom: 'Planning Garderie',
      description: 'Planning pour la garderie avec 10 places par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 10, new Date('2025-08-31'))
    },
    // Planning pour Toilettage
    {
      id: 'planning-3',
      prestationId: '3',
      nom: 'Planning Toilettage',
      description: 'Planning pour le toilettage avec 4 créneaux par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 4, new Date('2025-08-31'))
    },
    // Planning pour Promenade
    {
      id: 'planning-4',
      prestationId: '4',
      nom: 'Planning Promenades',
      description: 'Planning pour les promenades avec 8 créneaux par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxAvecVariations(new Date('2025-08-01'), new Date('2025-08-31'))
    },
    // Planning pour Garde à domicile
    {
      id: 'planning-5',
      prestationId: '5',
      nom: 'Planning Garde Domicile',
      description: 'Planning pour les gardes à domicile',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 6, new Date('2025-08-31'))
    },
    // Planning pour Pension chat
    {
      id: 'planning-6',
      prestationId: '6',
      nom: 'Planning Pension Chat',
      description: 'Planning pour la pension des chats avec 8 places par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 8, new Date('2025-08-31'))
    }
  ]);

  /**
   * Génère des créneaux avec la même capacité pour une période donnée
   * Inclut différents scénarios : disponible, limité, complet
   */
  private genererCreneauxPourMois(
    dateDebut: Date,
    capacite: number,
    dateFin: Date
  ): CreneauDisponible[] {
    const creneaux: CreneauDisponible[] = [];
    const dateCourante = new Date(dateDebut);

    while (dateCourante <= dateFin) {
      let reservationsExistantes = 0;
      const jourSemaine = dateCourante.getDay(); // 0 = dimanche, 6 = samedi
      const jourMois = dateCourante.getDate();

      // Créer différents scénarios de disponibilité pour les tests
      if (jourMois <= 5 || jourMois === 29) {
        // Première semaine : créneaux pleins (pour tester date-full)
        reservationsExistantes = capacite; // Complet
      } else if (jourMois <= 10) {
        // Deuxième semaine : places limitées (pour tester date-limited)
        reservationsExistantes = Math.max(0, capacite - Math.floor(Math.random() * 2 + 1)); // 1-2 places restantes
      } else if (jourMois <= 15) {
        // Troisième semaine : bien disponible (pour tester date-available)
        reservationsExistantes = Math.floor(Math.random() * (capacite / 3)); // Beaucoup de places
      } else {
        // Reste du mois : mix aléatoire
        const scenario = Math.random();
        if (scenario < 0.2) {
          // 20% de chances d'être complet
          reservationsExistantes = capacite;
        } else if (scenario < 0.4) {
          // 20% de chances d'être limité
          reservationsExistantes = Math.max(0, capacite - Math.floor(Math.random() * 2 + 1));
        } else {
          // 60% de chances d'être disponible
          reservationsExistantes = Math.floor(Math.random() * (capacite / 2));
        }
      }

      // Ne pas générer de créneaux seulement pour les dimanches (pour tester date-unavailable)
      // Les samedis sont maintenus avec un service normal
      const estDimanche = jourSemaine === 0;
      const estJourFerie = this.estJourFerie(dateCourante);

      if (!estDimanche && !estJourFerie) {
        creneaux.push({
          date: new Date(dateCourante),
          capaciteMax: capacite,
          capaciteReservee: reservationsExistantes,
          capaciteDisponible: capacite - reservationsExistantes
        });
      }
      // Si c'est dimanche ou jour férié, on ne génère pas de créneau
      // = date non programmée = date-unavailable

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    return creneaux;
  }

  /**
   * Détermine si une date est un jour férié (simplifié)
   */
  private estJourFerie(date: Date): boolean {
    const jourMois = date.getDate();
    const mois = date.getMonth();

    // Quelques jours fériés fixes pour les tests
    return (
      (mois === 7 && jourMois === 15) || // 15 août
      (mois === 7 && jourMois === 25) || // Arbitraire pour tests
      (mois === 8 && jourMois === 1)
    ); // Arbitraire pour tests
  }

  /**
   * Génère des créneaux avec des variations de capacité (weekend vs semaine)
   */
  private genererCreneauxAvecVariations(dateDebut: Date, dateFin: Date): CreneauDisponible[] {
    const creneaux: CreneauDisponible[] = [];
    const dateCourante = new Date(dateDebut);

    while (dateCourante <= dateFin) {
      // Plus de capacité le weekend
      const estWeekend = dateCourante.getDay() === 0 || dateCourante.getDay() === 6;
      const capacite = estWeekend ? 12 : 8;

      // Simuler quelques réservations existantes
      const reservationsExistantes = Math.floor(Math.random() * (capacite / 3));

      creneaux.push({
        date: new Date(dateCourante),
        capaciteMax: capacite,
        capaciteReservee: reservationsExistantes,
        capaciteDisponible: capacite - reservationsExistantes
      });

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    return creneaux;
  }

  /**
   * Obtient le planning d'une prestation
   */
  getPlanningParPrestation(prestationId: string): Observable<PlanningPrestation | null> {
    const planning = this.plannings().find((p) => p.prestationId === prestationId);
    return of(planning || null);
  }

  /**
   * Obtient tous les plannings
   */
  getTousLesPlannings(): Observable<PlanningPrestation[]> {
    return of(this.plannings());
  }

  /**
   * Vérifie la disponibilité pour une période donnée
   */
  verifierDisponibilite(query: DisponibiliteQuery): Observable<DisponibiliteResponse> {
    const planning = this.plannings().find((p) => p.prestationId === query.prestationId);

    if (!planning || !planning.estActif) {
      return of({
        prestationId: query.prestationId,
        estDisponible: false,
        creneauxDisponibles: [],
        message: 'Aucun planning actif trouvé pour cette prestation'
      });
    }

    const dateFin = query.dateFin || query.dateDebut;
    const quantiteDemandee = query.quantite || 1;
    const creneauxDisponibles: CreneauDisponible[] = [];
    let estDisponible = true;

    // Vérifier chaque jour de la période
    const dateCourante = new Date(query.dateDebut);
    while (dateCourante <= dateFin) {
      const creneau = planning.creneaux.find((c) => c.date.getTime() === dateCourante.getTime());

      if (!creneau || creneau.capaciteDisponible < quantiteDemandee) {
        estDisponible = false;
        if (creneau) {
          creneauxDisponibles.push(creneau);
        }
      } else {
        creneauxDisponibles.push(creneau);
      }

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    return of({
      prestationId: query.prestationId,
      estDisponible,
      creneauxDisponibles,
      message: estDisponible
        ? 'Créneaux disponibles pour la période demandée'
        : 'Capacité insuffisante pour certaines dates'
    });
  }

  /**
   * Réserve des créneaux pour une période
   */
  reserverCreneaux(
    prestationId: string,
    dateDebut: Date,
    dateFin: Date | null,
    quantite: number = 1
  ): Observable<boolean> {
    const plannings = this.plannings();
    const planningIndex = plannings.findIndex((p) => p.prestationId === prestationId);

    if (planningIndex === -1) {
      return of(false);
    }

    const planning = plannings[planningIndex];
    const dateFinal = dateFin || dateDebut;

    // Vérifier d'abord la disponibilité
    const dateCourante = new Date(dateDebut);
    while (dateCourante <= dateFinal) {
      const creneauIndex = planning.creneaux.findIndex(
        (c) => c.date.getTime() === dateCourante.getTime()
      );

      if (creneauIndex === -1 || planning.creneaux[creneauIndex].capaciteDisponible < quantite) {
        return of(false);
      }

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    // Effectuer la réservation
    dateCourante.setTime(dateDebut.getTime());
    while (dateCourante <= dateFinal) {
      const creneauIndex = planning.creneaux.findIndex(
        (c) => c.date.getTime() === dateCourante.getTime()
      );

      if (creneauIndex !== -1) {
        const creneau = planning.creneaux[creneauIndex];
        creneau.capaciteReservee += quantite;
        creneau.capaciteDisponible = creneau.capaciteMax - creneau.capaciteReservee;
      }

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    // Mettre à jour le signal
    this.plannings.set([...plannings]);

    return of(true);
  }

  /**
   * Annule des réservations pour une période
   */
  annulerReservations(
    prestationId: string,
    dateDebut: Date,
    dateFin: Date | null,
    quantite: number = 1
  ): Observable<boolean> {
    const plannings = this.plannings();
    const planningIndex = plannings.findIndex((p) => p.prestationId === prestationId);

    if (planningIndex === -1) {
      return of(false);
    }

    const planning = plannings[planningIndex];
    const dateFinal = dateFin || dateDebut;

    // Annuler les réservations
    const dateCourante = new Date(dateDebut);
    while (dateCourante <= dateFinal) {
      const creneauIndex = planning.creneaux.findIndex(
        (c) => c.date.getTime() === dateCourante.getTime()
      );

      if (creneauIndex !== -1) {
        const creneau = planning.creneaux[creneauIndex];
        creneau.capaciteReservee = Math.max(0, creneau.capaciteReservee - quantite);
        creneau.capaciteDisponible = creneau.capaciteMax - creneau.capaciteReservee;
      }

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    // Mettre à jour le signal
    this.plannings.set([...plannings]);

    return of(true);
  }

  /**
   * Obtient les créneaux disponibles pour un mois donné
   */
  getCreneauxPourMois(
    prestationId: string,
    annee: number,
    mois: number
  ): Observable<CreneauDisponible[]> {
    const planning = this.plannings().find((p) => p.prestationId === prestationId);

    if (!planning) {
      return of([]);
    }

    const creneauxDuMois = planning.creneaux.filter(
      (c) => c.date.getFullYear() === annee && c.date.getMonth() === mois
    );

    return of(creneauxDuMois);
  }

  /**
   * Obtient les créneaux pour une date spécifique (debug)
   */
  getCreneauxForDate(date: Date): CreneauDisponible[] {
    const allCreneaux: CreneauDisponible[] = [];

    // Collecter tous les créneaux de tous les plannings
    this.plannings().forEach((planning) => {
      if (planning.creneaux) {
        allCreneaux.push(...planning.creneaux);
      }
    });

    // Filtrer par date
    return allCreneaux.filter((creneau) => {
      const creneauDate = new Date(creneau.date);
      return creneauDate.toDateString() === date.toDateString();
    });
  }

  /**
   * Debug : Affiche les informations pour une date spécifique
   */
  debugDate(date: Date): void {
    const creneaux = this.getCreneauxForDate(date);
    console.log(`=== DEBUG DATE ${date.toDateString()} ===`);
    console.log('Jour de la semaine:', date.getDay()); // 0=dimanche, 6=samedi
    console.log('Créneaux trouvés:', creneaux.length);
    console.log('Détails des créneaux:', creneaux);
    console.log('Est jour férié:', this.estJourFerie(date));
  }
}
