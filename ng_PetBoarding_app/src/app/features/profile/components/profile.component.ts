import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';

import { AuthService } from '../../auth/services/auth.service';
import { Pet, PetGenderLabels, PetTypeLabels } from '../models/pet.model';
import { ProfileService } from '../services/profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTooltipModule,
    MatBadgeModule
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly profileService = inject(ProfileService);

  // Signals pour l'état du composant
  isLoadingPets = signal(false);

  // Getters pour les données
  currentUser = this.authService.currentUser;
  pets = this.profileService.pets;
  isLoading = this.profileService.isLoading;

  // Labels pour les enums
  petTypeLabels = PetTypeLabels;
  petGenderLabels = PetGenderLabels;

  // Computed properties
  totalPets = computed(() => this.pets().length);

  petsByType = computed(() => {
    const pets = this.pets();
    const types = new Map<string, number>();

    pets.forEach((pet) => {
      const typeLabel = this.petTypeLabels[pet.type];
      types.set(typeLabel, (types.get(typeLabel) || 0) + 1);
    });

    return Array.from(types.entries()).map(([type, count]) => ({ type, count }));
  });

  ngOnInit(): void {
    this.loadUserPets();
  }

  /**
   * Charger les animaux de l'utilisateur
   */
  private loadUserPets(): void {
    this.isLoadingPets.set(true);

    this.profileService.loadUserPets().subscribe({
      next: () => {
        this.isLoadingPets.set(false);
      },
      error: (error) => {
        // eslint-disable-next-line no-console
        console.error('Erreur lors du chargement des animaux:', error);
        this.isLoadingPets.set(false);
      }
    });
  }

  /**
   * Naviguer vers la modification du profil
   */
  editProfile(): void {
    // TODO: Implémenter la navigation vers la page d'édition
    // eslint-disable-next-line no-console
    console.log('Navigation vers édition du profil');
  }

  /**
   * Ajouter un nouvel animal
   */
  addPet(): void {
    // TODO: Implémenter la navigation vers l'ajout d'animal
    // eslint-disable-next-line no-console
    console.log("Navigation vers ajout d'animal");
  }

  /**
   * Modifier un animal
   */
  editPet(pet: Pet): void {
    // TODO: Implémenter la navigation vers l'édition d'animal
    // eslint-disable-next-line no-console
    console.log('Navigation vers édition animal:', pet.name);
  }

  /**
   * Voir les détails d'un animal
   */
  viewPetDetails(pet: Pet): void {
    // TODO: Implémenter la vue détails d'animal
    // eslint-disable-next-line no-console
    console.log('Voir détails animal:', pet.name);
  }

  /**
   * Calculer l'âge en texte
   */
  getAgeText(age: number): string {
    if (age === 0) return "Moins d'un an";
    if (age === 1) return '1 an';
    return `${age} ans`;
  }

  /**
   * Obtenir l'icône selon le type d'animal
   */
  getPetIcon(type: string): string {
    const iconMap: Record<string, string> = {
      dog: 'pets',
      cat: 'pets',
      bird: 'flutter_dash',
      rabbit: 'cruelty_free',
      hamster: 'pets',
      fish: 'water',
      reptile: 'pets',
      other: 'pets'
    };

    return iconMap[type] || 'pets';
  }

  /**
   * Vérifier si les vaccins sont à jour
   */
  areVaccinationsUpToDate(pet: Pet): boolean {
    const now = new Date();
    return pet.vaccinations.every((vaccination) => {
      if (!vaccination.expiryDate) return true;
      return vaccination.expiryDate > now;
    });
  }

  /**
   * Obtenir le statut des vaccinations
   */
  getVaccinationStatus(pet: Pet): { status: string; class: string; icon: string } {
    const isUpToDate = this.areVaccinationsUpToDate(pet);

    if (isUpToDate) {
      return {
        status: 'À jour',
        class: 'status-success',
        icon: 'check_circle'
      };
    } else {
      return {
        status: 'Attention',
        class: 'status-warning',
        icon: 'warning'
      };
    }
  }
}
