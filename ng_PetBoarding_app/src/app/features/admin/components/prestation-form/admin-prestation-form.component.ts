import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { PrestationsService } from '../../../prestations/services/prestations.service';
import { Prestation } from '../../../../shared/models/prestation.model';
import { PetType, PetTypeLabels } from '../../../pets/models/pet.model';

@Component({
  selector: 'app-admin-prestation-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './admin-prestation-form.component.html',
  styleUrl: './admin-prestation-form.component.scss'
})
export class AdminPrestationFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly prestationsService = inject(PrestationsService);
  private readonly snackBar = inject(MatSnackBar);

  prestationForm: FormGroup;
  isEditMode = false;
  prestationId: string | null = null;
  isLoading = signal(false);
  isSubmitting = signal(false);

  petTypes = Object.values(PetType).map(type => ({
    value: type,
    label: PetTypeLabels[type]
  }));

  constructor() {
    this.prestationForm = this.fb.group({
      libelle: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      categorieAnimal: [null, [Validators.required]],
      prix: [null, [Validators.required, Validators.min(0)]],
      duree: [null, [Validators.required, Validators.min(1)]],
      disponible: [true]
    });
  }

  ngOnInit(): void {
    this.prestationId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.prestationId && this.router.url.includes('/edit');

    if (this.isEditMode && this.prestationId) {
      this.loadPrestation(this.prestationId);
    }
  }

  loadPrestation(id: string): void {
    this.isLoading.set(true);

    this.prestationsService.getPrestationById(id).subscribe({
      next: (prestation) => {
        if (prestation) {
          this.prestationForm.patchValue({
            libelle: prestation.libelle,
            description: prestation.description,
            categorieAnimal: prestation.categorieAnimal,
            prix: prestation.prix,
            duree: prestation.duree,
            disponible: prestation.disponible
          });
        }
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erreur lors du chargement:', error);
        this.snackBar.open('Erreur lors du chargement de la prestation', 'Fermer', {
          duration: 5000
        });
        this.isLoading.set(false);
        this.goBack();
      }
    });
  }

  onSubmit(): void {
    if (this.prestationForm.valid) {
      this.isSubmitting.set(true);

      if (this.isEditMode && this.prestationId) {
        this.updatePrestation();
      } else {
        this.createPrestation();
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  private createPrestation(): void {
    const formValue = this.prestationForm.value;
    const prestation: Omit<Prestation, 'id'> = {
      libelle: formValue.libelle.trim(),
      description: formValue.description.trim(),
      categorieAnimal: formValue.categorieAnimal,
      prix: formValue.prix,
      duree: formValue.duree,
      disponible: formValue.disponible
    };

    this.prestationsService.createPrestation(prestation).subscribe({
      next: (success) => {
        if (success) {
          this.snackBar.open('Prestation créée avec succès', 'Fermer', {
            duration: 3000
          });
          this.router.navigate(['/admin/prestations']);
        } else {
          this.snackBar.open('Erreur lors de la création de la prestation', 'Fermer', {
            duration: 5000
          });
        }
        this.isSubmitting.set(false);
      },
      error: (error) => {
        console.error('Erreur lors de la création:', error);
        this.snackBar.open('Erreur lors de la création de la prestation', 'Fermer', {
          duration: 5000
        });
        this.isSubmitting.set(false);
      }
    });
  }

  private updatePrestation(): void {
    if (!this.prestationId) return;

    const formValue = this.prestationForm.value;
    const updates: Partial<Prestation> = {
      libelle: formValue.libelle.trim(),
      description: formValue.description.trim(),
      categorieAnimal: formValue.categorieAnimal,
      prix: formValue.prix,
      duree: formValue.duree,
      disponible: formValue.disponible
    };

    this.prestationsService.updatePrestation(this.prestationId, updates).subscribe({
      next: (success) => {
        if (success) {
          this.snackBar.open('Prestation modifiée avec succès', 'Fermer', {
            duration: 3000
          });
          this.router.navigate(['/admin/prestations', this.prestationId]);
        } else {
          this.snackBar.open('Erreur lors de la modification de la prestation', 'Fermer', {
            duration: 5000
          });
        }
        this.isSubmitting.set(false);
      },
      error: (error) => {
        console.error('Erreur lors de la modification:', error);
        this.snackBar.open('Erreur lors de la modification de la prestation', 'Fermer', {
          duration: 5000
        });
        this.isSubmitting.set(false);
      }
    });
  }

  private markFormGroupTouched(): void {
    Object.keys(this.prestationForm.controls).forEach(key => {
      const control = this.prestationForm.get(key);
      control?.markAsTouched();
    });
  }

  goBack(): void {
    if (this.isEditMode && this.prestationId) {
      this.router.navigate(['/admin/prestations', this.prestationId]);
    } else {
      this.router.navigate(['/admin/prestations']);
    }
  }
}