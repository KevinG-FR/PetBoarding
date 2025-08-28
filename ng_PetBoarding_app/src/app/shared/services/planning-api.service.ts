import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

// Backend DTOs
export interface PlanningDto {
  id: string;
  prestationId: string;
  nom: string;
  description?: string;
  estActif: boolean;
  dateCreation: string;
  dateModification?: string;
  creneaux: CreneauDisponibleDto[];
}

export interface CreneauDisponibleDto {
  date: string;
  capaciteMax: number;
  capaciteReservee: number;
  capaciteDisponible: number;
}

export interface DisponibiliteQueryDto {
  prestationId: string;
  startDate: string;
  endDate?: string;
  quantity?: number;
}

export interface DisponibiliteResponseDto {
  prestationId: string;
  isAvailable: boolean;
  availableSlots: CreneauDisponibleDto[];
  message?: string;
}

export interface ReserverCreneauxRequest {
  prestationId: string;
  startDate: string;
  endDate?: string;
  quantity: number;
}

export interface ReservationResponse {
  success: boolean;
  message?: string;
}

export interface GetAllPlanningsResponse {
  success: boolean;
  data: PlanningDto[];
  message?: string;
}

export interface GetPlanningResponse {
  success: boolean;
  data?: PlanningDto;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PlanningApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/v1/planning`;

  getAllPlannings(): Observable<GetAllPlanningsResponse> {
    return this.http.get<GetAllPlanningsResponse>(this.baseUrl);
  }

  getPlanningByPrestation(prestationId: string): Observable<GetPlanningResponse> {
    return this.http.get<GetPlanningResponse>(`${this.baseUrl}/prestation/${prestationId}`);
  }

  verifierDisponibilite(query: DisponibiliteQueryDto): Observable<DisponibiliteResponseDto> {
    return this.http.post<DisponibiliteResponseDto>(`${this.baseUrl}/disponibilite`, query);
  }

  reserverCreneaux(request: ReserverCreneauxRequest): Observable<ReservationResponse> {
    return this.http.post<ReservationResponse>(`${this.baseUrl}/reserver`, request);
  }

  annulerReservations(request: ReserverCreneauxRequest): Observable<ReservationResponse> {
    return this.http.post<ReservationResponse>(`${this.baseUrl}/annuler`, request);
  }
}