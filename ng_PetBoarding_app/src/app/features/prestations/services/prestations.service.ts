import { Injectable, inject, signal } from '@angular/core';
import { Observable, map } from 'rxjs';
import { PetType } from '../../pets/models/pet.model';
import { PlanningPrestation, Prestation, PrestationFilters } from '../models/prestation.model';
import { PlanningService } from './planning.service';

export interface CategoryInfo {
  label: string;
  icon: string;
  color: string;
}

@Injectable({
  providedIn: 'root'
})
export class PrestationsService {
  private readonly planningService = inject(PlanningService);

  private prestations = signal<Prestation[]>([
    {
      id: '1',
      libelle: 'Pension complète',
      description: 'Garde de jour et nuit avec promenades et soins',
      categorieAnimal: PetType.DOG,
      prix: 35,
      duree: 1440,
      disponible: true
    },
    {
      id: '2',
      libelle: 'Garderie journée',
      description: 'Garde en journée avec activités et socialisation',
      categorieAnimal: PetType.DOG,
      prix: 25,
      duree: 480,
      disponible: true
    },
    {
      id: '3',
      libelle: 'Toilettage complet',
      description: 'Bain, coupe, griffes et soins esthétiques',
      categorieAnimal: PetType.DOG,
      prix: 45,
      duree: 120,
      disponible: true
    },
    {
      id: '4',
      libelle: 'Promenade',
      description: 'Sortie individuelle ou en groupe',
      categorieAnimal: PetType.DOG,
      prix: 15,
      duree: 60,
      disponible: true
    },
    {
      id: '5',
      libelle: 'Garde à domicile',
      description: 'Visite et soins au domicile du propriétaire',
      categorieAnimal: PetType.CAT,
      prix: 20,
      duree: 30,
      disponible: true
    },
    {
      id: '6',
      libelle: 'Pension chat',
      description: 'Hébergement en chatterie avec soins personnalisés',
      categorieAnimal: PetType.CAT,
      prix: 25,
      duree: 1440,
      disponible: true
    },
    {
      id: '7',
      libelle: 'Toilettage chat',
      description: 'Brossage, bain et coupe de griffes',
      categorieAnimal: PetType.CAT,
      prix: 35,
      duree: 90,
      disponible: true
    },
    {
      id: '8',
      libelle: 'Consultation comportementale',
      description: 'Séance avec un spécialiste du comportement animal',
      categorieAnimal: PetType.DOG,
      prix: 60,
      duree: 60,
      disponible: false
    }
  ]);

  getAllPrestations() {
    return this.prestations.asReadonly();
  }

  getAllPrestationsAvecPlanning(): Observable<Prestation[]> {
    return this.planningService.getTousLesPlannings().pipe(
      map((plannings: PlanningPrestation[]) => {
        return this.prestations().map((prestation: Prestation) => {
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
  }

  getPrestationAvecPlanning(prestationId: string): Observable<Prestation | null> {
    return this.planningService.getPlanningParPrestation(prestationId).pipe(
      map((planning: PlanningPrestation | null) => {
        const prestation = this.getPrestationById(prestationId);
        if (!prestation) return null;

        return {
          ...prestation,
          planning: planning || undefined
        };
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

  getPrestationById(id: string): Prestation | undefined {
    return this.prestations().find((p: Prestation) => p.id === id);
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
          label: category,
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
