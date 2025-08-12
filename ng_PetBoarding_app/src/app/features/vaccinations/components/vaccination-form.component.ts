import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

import { Vaccination } from '../models/vaccination';
import { VaccinationFormData } from '../models/vaccination-form.model';

@Component({
  selector: 'app-vaccination-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './vaccination-form.component.html',
  styleUrl: './vaccination-form.component.scss'
})
export class VaccinationFormComponent implements OnInit {
  @Input() vaccination?: Vaccination; // Pour l'édition
  @Input() isLoading = false;
  @Output() formSubmit = new EventEmitter<VaccinationFormData>();
  @Output() formCancel = new EventEmitter<void>();

  vaccinationForm!: FormGroup;
  isEditMode = false;

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.isEditMode = !!this.vaccination;
    this.initializeForm();
  }

  private initializeForm(): void {
    this.vaccinationForm = this.fb.group({
      name: [this.vaccination?.name || '', [Validators.required, Validators.minLength(2)]],
      date: [this.vaccination?.date || '', Validators.required],
      expiryDate: [this.vaccination?.expiryDate || ''],
      veterinarian: [
        this.vaccination?.veterinarian || '',
        [Validators.required, Validators.minLength(2)]
      ],
      batchNumber: [this.vaccination?.batchNumber || '']
    });
  }

  onSubmit(): void {
    if (this.vaccinationForm.valid) {
      const formData: VaccinationFormData = this.vaccinationForm.value;
      this.formSubmit.emit(formData);
    } else {
      this.markFormGroupTouched();
    }
  }

  onCancel(): void {
    this.formCancel.emit();
  }

  private markFormGroupTouched(): void {
    Object.keys(this.vaccinationForm.controls).forEach((key) => {
      const control = this.vaccinationForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.vaccinationForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) return 'Ce champ est requis';
      if (control.errors['minlength'])
        return `Minimum ${control.errors['minlength'].requiredLength} caractères`;
    }
    return '';
  }
}
