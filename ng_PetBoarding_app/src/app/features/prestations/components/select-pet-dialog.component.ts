import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PetFormComponent } from '../../pets/components/pet-form.component';
import { Pet, PetType } from '../../pets/models/pet.model';
import { PetService } from '../../pets/services/pet.service';
import { Prestation } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';

@Component({
  selector: 'app-select-pet-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule
  ],
  templateUrl: './select-pet-dialog.component.html',
  styleUrl: './select-pet-dialog.component.scss'
})
export class SelectPetDialogComponent {
  private petService = inject(PetService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<SelectPetDialogComponent>);
  private prestationsService = inject(PrestationsService);
  private data = inject(MAT_DIALOG_DATA) as { prestation: Prestation };

  pets: Pet[] = [];
  isLoading = false;

  get prestation(): Prestation {
    return this.data.prestation;
  }

  constructor() {
    this.loadPets();
  }

  get filteredPets(): Pet[] {
    if (!this.pets) return [];
    const category = this.prestation.categorieAnimal;
    if (!category) return this.pets;

    return this.pets.filter((p) => p.type === category);
  }

  private loadPets(): void {
    this.isLoading = true;
    this.petService.loadUserPets();
    this.pets = this.petService.getPetsByType(this.prestation.categorieAnimal);
    this.isLoading = false;
  }

  selectPet(pet: Pet): void {
    this.dialogRef.close(pet);
  }

  openAddPet(): void {
    const dialogRef = this.dialog.open(PetFormComponent, {
      width: '700px'
    });

    dialogRef.afterClosed().subscribe(() => {
      this.loadPets();
    });
  }

  getPetAvatarClass(type: string): string {
    const petType = type === 'dog' ? PetType.DOG : type === 'cat' ? PetType.CAT : null;

    if (!petType) return 'chip-default';

    switch (petType) {
      case PetType.DOG:
        return 'chip-chien';
      case PetType.CAT:
        return 'chip-chat';
      default:
        return 'chip-default';
    }
  }

  getPetIcon(type: PetType): string {
    const categoryInfo = this.prestationsService.getCategoryInfo(type);
    return categoryInfo.icon || 'pets';
  }
}
