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

import { PetFormData } from '../models/pet-form.model';
import { Pet, PetGender, PetGenderLabels, PetType, PetTypeLabels } from '../models/pet.model';

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
    MatProgressSpinnerModule
  ],
  templateUrl: './pet-form.component.html',
  styleUrl: './pet-form.component.scss'
})
export class PetFormComponent implements OnInit {
  @Input() pet?: Pet; // Pour l'édition
  @Input() isLoading = false;
  @Output() formSubmit = new EventEmitter<PetFormData>();
  @Output() formCancel = new EventEmitter<void>();

  petForm!: FormGroup;
  petTypes = Object.values(PetType);
  petTypeLabels = PetTypeLabels;
  petGenders = Object.values(PetGender);
  petGenderLabels = PetGenderLabels;

  isEditMode = signal(false);

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.isEditMode.set(!!this.pet);
    this.initializeForm();
  }

  private initializeForm(): void {
    this.petForm = this.fb.group({
      name: [this.pet?.name || '', [Validators.required, Validators.minLength(2)]],
      type: [this.pet?.type || '', Validators.required],
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
      const formData: PetFormData = this.petForm.value;
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
}
