import { CommonModule } from '@angular/common';
import { Component, inject, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
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
  @Input() prestation!: Prestation;

  private petService = inject(PetService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<SelectPetDialogComponent>);
  private prestationsService = inject(PrestationsService);

  pets: Pet[] = [];
  isLoading = false;

  constructor() {
    this.loadPets();
  }

  get filteredPets() {
    if (!this.pets) return [];
    const category = this.prestation?.categorieAnimal;
    if (!category) return this.pets;

    return this.pets.filter((p) => p.type === category);
  }

  private loadPets(): void {
    this.isLoading = true;
    this.petService.loadUserPets();
    this.pets = this.petService.pets();
    this.isLoading = false;
  }

  selectPet(pet: Pet) {
    // Close this dialog and return the selected pet as result
    this.dialogRef.close(pet);
  }

  openAddPet() {
    const dialogRef = this.dialog.open(PetFormComponent, {
      width: '700px'
    });

    dialogRef.afterClosed().subscribe(() => {
      // reload pets after closing form
      this.loadPets();
    });
  }

  getPetAvatarClass(type: string): string {
    // Map pet type string to PetType enum for consistent styling
    const petType = type === 'dog' ? PetType.DOG : type === 'cat' ? PetType.CAT : null;

    if (!petType) return 'chip-default';

    // Use same logic as PrestationItemComponent
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
