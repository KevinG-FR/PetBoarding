import { Injectable, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import { PlanningPrestation } from '../models/prestation.model';
import { AvailableSlot, DisponibiliteQuery, DisponibiliteResponse } from '../models/Slot';

@Injectable({
  providedIn: 'root'
})
export class PlanningService {
  private plannings = signal<PlanningPrestation[]>([
    {
      id: 'planning-1',
      prestationId: '1',
      nom: 'Planning Pension Complète',
      description: 'Planning pour la pension complète avec 5 places par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 5, new Date('2025-08-31'))
    },
    {
      id: 'planning-2',
      prestationId: '2',
      nom: 'Planning Garderie',
      description: 'Planning pour la garderie avec 10 places par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 10, new Date('2025-08-31'))
    },
    {
      id: 'planning-3',
      prestationId: '3',
      nom: 'Planning Toilettage',
      description: 'Planning pour le toilettage avec 4 créneaux par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 4, new Date('2025-08-31'))
    },
    {
      id: 'planning-4',
      prestationId: '4',
      nom: 'Planning Promenades',
      description: 'Planning pour les promenades avec 8 créneaux par jour',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxAvecVariations(new Date('2025-08-01'), new Date('2025-08-31'))
    },
    {
      id: 'planning-5',
      prestationId: '5',
      nom: 'Planning Garde Domicile',
      description: 'Planning pour les gardes à domicile',
      estActif: true,
      dateCreation: new Date('2025-01-01'),
      creneaux: this.genererCreneauxPourMois(new Date('2025-08-01'), 6, new Date('2025-08-31'))
    },
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

  private genererCreneauxPourMois(
    dateDebut: Date,
    capacite: number,
    dateFin: Date
  ): AvailableSlot[] {
    const creneaux: AvailableSlot[] = [];
    const dateCourante = new Date(dateDebut);

    while (dateCourante <= dateFin) {
      let reservationsExistantes = 0;
      const jourSemaine = dateCourante.getDay();
      const jourMois = dateCourante.getDate();

      if (jourMois <= 5 || jourMois === 29) {
        reservationsExistantes = capacite;
      } else if (jourMois <= 10) {
        reservationsExistantes = Math.max(0, capacite - Math.floor(Math.random() * 2 + 1));
      } else if (jourMois <= 15) {
        reservationsExistantes = Math.floor(Math.random() * (capacite / 3));
      } else {
        const scenario = Math.random();
        if (scenario < 0.2) {
          reservationsExistantes = capacite;
        } else if (scenario < 0.4) {
          reservationsExistantes = Math.max(0, capacite - Math.floor(Math.random() * 2 + 1));
        } else {
          reservationsExistantes = Math.floor(Math.random() * (capacite / 2));
        }
      }

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

      dateCourante.setDate(dateCourante.getDate() + 1);
    }

    return creneaux;
  }

  private estJourFerie(date: Date): boolean {
    const jourMois = date.getDate();
    const mois = date.getMonth();

    return (
      (mois === 7 && jourMois === 15) ||
      (mois === 7 && jourMois === 25) ||
      (mois === 8 && jourMois === 1)
    );
  }

  private genererCreneauxAvecVariations(dateDebut: Date, dateFin: Date): AvailableSlot[] {
    const creneaux: AvailableSlot[] = [];
    const dateCourante = new Date(dateDebut);

    while (dateCourante <= dateFin) {
      const estWeekend = dateCourante.getDay() === 0 || dateCourante.getDay() === 6;
      const capacite = estWeekend ? 12 : 8;

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

  getPlanningParPrestation(prestationId: string): Observable<PlanningPrestation | null> {
    const planning = this.plannings().find((p) => p.prestationId === prestationId);
    return of(planning || null);
  }

  getTousLesPlannings(): Observable<PlanningPrestation[]> {
    return of(this.plannings());
  }

  verifierDisponibilite(query: DisponibiliteQuery): Observable<DisponibiliteResponse> {
    const planning = this.plannings().find((p) => p.prestationId === query.prestationId);

    if (!planning || !planning.estActif) {
      return of({
        prestationId: query.prestationId,
        isAvailable: false,
        availablesSlots: [],
        message: 'Aucun planning actif trouvé pour cette prestation'
      });
    }

    const dateFin = query.endDate || query.startDate;
    const quantiteDemandee = query.quantity || 1;
    const creneauxDisponibles: AvailableSlot[] = [];
    let isAvailable = true;

    const dateCourante = new Date(query.startDate);
    while (dateCourante <= dateFin) {
      const creneau = planning.creneaux.find((c) => c.date.getTime() === dateCourante.getTime());

      if (!creneau || creneau.capaciteDisponible < quantiteDemandee) {
        isAvailable = false;
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
      isAvailable: isAvailable,
      availablesSlots: creneauxDisponibles,
      message: isAvailable
        ? 'Créneaux disponibles pour la période demandée'
        : 'Capacité insuffisante pour certaines dates'
    });
  }

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

    this.plannings.set([...plannings]);

    return of(true);
  }

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

    this.plannings.set([...plannings]);

    return of(true);
  }

  getCreneauxPourMois(
    prestationId: string,
    annee: number,
    mois: number
  ): Observable<AvailableSlot[]> {
    const planning = this.plannings().find((p) => p.prestationId === prestationId);

    if (!planning) {
      return of([]);
    }

    const creneauxDuMois = planning.creneaux.filter(
      (c) => c.date.getFullYear() === annee && c.date.getMonth() === mois
    );

    return of(creneauxDuMois);
  }

  getCreneauxForDate(date: Date): AvailableSlot[] {
    const allCreneaux: AvailableSlot[] = [];

    this.plannings().forEach((planning) => {
      if (planning.creneaux) {
        allCreneaux.push(...planning.creneaux);
      }
    });

    return allCreneaux.filter((creneau) => {
      const creneauDate = new Date(creneau.date);
      return creneauDate.toDateString() === date.toDateString();
    });
  }
}
