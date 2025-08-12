import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';
import { PetsSectionComponent } from '../../pets/components/pets-section.component';
import { Pet } from '../../pets/models/pet.model';
import { ProfileService } from '../services/profile.service';
import { ProfileInfoComponent } from './profile-info.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    ProfileInfoComponent,
    PetsSectionComponent
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);

  // Getters pour les données
  currentUser = this.authService.currentUser;
  pets = this.profileService.pets;
  isLoading = this.profileService.isLoading;

  // Computed properties pour les stats
  totalPets = computed(() => this.pets().length);
  lastUpdate = computed(() => {
    const pets = this.pets();
    if (pets.length === 0) return null;

    const lastUpdated = pets.reduce((latest, pet) => {
      return pet.updatedAt > latest ? pet.updatedAt : latest;
    }, pets[0].updatedAt);

    return this.formatDate(lastUpdated);
  });

  ngOnInit(): void {
    this.loadUserPets();
  }

  /**
   * Charger les animaux de l'utilisateur
   */
  private loadUserPets(): void {
    this.profileService.loadUserPets().subscribe({
      error: () => {
        // Gestion d'erreur silencieuse pour l'instant
      }
    });
  }

  /**
   * Naviguer vers la modification du profil
   */
  editProfile(): void {
    // TODO: Implémenter la navigation vers la page d'édition
  }

  /**
   * Ajouter un nouvel animal
   */
  onAddPet(): void {
    this.router.navigate(['/profile/pets/add']);
  }

  /**
   * Modifier un animal
   */
  onEditPet(pet: Pet): void {
    // TODO: Implémenter la navigation vers l'édition d'animal
    // Utiliser pet.name pour éviter l'erreur unused
    void pet.name;
  }

  /**
   * Supprimer un animal
   */
  onDeletePet(pet: Pet): void {
    // TODO: Implémenter la suppression avec confirmation
    // Utiliser pet.name pour éviter l'erreur unused
    void pet.name;
  }

  /**
   * Voir les détails d'un animal
   */
  onViewPetDetails(pet: Pet): void {
    // TODO: Implémenter la vue détails d'animal
    // Utiliser pet.name pour éviter l'erreur unused
    void pet.name;
  }

  /**
   * Formater une date
   */
  private formatDate(date: Date): string {
    return new Intl.DateTimeFormat('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(new Date(date));
  }
}
