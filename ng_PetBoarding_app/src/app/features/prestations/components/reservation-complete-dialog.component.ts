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
import { PetFormComponent } from '../../pets/components/pet-form.component';
import { Pet, PetType } from '../../pets/models/pet.model';
import { PetService } from '../../pets/services/pet.service';
import { Prestation } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';
import {
  SelectionDatesComponent,
  SelectionDatesResult
} from './selection-dates/selection-dates.component';

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
    SelectionDatesComponent
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

  // État du composant
  pets = signal<Pet[]>([]);
  isLoading = signal(false);
  selectedPet = signal<Pet | null>(null);
  dateSelection = signal<SelectionDatesResult | null>(null);
  currentStep = signal(0);

  get prestation(): Prestation {
    return this.data.prestation;
  }

  // Computed pour vérifier si la réservation peut être confirmée
  canConfirm = computed(() => {
    const pet = this.selectedPet();
    const dates = this.dateSelection();
    return pet && dates && dates.estValide;
  });

  // Computed pour le résumé
  reservationSummary = computed(() => {
    const pet = this.selectedPet();
    const dates = this.dateSelection();

    if (!pet || !dates) return null;

    return {
      pet,
      dateDebut: dates.dateDebut,
      dateFin: dates.dateFin,
      nombreJours: dates.nombreJours,
      prixTotal: dates.prixTotal
    };
  });

  constructor() {
    this.loadPets();
  }

  private loadPets(): void {
    this.isLoading.set(true);
    try {
      this.petService.loadUserPets(); // à modifier.
      this.pets.set(this.petService.getPetsByType(this.prestation.categorieAnimal));
      this.isLoading.set(false);
    } catch {
      this.isLoading.set(false);
    }
  }

  onPetSelected(pet: Pet): void {
    this.selectedPet.set(pet);
    this.nextStep();
  }

  onDateSelectionChange(selection: SelectionDatesResult): void {
    this.dateSelection.set(selection);
  }

  onCreateNewPet(): void {
    const dialogRef = this.dialog.open(PetFormComponent, {
      width: '600px',
      data: {
        mode: 'create',
        compatibleType: this.prestation.categorieAnimal
      }
    });

    dialogRef.afterClosed().subscribe((newPet: Pet) => {
      if (newPet) {
        const currentPets = this.pets();
        this.pets.set([...currentPets, newPet]);
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
          dateDebut: dates.dateDebut,
          dateFin: dates.dateFin
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  getCategoryInfo() {
    return this.prestationsService.getCategoryInfo(this.prestation.categorieAnimal);
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
