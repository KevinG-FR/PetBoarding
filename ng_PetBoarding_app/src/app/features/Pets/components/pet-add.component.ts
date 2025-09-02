import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { PetFormData } from '../models/pet-form.model';
import { PetService } from '../services/pet.service';
import { PetFormComponent } from './pet-form.component';

@Component({
  selector: 'app-pet-add',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, PetFormComponent],
  templateUrl: './pet-add.component.html',
  styleUrl: './pet-add.component.scss'
})
export class PetAddComponent {
  private readonly petService = inject(PetService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);

  isLoading = false;

  onFormSubmit(petData: PetFormData): void {
    this.isLoading = true;

    this.petService.addPet(petData).subscribe({
      next: (newPet) => {
        this.isLoading = false;
        if (newPet) {
          this.snackBar.open(`${newPet.name} a été ajouté avec succès !`, 'Fermer', {
            duration: 5000
          });
          this.router.navigate(['/profile']);
        } else {
          this.snackBar.open("Erreur lors de la création de l'animal", 'Fermer', {
            duration: 5000
          });
        }
      },
      error: (_) => {
        this.isLoading = false;
        this.snackBar.open("Erreur lors de la création de l'animal", 'Fermer', { duration: 5000 });
      }
    });
  }

  onFormCancel(): void {
    this.router.navigate(['/profile']);
  }
}
