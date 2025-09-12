import { Injectable, inject, signal } from '@angular/core';
import { Observable, catchError, map, of, tap } from 'rxjs';
import {
  CreatePrestationRequest,
  PrestationDto,
  UpdatePrestationRequest
} from '../../../shared/contracts/prestations/prestation.dto';
import { PrestationApiService } from '../../../shared/services/prestation-api.service';
import { PetType } from '../../pets/models/pet.model';
import { PlanningPrestation, Prestation, PrestationFilters } from '../models/prestation.model';
import { PlanningService } from './planning.service';

export interface CategoryInfo {
  label: string;
  icon: string;
  color: string;
}

// Mapping des types d'animaux entre backend (TypeAnimal enum) et frontend (PetType enum)
const FrontendPetTypeMap = {
  1: PetType.DOG, // TypeAnimal.Chien = 1
  2: PetType.CAT, // TypeAnimal.Chat = 2
  3: PetType.BIRD, // TypeAnimal.Oiseau = 3
  4: PetType.RABBIT, // TypeAnimal.Lapin = 4
  5: PetType.HAMSTER, // TypeAnimal.Hamster = 5
  99: PetType.DOG // TypeAnimal.Autre = 99 -> par défaut Chien
} as const;

import { switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PrestationsService {
  private readonly planningService = inject(PlanningService);
  private readonly prestationApiService = inject(PrestationApiService);

  // Signal pour stocker les prestations en cache local
  private prestations = signal<Prestation[]>([]);
  private isLoading = signal<boolean>(false);
  private error = signal<string | null>(null);

  /**
   * Convertit un type d'animal backend (entier) en PetType frontend (enum string)
   * Méthode publique pour être utilisée dans tous les composants
   */
  mapBackendPetType(backendType: any): PetType {
    const mapped =
      FrontendPetTypeMap[backendType as keyof typeof FrontendPetTypeMap] || backendType;
    return mapped;
  }

  private mapDtoToPrestation(dto: PrestationDto): Prestation {
    return {
      id: dto.id,
      libelle: dto.libelle,
      description: dto.description,
      categorieAnimal: this.mapBackendPetType(dto.categorieAnimal),
      prix: dto.prix,
      duree: dto.dureeEnMinutes,
      disponible: dto.estDisponible
    };
  }

  /**
   * Convertit une Prestation en CreatePrestationRequest
   */
  private mapPrestationToCreateRequest(
    prestation: Omit<Prestation, 'id'>
  ): CreatePrestationRequest {
    return {
      libelle: prestation.libelle,
      description: prestation.description,
      categorieAnimal: prestation.categorieAnimal,
      prix: prestation.prix,
      dureeEnMinutes: prestation.duree,
      estDisponible: prestation.disponible
    };
  }

  /**
   * Charge toutes les prestations depuis l'API
   */
  loadPrestations(filters?: PrestationFilters): Observable<Prestation[]> {
    this.isLoading.set(true);
    this.error.set(null);

    const apiFilters = filters
      ? {
          categorieAnimal: filters.categorieAnimal,
          searchText: filters.searchText
        }
      : undefined;

    return this.prestationApiService.getPrestations(apiFilters).pipe(
      map((response) => response.prestations.map((dto) => this.mapDtoToPrestation(dto))),
      tap((prestations) => {
        this.prestations.set(prestations);
        this.isLoading.set(false);
      }),
      catchError((_) => {
        this.error.set('Erreur lors du chargement des prestations');
        this.isLoading.set(false);
        return of([]);
      })
    );
  }

  /**
   * Récupère toutes les prestations (depuis le cache local ou charge depuis l'API)
   */
  getAllPrestations() {
    if (this.prestations().length === 0) {
      // Si pas de données en cache, charge depuis l'API
      this.loadPrestations().subscribe();
    }
    return this.prestations.asReadonly();
  }

  /**
   * Récupère une prestation par son ID
   */
  getPrestationById(id: string): Observable<Prestation | null> {
    return this.prestationApiService.getPrestationById(id).pipe(
      map((response) => this.mapDtoToPrestation(response.prestation)),
      catchError((_) => {
        return of(null);
      })
    );
  }

  /**
   * Crée une nouvelle prestation
   */
  createPrestation(prestation: Omit<Prestation, 'id'>): Observable<boolean> {
    const request = this.mapPrestationToCreateRequest(prestation);

    return this.prestationApiService.createPrestation(request).pipe(
      tap((response) => {
        if (response.prestation) {
          // Recharge les prestations après création
          this.loadPrestations().subscribe();
        }
      }),
      map((response) => !!response.prestation),
      catchError((_) => {
        return of(false);
      })
    );
  }

  /**
   * Met à jour une prestation existante
   */
  updatePrestation(id: string, updates: Partial<Prestation>): Observable<boolean> {
    const request: UpdatePrestationRequest = {};

    if (updates.libelle !== undefined) request.libelle = updates.libelle;
    if (updates.description !== undefined) request.description = updates.description;
    if (updates.categorieAnimal !== undefined) request.categorieAnimal = updates.categorieAnimal;
    if (updates.prix !== undefined) request.prix = updates.prix;
    if (updates.duree !== undefined) request.dureeEnMinutes = updates.duree;
    if (updates.disponible !== undefined) request.estDisponible = updates.disponible;

    return this.prestationApiService.updatePrestation(id, request).pipe(
      tap((response) => {
        if (response.prestation) {
          // Recharge les prestations après mise à jour
          this.loadPrestations().subscribe();
        }
      }),
      map((response) => !!response.prestation),
      catchError((_) => {
        return of(false);
      })
    );
  }

  /**
   * Supprime une prestation
   */
  deletePrestation(id: string): Observable<boolean> {
    return this.prestationApiService.deletePrestation(id).pipe(
      tap((response) => {
        if (response.success) {
          // Recharge les prestations après suppression
          this.loadPrestations().subscribe();
        }
      }),
      map((response) => response.success),
      catchError((_) => {
        return of(false);
      })
    );
  }

  getAllPrestationsAvecPlanning(): Observable<Prestation[]> {
    return this.planningService
      .getTousLesPlannings()
      .pipe(
        map((plannings: PlanningPrestation[]) => {
          // Utilise les prestations depuis l'API au lieu du cache local
          return this.loadPrestations().pipe(
            map((prestations: Prestation[]) => {
              return prestations.map((prestation: Prestation) => {
                const planning = plannings.find(
                  (p: PlanningPrestation) => p.prestationId === prestation.id
                );
                return {
                  ...prestation,
                  planning: planning || undefined
                };
              });
            })
          );
        })
      )
      .pipe(
        // Flatten l'Observable imbriqué
        switchMap((prestationsObs) => prestationsObs)
      );
  }

  getPrestationAvecPlanning(prestationId: string): Observable<Prestation | null> {
    return this.planningService.getPlanningParPrestation(prestationId).pipe(
      switchMap((planning: PlanningPrestation | null) => {
        return this.getPrestationById(prestationId).pipe(
          map((prestation: Prestation | null) => {
            if (!prestation) return null;

            return {
              ...prestation,
              planning: planning || undefined
            };
          })
        );
      })
    );
  }

  createFilteredPrestations(prestations: Prestation[], filters: PrestationFilters): Prestation[] {
    return prestations.filter((prestation: Prestation) => {
      if (filters.categorieAnimal && prestation.categorieAnimal !== filters.categorieAnimal) {
        return false;
      }

      if (filters.searchText) {
        const searchTerm = filters.searchText.toLowerCase().trim();
        if (searchTerm) {
          const matchesLibelle = prestation.libelle.toLowerCase().includes(searchTerm);
          const matchesDescription = prestation.description.toLowerCase().includes(searchTerm);
          if (!matchesLibelle && !matchesDescription) {
            return false;
          }
        }
      }

      return true;
    });
  }

  /**
   * Récupère une prestation par son ID depuis le cache local (méthode synchrone)
   */
  getPrestationFromCache(id: string): Prestation | undefined {
    return this.prestations().find((p: Prestation) => p.id === id);
  }

  /**
   * Getters pour les états
   */
  getIsLoading() {
    return this.isLoading.asReadonly();
  }

  getError() {
    return this.error.asReadonly();
  }

  getCategoriesAnimaux(): PetType[] {
    return Object.values(PetType);
  }

  getCategoryInfo(category: PetType): CategoryInfo {
    switch (category) {
      case PetType.DOG:
        return {
          label: 'Chien',
          icon: 'fas fa-dog',
          color: '#1976d2'
        };
      case PetType.CAT:
        return {
          label: 'Chat',
          icon: 'fas fa-cat',
          color: '#7b1fa2'
        };
      case PetType.BIRD:
        return {
          label: 'Oiseau',
          icon: 'fas fa-dove',
          color: '#ff9800'
        };
      case PetType.RABBIT:
        return {
          label: 'Lapin',
          icon: 'fas fa-carrot',
          color: '#4caf50'
        };
      case PetType.HAMSTER:
        return {
          label: 'Hamster',
          icon: 'fas fa-paw',
          color: '#9c27b0'
        };
      default:
        return {
          label: String(category),
          icon: 'fas fa-paw',
          color: '#666666'
        };
    }
  }

  getCategoryIcon(category: PetType): string {
    return this.getCategoryInfo(category).icon;
  }

  getCategoryLabel(category: PetType): string {
    return this.getCategoryInfo(category).label;
  }

  getCategoryColor(category: PetType): string {
    return this.getCategoryInfo(category).color;
  }
}
