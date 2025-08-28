import { Injectable, inject } from '@angular/core';
import { Observable, map, of } from 'rxjs';
import { PlanningPrestation } from '../models/prestation.model';
import { AvailableSlot, DisponibiliteQuery, DisponibiliteResponse } from '../models/Slot';
import { PlanningApiService, PlanningDto, CreneauDisponibleDto, DisponibiliteQueryDto } from '../../../shared/services/planning-api.service';

@Injectable({
  providedIn: 'root'
})
export class PlanningService {
  private readonly planningApi = inject(PlanningApiService);
  // Méthodes de conversion entre DTOs backend et modèles frontend
  private convertPlanningDtoToModel(dto: PlanningDto): PlanningPrestation {
    return {
      id: dto.id,
      prestationId: dto.prestationId,
      nom: dto.nom,
      description: dto.description,
      estActif: dto.estActif,
      dateCreation: new Date(dto.dateCreation),
      dateModification: dto.dateModification ? new Date(dto.dateModification) : undefined,
      creneaux: dto.creneaux.map(this.convertCreneauDtoToModel)
    };
  }

  private convertCreneauDtoToModel(dto: CreneauDisponibleDto): AvailableSlot {
    return {
      date: new Date(dto.date),
      capaciteMax: dto.capaciteMax,
      capaciteReservee: dto.capaciteReservee,
      capaciteDisponible: dto.capaciteDisponible
    };
  }

  private convertDisponibiliteQuery(query: DisponibiliteQuery): DisponibiliteQueryDto {
    return {
      prestationId: query.prestationId,
      startDate: query.startDate.toISOString(),
      endDate: query.endDate?.toISOString(),
      quantity: query.quantity
    };
  }


  getPlanningParPrestation(prestationId: string): Observable<PlanningPrestation | null> {
    return this.planningApi.getPlanningByPrestation(prestationId).pipe(
      map(response => response.success && response.data 
        ? this.convertPlanningDtoToModel(response.data) 
        : null
      )
    );
  }

  getTousLesPlannings(): Observable<PlanningPrestation[]> {
    return this.planningApi.getAllPlannings().pipe(
      map(response => response.success 
        ? response.data.map(dto => this.convertPlanningDtoToModel(dto))
        : []
      )
    );
  }

  verifierDisponibilite(query: DisponibiliteQuery): Observable<DisponibiliteResponse> {
    const queryDto = this.convertDisponibiliteQuery(query);
    return this.planningApi.verifierDisponibilite(queryDto).pipe(
      map(response => ({
        prestationId: response.prestationId,
        isAvailable: response.isAvailable,
        availablesSlots: response.availableSlots.map(dto => this.convertCreneauDtoToModel(dto)),
        message: response.message
      }))
    );
  }

  reserverCreneaux(
    prestationId: string,
    dateDebut: Date,
    dateFin: Date | null,
    quantite: number = 1
  ): Observable<boolean> {
    const request = {
      prestationId,
      startDate: dateDebut.toISOString(),
      endDate: dateFin?.toISOString(),
      quantity: quantite
    };

    return this.planningApi.reserverCreneaux(request).pipe(
      map(response => response.success)
    );
  }

  annulerReservations(
    prestationId: string,
    dateDebut: Date,
    dateFin: Date | null,
    quantite: number = 1
  ): Observable<boolean> {
    const request = {
      prestationId,
      startDate: dateDebut.toISOString(),
      endDate: dateFin?.toISOString(),
      quantity: quantite
    };

    return this.planningApi.annulerReservations(request).pipe(
      map(response => response.success)
    );
  }

  getCreneauxPourMois(
    prestationId: string,
    annee: number,
    mois: number
  ): Observable<AvailableSlot[]> {
    return this.getPlanningParPrestation(prestationId).pipe(
      map(planning => {
        if (!planning) {
          return [];
        }

        return planning.creneaux.filter(
          (c) => c.date.getFullYear() === annee && c.date.getMonth() === mois
        );
      })
    );
  }

  getCreneauxForDate(date: Date): Observable<AvailableSlot[]> {
    return this.getTousLesPlannings().pipe(
      map(plannings => {
        const allCreneaux: AvailableSlot[] = [];

        plannings.forEach((planning) => {
          if (planning.creneaux) {
            allCreneaux.push(...planning.creneaux);
          }
        });

        return allCreneaux.filter((creneau) => {
          const creneauDate = new Date(creneau.date);
          return creneauDate.toDateString() === date.toDateString();
        });
      })
    );
  }
}
