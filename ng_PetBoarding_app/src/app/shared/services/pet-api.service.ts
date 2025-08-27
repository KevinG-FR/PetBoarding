import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PetType } from '../../features/pets/models/pet.model';

// Mapping pour convertir PetType frontend vers backend
const BackendPetTypeMap = {
  [PetType.DOG]: 1,    // Chien
  [PetType.CAT]: 2,    // Chat  
  [PetType.BIRD]: 3,   // Oiseau
  [PetType.RABBIT]: 4, // Lapin
  [PetType.HAMSTER]: 5 // Hamster
} as const;

import {
  CreatePetRequest,
  CreatePetResponse,
  DeletePetResponse,
  GetAllPetsResponse,
  GetPetResponse,
  UpdatePetRequest,
  UpdatePetResponse
} from '../contracts/pets/pet.dto';

@Injectable({
  providedIn: 'root'
})
export class PetApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/v1/pets`;

  /**
   * Récupère tous les pets de l'utilisateur connecté
   */
  getPets(filters?: {
    type?: PetType;
  }): Observable<GetAllPetsResponse> {
    let params = new HttpParams();

    if (filters?.type !== undefined) {
      const backendType = BackendPetTypeMap[filters.type];
      params = params.set('type', backendType.toString());
    }

    return this.http.get<GetAllPetsResponse>(this.baseUrl, { params });
  }

  /**
   * Récupère tous les pets d'un propriétaire spécifique
   */
  getPetsByOwner(ownerId: string, filters?: {
    type?: PetType;
  }): Observable<GetAllPetsResponse> {
    let params = new HttpParams();

    if (filters?.type !== undefined) {
      const backendType = BackendPetTypeMap[filters.type];
      params = params.set('type', backendType.toString());
    }

    return this.http.get<GetAllPetsResponse>(`${this.baseUrl}/owner/${ownerId}`, { params });
  }

  /**
   * Récupère un pet par son ID
   */
  getPetById(id: string): Observable<GetPetResponse> {
    return this.http.get<GetPetResponse>(`${this.baseUrl}/${id}`);
  }

  /**
   * Crée un nouveau pet
   */
  createPet(request: CreatePetRequest): Observable<CreatePetResponse> {
    return this.http.post<CreatePetResponse>(this.baseUrl, request);
  }

  /**
   * Met à jour un pet existant
   */
  updatePet(
    id: string,
    request: UpdatePetRequest
  ): Observable<UpdatePetResponse> {
    return this.http.put<UpdatePetResponse>(`${this.baseUrl}/${id}`, request);
  }

  /**
   * Supprime un pet
   */
  deletePet(id: string): Observable<DeletePetResponse> {
    return this.http.delete<DeletePetResponse>(`${this.baseUrl}/${id}`);
  }
}