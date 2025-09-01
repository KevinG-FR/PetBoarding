import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

// Backend DTOs - Alignés avec le backend .NET
export interface ReservationDto {
  id: string;
  userId: string;
  animalId: string;
  animalName: string;
  serviceId: string;
  startDate: string;
  endDate?: string;
  comments?: string;
  status: string;
  createdAt: string;
  updatedAt?: string;
  totalPrice?: number;
}

export interface CreateReservationRequest {
  userId: string;
  animalId: string;
  animalName: string;
  serviceId: string;
  startDate: string;
  endDate?: string;
  comments?: string;
}

export interface CreateReservationResponse {
  reservation: ReservationDto;
}

export interface GetAllReservationsResponse {
  reservations: ReservationDto[];
  totalCount: number;
}

export interface GetReservationResponse {
  success: boolean;
  data?: ReservationDto;
  message?: string;
}

export interface UpdateReservationRequest {
  animalId?: string;
  animalName?: string;
  serviceId?: string;
  startDate?: string;
  endDate?: string;
  comments?: string;
}

export interface ValidatePaymentRequest {
  amountPaid: number;
  paymentMethod?: string;
}

export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReservationsApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/v1/reservations`;

  /**
   * Récupère toutes les réservations
   */
  getAllReservations(): Observable<GetAllReservationsResponse> {
    return this.http.get<GetAllReservationsResponse>(this.baseUrl);
  }

  /**
   * Récupère une réservation par son ID
   */
  getReservationById(id: string): Observable<GetReservationResponse> {
    return this.http.get<GetReservationResponse>(`${this.baseUrl}/${id}`);
  }

  /**
   * Crée une nouvelle réservation
   */
  createReservation(request: CreateReservationRequest): Observable<CreateReservationResponse> {
    return this.http.post<CreateReservationResponse>(this.baseUrl, request);
  }

  /**
   * Met à jour une réservation
   */
  updateReservation(id: string, request: UpdateReservationRequest): Observable<ApiResponse<ReservationDto>> {
    return this.http.put<ApiResponse<ReservationDto>>(`${this.baseUrl}/${id}`, request);
  }

  /**
   * Annule une réservation
   */
  cancelReservation(id: string): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.baseUrl}/${id}`);
  }

  /**
   * Valide le paiement d'une réservation
   */
  validatePayment(id: string, request: ValidatePaymentRequest): Observable<ApiResponse<ReservationDto>> {
    return this.http.post<ApiResponse<ReservationDto>>(`${this.baseUrl}/${id}/validate-payment`, request);
  }

  /**
   * Récupère les réservations d'un utilisateur
   */
  getUserReservations(userId: string): Observable<GetAllReservationsResponse> {
    return this.http.get<GetAllReservationsResponse>(`${this.baseUrl}/user/${userId}`);
  }

  /**
   * Récupère les réservations affichées d'un utilisateur (Validated, InProgress, Completed uniquement)
   */
  getUserDisplayedReservations(userId: string): Observable<GetAllReservationsResponse> {
    return this.http.get<GetAllReservationsResponse>(`${this.baseUrl}/user/${userId}/displayed`);
  }

  /**
   * Récupère les réservations par statut
   */
  getReservationsByStatus(status: string): Observable<GetAllReservationsResponse> {
    return this.http.get<GetAllReservationsResponse>(`${this.baseUrl}/status/${status}`);
  }

  /**
   * Récupère les réservations pour une période donnée
   */
  getReservationsForPeriod(startDate: string, endDate?: string): Observable<GetAllReservationsResponse> {
    const params: any = { startDate };
    if (endDate) {
      params.endDate = endDate;
    }
    return this.http.get<GetAllReservationsResponse>(`${this.baseUrl}/period`, { params });
  }
}