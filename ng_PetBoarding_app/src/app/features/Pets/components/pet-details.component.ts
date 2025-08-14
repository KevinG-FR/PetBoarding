import { CommonModule } from '@angular/common';
import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ActivatedRoute, Router } from '@angular/router';

import { ProfileService } from '../../profile/services/profile.service';
import { Pet, PetGender, PetGenderLabels, PetType, PetTypeLabels } from '../models/pet.model';

interface ViewMode {
  isEditing: boolean;
  hasUnsavedChanges: boolean;
}

@Component({
  selector: 'app-pet-details',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatTooltipModule,
    MatDialogModule
  ],
  templateUrl: './pet-details.component.html',
  styleUrl: './pet-details.component.scss'
})
export class PetDetailsComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private profileService = inject(ProfileService);

  // Signals
  pet = signal<Pet | null>(null);
  loading = signal(true);
  viewMode = signal<ViewMode>({ isEditing: false, hasUnsavedChanges: false });
  originalFormValue = signal<Record<string, unknown>>({});

  // Form
  petForm: FormGroup;

  // Labels
  petTypeLabels = PetTypeLabels;
  petGenderLabels = PetGenderLabels;
  petTypes = Object.values(PetType);
  petGenders = Object.values(PetGender);

  // Computed
  isEditing = computed(() => this.viewMode().isEditing);
  hasUnsavedChanges = computed(() => this.viewMode().hasUnsavedChanges);

  constructor() {
    this.petForm = this.createForm();

    // Effet pour charger l'animal depuis l'URL
    effect(() => {
      const petId = this.route.snapshot.paramMap.get('id');
      const isEditRoute = this.route.snapshot.url.some((segment) => segment.path === 'edit');

      if (petId) {
        this.loading.set(true);

        // S'assurer que les animaux sont chargés d'abord
        if (this.profileService.pets().length === 0) {
          this.profileService.loadUserPets().subscribe({
            next: () => {
              this.loadPetDetails(petId, isEditRoute);
            },
            error: () => {
              this.handleLoadError();
            }
          });
        } else {
          this.loadPetDetails(petId, isEditRoute);
        }
      }
    });

    // Effet pour surveiller les changements du formulaire
    effect(() => {
      if (this.isEditing()) {
        this.petForm.valueChanges.subscribe(() => {
          this.checkForChanges();
        });
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      type: ['', Validators.required],
      breed: ['', Validators.required],
      age: [0, [Validators.required, Validators.min(0), Validators.max(30)]],
      weight: [null, [Validators.min(0.1), Validators.max(100)]],
      color: ['', Validators.required],
      gender: ['', Validators.required],
      isNeutered: [false],
      microchipNumber: [''],
      medicalNotes: [''],
      specialNeeds: [''],
      emergencyContact: this.fb.group({
        name: [''],
        phone: [''],
        relationship: ['']
      })
    });
  }

  private loadPetDetails(petId: string, isEditRoute: boolean): void {
    this.profileService.getPetById(petId).subscribe({
      next: (pet) => {
        if (pet) {
          this.pet.set(pet);
          this.initializeForm(pet);
          this.viewMode.set({
            isEditing: isEditRoute,
            hasUnsavedChanges: false
          });
        } else {
          this.router.navigate(['/profile']);
        }
        this.loading.set(false);
      },
      error: () => {
        this.handleLoadError();
      }
    });
  }

  private handleLoadError(): void {
    this.snackBar.open("Erreur lors du chargement de l'animal", 'Fermer', {
      duration: 3000
    });
    this.router.navigate(['/profile']);
    this.loading.set(false);
  }

  private initializeForm(pet: Pet): void {
    this.petForm.patchValue({
      name: pet.name,
      type: pet.type,
      breed: pet.breed,
      age: pet.age,
      weight: pet.weight,
      color: pet.color,
      gender: pet.gender,
      isNeutered: pet.isNeutered,
      microchipNumber: pet.microchipNumber || '',
      medicalNotes: pet.medicalNotes || '',
      specialNeeds: pet.specialNeeds || '',
      emergencyContact: {
        name: pet.emergencyContact?.name || '',
        phone: pet.emergencyContact?.phone || '',
        relationship: pet.emergencyContact?.relationship || ''
      }
    });

    this.originalFormValue.set(this.petForm.value);
  }

  private checkForChanges(): void {
    const currentValue = this.petForm.value;
    const originalValue = this.originalFormValue();
    const hasChanges = JSON.stringify(currentValue) !== JSON.stringify(originalValue);

    this.viewMode.update((mode) => ({
      ...mode,
      hasUnsavedChanges: hasChanges
    }));
  }

  onEditMode(): void {
    this.viewMode.update((mode) => ({ ...mode, isEditing: true }));
  }

  onCancelEdit(): void {
    if (this.hasUnsavedChanges()) {
      // Ouvrir une dialog de confirmation
      const confirmDialog = this.dialog.open(ConfirmCancelDialog);

      confirmDialog.afterClosed().subscribe((result) => {
        if (result) {
          this.cancelEdit();
        }
      });
    } else {
      this.cancelEdit();
    }
  }

  private cancelEdit(): void {
    this.petForm.patchValue(this.originalFormValue());
    this.viewMode.set({ isEditing: false, hasUnsavedChanges: false });
  }

  onSave(): void {
    if (this.petForm.valid && this.pet()) {
      const petId = this.pet()!.id;
      const formValue = this.petForm.value;

      this.profileService.updatePet(petId, formValue).subscribe({
        next: (updatedPet) => {
          this.pet.set(updatedPet);
          this.originalFormValue.set(this.petForm.value);
          this.viewMode.set({ isEditing: false, hasUnsavedChanges: false });

          this.snackBar.open('Modifications sauvegardées avec succès', 'Fermer', {
            duration: 3000
          });
        },
        error: () => {
          this.snackBar.open('Erreur lors de la sauvegarde', 'Fermer', {
            duration: 3000
          });
        }
      });
    } else {
      this.snackBar.open('Veuillez corriger les erreurs dans le formulaire', 'Fermer', {
        duration: 3000
      });
    }
  }

  onBack(): void {
    if (this.hasUnsavedChanges()) {
      const confirmDialog = this.dialog.open(ConfirmCancelDialog, {
        data: {
          message: 'Vous avez des modifications non sauvegardées. Voulez-vous vraiment quitter ?'
        }
      });

      confirmDialog.afterClosed().subscribe((result) => {
        if (result) {
          this.router.navigate(['/profile']);
        }
      });
    } else {
      this.router.navigate(['/profile']);
    }
  }

  getPetIcon(type: PetType): string {
    const icons = {
      [PetType.DOG]: 'pets',
      [PetType.CAT]: 'pets',
      [PetType.BIRD]: 'flutter_dash',
      [PetType.RABBIT]: 'cruelty_free',
      [PetType.HAMSTER]: 'pets'
    };
    return icons[type] || 'pets';
  }

  getAgeDisplay(age: number): string {
    if (age === 0) return "Moins d'un an";
    if (age === 1) return '1 an';
    return `${age} ans`;
  }

  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(new Date(date));
  }
}

// Composant Dialog pour confirmation d'annulation
@Component({
  selector: 'app-confirm-cancel-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Confirmer l'annulation</h2>
    <mat-dialog-content>
      <p>Vous avez des modifications non sauvegardées. Voulez-vous vraiment annuler ?</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close="false">Continuer l'édition</button>
      <button mat-button mat-dialog-close="true" color="warn">Annuler les modifications</button>
    </mat-dialog-actions>
  `
})
export class ConfirmCancelDialog {}
