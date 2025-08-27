import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { PetFormData } from '../../pets/models/pet-form.model';
import {
  Pet,
  PetGender,
  PetGenderLabels,
  PetType,
  PetTypeLabels
} from '../../pets/models/pet.model';
import { VaccinationFormData } from '../../vaccinations';
import { VaccinationFormComponent } from '../../vaccinations/components/vaccination-form.component';
import { VaccinationListComponent } from '../../vaccinations/components/vaccination-list.component';
import { Vaccination } from '../../vaccinations/models/vaccination';

@Component({
  selector: 'app-pet-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatProgressSpinnerModule,
    VaccinationListComponent,
    VaccinationFormComponent
  ],
  templateUrl: './pet-form.component.html',
  styleUrl: './pet-form.component.scss'
})
export class PetFormComponent implements OnInit {
  @Input() pet?: Pet; // Pour l'édition
  @Input() isLoading = false;
  @Input() compatibleType?: PetType; // Type d'animal compatible pour les réservations
  @Output() formSubmit = new EventEmitter<PetFormData>();
  @Output() formCancel = new EventEmitter<void>();

  petForm!: FormGroup;
  petTypes = Object.values(PetType);
  petTypeLabels = PetTypeLabels;
  petGenders = Object.values(PetGender);
  petGenderLabels = PetGenderLabels;

  isEditMode = signal(false);
  vaccinations = signal<Vaccination[]>([]);
  showVaccinationForm = signal(false);
  editingVaccination = signal<Vaccination | undefined>(undefined);

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.isEditMode.set(!!this.pet);
    if (this.pet?.vaccinations) {
      this.vaccinations.set([...this.pet.vaccinations]);
    }
    this.initializeForm();
  }

  private initializeForm(): void {
    this.petForm = this.fb.group({
      name: [this.pet?.name || '', [Validators.required, Validators.minLength(2)]],
      type: [this.pet?.type || this.compatibleType || '', Validators.required],
      breed: [this.pet?.breed || '', [Validators.required, Validators.minLength(2)]],
      age: [this.pet?.age || '', [Validators.required, Validators.min(0), Validators.max(30)]],
      weight: [this.pet?.weight || ''],
      color: [this.pet?.color || '', [Validators.required, Validators.minLength(2)]],
      gender: [this.pet?.gender || '', Validators.required],
      isNeutered: [this.pet?.isNeutered || false],
      microchipNumber: [this.pet?.microchipNumber || ''],
      medicalNotes: [this.pet?.medicalNotes || ''],
      specialNeeds: [this.pet?.specialNeeds || ''],
      photoUrl: [this.pet?.photoUrl || ''],
      emergencyContact: this.fb.group({
        name: [this.pet?.emergencyContact?.name || ''],
        phone: [this.pet?.emergencyContact?.phone || ''],
        relationship: [this.pet?.emergencyContact?.relationship || '']
      })
    });
  }

  onSubmit(): void {
    if (this.petForm.valid) {
      const formData: PetFormData = {
        ...this.petForm.value,
        vaccinations: this.vaccinations()
      };
      this.formSubmit.emit(formData);
    } else {
      this.markFormGroupTouched();
    }
  }

  onCancel(): void {
    this.formCancel.emit();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.petForm.controls).forEach((key) => {
      const control = this.petForm.get(key);
      control?.markAsTouched();

      if (control && 'controls' in control) {
        const formGroup = control as FormGroup;
        Object.keys(formGroup.controls).forEach((nestedKey) => {
          control.get(nestedKey)?.markAsTouched();
        });
      }
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.petForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Ce champ est requis';
      if (control.errors['minlength'])
        return `Minimum ${control.errors['minlength'].requiredLength} caractères`;
      if (control.errors['min'])
        return `La valeur doit être supérieure à ${control.errors['min'].min}`;
      if (control.errors['max'])
        return `La valeur doit être inférieure à ${control.errors['max'].max}`;
    }
    return '';
  }

  getEmergencyContactError(fieldName: string): string {
    const control = this.petForm.get(`emergencyContact.${fieldName}`);
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Ce champ est requis';
      if (control.errors['minlength'])
        return `Minimum ${control.errors['minlength'].requiredLength} caractères`;
    }
    return '';
  }

  // Méthodes pour gérer les vaccinations
  onAddVaccination(): void {
    this.editingVaccination.set(undefined);
    this.showVaccinationForm.set(true);
  }

  onEditVaccination(vaccination: Vaccination): void {
    this.editingVaccination.set(vaccination);
    this.showVaccinationForm.set(true);
  }

  onDeleteVaccination(vaccination: Vaccination): void {
    const updatedVaccinations = this.vaccinations().filter((v) => v.id !== vaccination.id);
    this.vaccinations.set(updatedVaccinations);
  }

  onVaccinationFormSubmit(formData: VaccinationFormData): void {
    const currentVaccinations = this.vaccinations();
    const editingVacc = this.editingVaccination();

    if (editingVacc) {
      // Modification
      const updatedVaccinations = currentVaccinations.map((v) =>
        v.id === editingVacc.id ? { ...v, ...formData } : v
      );
      this.vaccinations.set(updatedVaccinations);
    } else {
      // Ajout
      const newVaccination: Vaccination = {
        id: Date.now().toString(),
        ...formData
      };
      this.vaccinations.set([...currentVaccinations, newVaccination]);
    }

    this.showVaccinationForm.set(false);
    this.editingVaccination.set(undefined);
  }

  onVaccinationFormCancel(): void {
    this.showVaccinationForm.set(false);
    this.editingVaccination.set(undefined);
  }
}
