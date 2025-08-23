import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PetType } from '../../features/pets/models/pet.model';
import {
  CreatePrestationRequest,
  CreatePrestationResponse,
  DeletePrestationResponse,
  GetAllPrestationsResponse,
  GetPrestationResponse,
  UpdatePrestationRequest,
  UpdatePrestationResponse
} from '../contracts/prestations/prestation.dto';

@Injectable({
  providedIn: 'root'
})
export class PrestationApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/v1/prestations`;

  getPrestations(filters?: {
    categorieAnimal?: PetType;
    searchText?: string;
    estDisponible?: boolean;
  }): Observable<GetAllPrestationsResponse> {
    let params = new HttpParams();

    if (filters?.categorieAnimal !== undefined) {
      params = params.set('categorieAnimal', filters.categorieAnimal.toString());
    }

    if (filters?.searchText) {
      params = params.set('searchText', filters.searchText);
    }

    if (filters?.estDisponible !== undefined) {
      params = params.set('estDisponible', filters.estDisponible.toString());
    }

    return this.http.get<GetAllPrestationsResponse>(this.baseUrl, { params });
  }

  getPrestationById(id: string): Observable<GetPrestationResponse> {
    return this.http.get<GetPrestationResponse>(`${this.baseUrl}/${id}`);
  }

  createPrestation(request: CreatePrestationRequest): Observable<CreatePrestationResponse> {
    return this.http.post<CreatePrestationResponse>(this.baseUrl, request);
  }

  updatePrestation(
    id: string,
    request: UpdatePrestationRequest
  ): Observable<UpdatePrestationResponse> {
    return this.http.put<UpdatePrestationResponse>(`${this.baseUrl}/${id}`, request);
  }

  deletePrestation(id: string): Observable<DeletePrestationResponse> {
    return this.http.delete<DeletePrestationResponse>(`${this.baseUrl}/${id}`);
  }
}
