import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
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
import { MatStepperModule } from '@angular/material/stepper';
import { MatTabsModule } from '@angular/material/tabs';
import { PetDialogComponent } from '../../pets/components/pet-dialog.component';
import { Pet, PetType } from '../../pets/models/pet.model';
import { PetService } from '../../pets/services/pet.service';
import { DateSelectionResult } from '../models/DateSelectionResult';
import { Prestation } from '../models/prestation.model';
import { CategoryInfo, PrestationsService } from '../services/prestations.service';
import { DateSelectionComponent } from './selection-dates.component';

export interface ReservationCompleteResult {
  pet: Pet;
  dateDebut: Date;
  dateFin?: Date;
  nombreJours: number;
  prixTotal: number;
}

@Component({
  selector: 'app-reservation-complete-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatStepperModule,
    MatTabsModule,
    DateSelectionComponent
  ],
  templateUrl: './reservation-complete-dialog.component.html',
  styleUrl: './reservation-complete-dialog.component.scss'
})
export class ReservationCompleteDialogComponent {
  private petService = inject(PetService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<ReservationCompleteDialogComponent>);
  private prestationsService = inject(PrestationsService);
  private data = inject(MAT_DIALOG_DATA) as { prestation: Prestation };

  // Computed signal pour filtrer les animaux compatibles depuis le service
  pets = computed(() => {
    const allPets = this.petService.pets();
    const filteredPets = allPets.filter(pet => pet.type === this.prestation.categorieAnimal);
    if (filteredPets.length === 0 && allPets.length > 0) {
      console.log('DEBUG - No compatible pets found:', {
        allPetsCount: allPets.length,
        prestationType: this.prestation.categorieAnimal,
        allPetsTypes: allPets.map(p => p.type),
        filteredCount: filteredPets.length
      });
    }
    return filteredPets;
  });
  isLoading = computed(() => this.petService.isLoading());
  selectedPet = signal<Pet | null>(null);
  dateSelection = signal<DateSelectionResult | null>(null);
  currentStep = signal(0);

  get prestation(): Prestation {
    return this.data.prestation;
  }

  canConfirm = computed(() => {
    const pet = this.selectedPet();
    const dates = this.dateSelection();
    return pet && dates && dates.isValid;
  });

  reservationSummary = computed(() => {
    const pet = this.selectedPet();
    const dates = this.dateSelection();

    if (!pet || !dates) return null;

    return {
      pet,
      dateDebut: dates.startDate,
      dateFin: dates.endDate,
      nombreJours: dates.numberOfDays,
      prixTotal: dates.totalPrice
    };
  });

  constructor() {
    // Toujours charger les pets à l'ouverture du dialog pour s'assurer d'avoir les données les plus récentes
    this.petService.loadUserPets().subscribe({
      next: (pets) => {
        console.log('Pets chargés:', pets);
      },
      error: (error) => {
        console.error('Erreur lors du chargement des pets:', error);
      }
    });
  }

  onPetSelected(pet: Pet): void {
    this.selectedPet.set(pet);
    this.nextStep();
  }

  onDateSelectionChange(selection: DateSelectionResult): void {
    this.dateSelection.set(selection);
  }

  onCreateNewPet(): void {
    const dialogRef = this.dialog.open(PetDialogComponent, {
      width: '600px',
      data: {
        mode: 'create',
        compatibleType: this.prestation.categorieAnimal
      }
    });

    dialogRef.afterClosed().subscribe((newPet: Pet) => {
      if (newPet) {
        // Le service se charge automatiquement de recharger les pets après création
        this.selectedPet.set(newPet);
        this.nextStep();
      }
    });
  }

  nextStep(): void {
    if (this.currentStep() < 1) {
      this.currentStep.set(this.currentStep() + 1);
    }
  }

  previousStep(): void {
    if (this.currentStep() > 0) {
      this.currentStep.set(this.currentStep() - 1);
      this.dateSelection.set(null);
    }
  }

  onConfirm(): void {
    const pet = this.selectedPet();
    const dates = this.dateSelection();

    if (pet && dates) {
      this.dialogRef.close({
        action: 'reserve',
        pet: pet,
        dates: {
          dateDebut: dates.startDate,
          dateFin: dates.endDate
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getCategoryInfo(): CategoryInfo {
    console.log('getCategoryInfo - prestation.categorieAnimal:', this.prestation.categorieAnimal, typeof this.prestation.categorieAnimal);
    const categoryInfo = this.prestationsService.getCategoryInfo(this.prestation.categorieAnimal);
    console.log('getCategoryInfo - result:', categoryInfo);
    return categoryInfo;
  }

  getPetIcon(petType: PetType): string {
    switch (petType) {
      case PetType.DOG:
        return 'fas fa-dog';
      case PetType.CAT:
        return 'fas fa-cat';
      case PetType.BIRD:
        return 'fas fa-dove';
      case PetType.RABBIT:
        return 'fas fa-carrot';
      case PetType.HAMSTER:
        return 'fas fa-paw';
      default:
        return 'fas fa-paw';
    }
  }
}
