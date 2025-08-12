import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

import { PetFormData } from '../../../Pets/models/pet-form.model';
import { ProfileService } from '../../services/profile.service';
import { PetFormComponent } from './pet-form.component';

@Component({
  selector: 'app-add-pet',
  standalone: true,
  imports: [PetFormComponent],
  template: `
    <app-pet-form
      [isLoading]="profileService.isLoading()"
      (formSubmit)="onAddPet($event)"
      (formCancel)="onCancel()" />
  `
})
export class AddPetComponent {
  private readonly router = inject(Router);
  protected readonly profileService = inject(ProfileService);

  onAddPet(formData: PetFormData): void {
    // Ajouter un tableau vide de vaccinations pour respecter l'interface Pet
    const petData = {
      ...formData,
      vaccinations: []
    };

    this.profileService.addPet(petData).subscribe({
      next: () => {
        // Rediriger vers le profil après ajout réussi
        this.router.navigate(['/profile']);
      },
      error: (error) => {
        // eslint-disable-next-line no-console
        console.error("Erreur lors de l'ajout de l'animal:", error);
        // TODO: Afficher un message d'erreur à l'utilisateur
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/profile']);
  }
}
