import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { PetFormData } from '../models/pet-form.model';
import { Pet, PetType } from '../models/pet.model';
import { PetService } from '../services/pet.service';
import { PetFormComponent } from './pet-form.component';

export interface PetDialogData {
  mode: 'create' | 'edit';
  pet?: Pet;
  compatibleType?: PetType;
}

@Component({
  selector: 'app-pet-dialog',
  standalone: true,
  imports: [MatDialogModule, PetFormComponent],
  template: `
    <h2 mat-dialog-title>
      {{ data.mode === 'create' ? 'Ajouter un animal' : 'Modifier l\'animal' }}
    </h2>
    
    <mat-dialog-content>
      <app-pet-form
        [pet]="data.pet"
        [isLoading]="petService.isLoading()"
        [compatibleType]="data.compatibleType"
        (formSubmit)="onFormSubmit($event)"
        (formCancel)="onCancel()">
      </app-pet-form>
    </mat-dialog-content>
  `
})
export class PetDialogComponent {
  private dialogRef = inject(MatDialogRef<PetDialogComponent>);
  protected petService = inject(PetService);
  protected data = inject(MAT_DIALOG_DATA) as PetDialogData;

  onFormSubmit(formData: PetFormData): void {
    if (this.data.mode === 'create') {
      this.petService.addPet(formData).subscribe({
        next: (newPet) => {
          if (newPet) {
            this.dialogRef.close(newPet);
          }
        },
        error: (error) => {
          console.error('Erreur lors de la création de l\'animal:', error);
        }
      });
    } else if (this.data.pet) {
      this.petService.updatePet(this.data.pet.id, formData).subscribe({
        next: (updatedPet) => {
          if (updatedPet) {
            this.dialogRef.close(updatedPet);
          }
        },
        error: (error) => {
          console.error('Erreur lors de la mise à jour de l\'animal:', error);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}