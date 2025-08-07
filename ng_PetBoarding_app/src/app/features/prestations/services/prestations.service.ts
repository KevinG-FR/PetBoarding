import { computed, Injectable, signal } from '@angular/core';
import { CategorieAnimal, Prestation, PrestationFilters } from '../models/prestation.model';

export interface CategoryInfo {
  label: string;
  icon: string;
  color: string;
}

@Injectable({
  providedIn: 'root'
})
export class PrestationsService {
  private prestations = signal<Prestation[]>([
    {
      id: '1',
      libelle: 'Pension complète',
      description: 'Garde de jour et nuit avec promenades et soins',
      categorieAnimal: CategorieAnimal.CHIEN,
      prix: 35,
      duree: 1440, // 24h en minutes
      disponible: true
    },
    {
      id: '2',
      libelle: 'Garderie journée',
      description: 'Garde en journée avec activités et socialisation',
      categorieAnimal: CategorieAnimal.CHIEN,
      prix: 25,
      duree: 480, // 8h en minutes
      disponible: true
    },
    {
      id: '3',
      libelle: 'Toilettage complet',
      description: 'Bain, coupe, griffes et soins esthétiques',
      categorieAnimal: CategorieAnimal.CHIEN,
      prix: 45,
      duree: 120, // 2h en minutes
      disponible: true
    },
    {
      id: '4',
      libelle: 'Promenade',
      description: 'Sortie individuelle ou en groupe',
      categorieAnimal: CategorieAnimal.CHIEN,
      prix: 15,
      duree: 60, // 1h en minutes
      disponible: true
    },
    {
      id: '5',
      libelle: 'Garde à domicile',
      description: 'Visite et soins au domicile du propriétaire',
      categorieAnimal: CategorieAnimal.CHAT,
      prix: 20,
      duree: 30, // 30min en minutes
      disponible: true
    },
    {
      id: '6',
      libelle: 'Pension chat',
      description: 'Hébergement en chatterie avec soins personnalisés',
      categorieAnimal: CategorieAnimal.CHAT,
      prix: 25,
      duree: 1440, // 24h en minutes
      disponible: true
    },
    {
      id: '7',
      libelle: 'Toilettage chat',
      description: 'Brossage, bain et coupe de griffes',
      categorieAnimal: CategorieAnimal.CHAT,
      prix: 35,
      duree: 90, // 1h30 en minutes
      disponible: true
    },
    {
      id: '8',
      libelle: 'Consultation comportementale',
      description: 'Séance avec un spécialiste du comportement animal',
      categorieAnimal: CategorieAnimal.CHIEN,
      prix: 60,
      duree: 60, // 1h en minutes
      disponible: false
    }
  ]);

  private filters = signal<PrestationFilters>({});

  // Computed signal pour filtrer les prestations
  filteredPrestations = computed(() => {
    const allPrestations = this.prestations();
    const currentFilters = this.filters();

    return allPrestations.filter((prestation) => {
      // Filtre par catégorie d'animal
      if (
        currentFilters.categorieAnimal &&
        prestation.categorieAnimal !== currentFilters.categorieAnimal
      ) {
        return false;
      }

      // Filtre par texte de recherche
      if (currentFilters.searchText) {
        const searchText = currentFilters.searchText.toLowerCase();
        return (
          prestation.libelle.toLowerCase().includes(searchText) ||
          prestation.description.toLowerCase().includes(searchText)
        );
      }

      return true;
    });
  });

  // Getters pour exposer les signals
  getAllPrestations() {
    return this.prestations.asReadonly();
  }

  getFilteredPrestations() {
    return this.filteredPrestations;
  }

  getFilters() {
    return this.filters.asReadonly();
  }

  // Méthodes pour modifier les filtres
  updateFilters(newFilters: PrestationFilters) {
    this.filters.set({ ...this.filters(), ...newFilters });
  }

  clearFilters() {
    this.filters.set({});
  }

  // Méthodes CRUD (pour plus tard)
  getPrestationById(id: string): Prestation | undefined {
    return this.prestations().find((p) => p.id === id);
  }

  getCategoriesAnimaux(): CategorieAnimal[] {
    return Object.values(CategorieAnimal);
  }

  // Méthodes utilitaires pour l'affichage
  getCategoryInfo(category: CategorieAnimal): CategoryInfo {
    switch (category) {
      case CategorieAnimal.CHIEN:
        return {
          label: 'Chien',
          icon: 'fas fa-dog',
          color: '#1976d2'
        };
      case CategorieAnimal.CHAT:
        return {
          label: 'Chat',
          icon: 'fas fa-cat',
          color: '#7b1fa2'
        };
      default:
        return {
          label: category,
          icon: 'fas fa-paw',
          color: '#666666'
        };
    }
  }

  // Méthodes de compatibilité (délèguent à getCategoryInfo)
  getCategoryIcon(category: CategorieAnimal): string {
    return this.getCategoryInfo(category).icon;
  }

  getCategoryLabel(category: CategorieAnimal): string {
    return this.getCategoryInfo(category).label;
  }

  getCategoryColor(category: CategorieAnimal): string {
    return this.getCategoryInfo(category).color;
  }
}
