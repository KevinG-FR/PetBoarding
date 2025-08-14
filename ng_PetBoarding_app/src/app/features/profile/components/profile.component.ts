import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { PetsSectionComponent } from '../../pets/components/pets-section.component';
import { PetService } from '../../pets/services/pet.service';
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
  private readonly profileService = inject(ProfileService);
  private readonly petService = inject(PetService);
  private readonly router = inject(Router);

  // Getters pour les données
  currentUser = this.profileService.currentUser;
  pets = this.petService.pets;
  isLoading = this.petService.isLoading;

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
    this.petService.loadUserPets().subscribe({
      error: () => {
        // Gestion d'erreur silencieuse pour l'instant
      }
    });
  }

  /**
   * Naviguer vers la modification du profil
   */
  editProfile(): void {
    this.router.navigate(['/profile/edit']);
  }

  /**
   * Ajouter un nouvel animal
   */
  onAddPet(): void {
    this.router.navigate(['/profile/pets/add']);
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
