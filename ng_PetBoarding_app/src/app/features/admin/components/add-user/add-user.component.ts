import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatStepperModule } from '@angular/material/stepper';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UserService } from '../../services/user.service';
import { ProfileType } from '../../../../shared/enums/profile-type.enum';

@Component({
  selector: 'app-add-user',
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
    MatStepperModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  saving = signal(false);
  
  readonly ProfileType = ProfileType;
  readonly adminProfileTypes = [ProfileType.Administrator, ProfileType.Employee];

  personalInfoForm: FormGroup;
  addressForm: FormGroup;
  passwordForm: FormGroup;

  constructor() {
    this.personalInfoForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      profileType: ['', [Validators.required]]
    });

    this.addressForm = this.fb.group({
      streetNumber: [''],
      streetName: [''],
      city: [''],
      postalCode: [''],
      country: ['France'],
      complement: ['']
    });

    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    
    return null;
  }

  onSubmit(): void {
    if (this.personalInfoForm.valid && this.passwordForm.valid) {
      this.saving.set(true);
      
      const personalInfo = this.personalInfoForm.value;
      const addressInfo = this.addressForm.value;
      const passwordInfo = this.passwordForm.value;
      
      const hasAddress = Object.values(addressInfo).some(value => value && value.toString().trim());
      
      const createUserRequest = {
        ...personalInfo,
        password: passwordInfo.password,
        address: hasAddress ? addressInfo : undefined
      };

      this.userService.createUser(createUserRequest).subscribe({
        next: (createdUser) => {
          this.saving.set(false);
          this.router.navigate(['/admin/users', createdUser.id]);
        },
        error: (error) => {
          console.error('Error creating user:', error);
          this.saving.set(false);
          alert('Erreur lors de la création de l\'utilisateur');
        }
      });
    } else {
      this.markAllFormGroupsTouched();
    }
  }

  onCancel(): void {
    this.router.navigate(['/admin/users']);
  }

  markAllFormGroupsTouched(): void {
    this.personalInfoForm.markAllAsTouched();
    this.addressForm.markAllAsTouched();
    this.passwordForm.markAllAsTouched();
  }

  getPersonalInfoErrorMessage(fieldName: string): string {
    const field = this.personalInfoForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `Le champ ${fieldName} est requis`;
      if (field.errors['email']) return 'Email invalide';
      if (field.errors['minlength']) return `Minimum ${field.errors['minlength'].requiredLength} caractères`;
    }
    return '';
  }

  getPasswordErrorMessage(fieldName: string): string {
    const field = this.passwordForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return 'Ce champ est requis';
      if (field.errors['minlength']) return 'Le mot de passe doit contenir au moins 8 caractères';
      if (field.errors['passwordMismatch']) return 'Les mots de passe ne correspondent pas';
    }
    return '';
  }

  get isFormValid(): boolean {
    return this.personalInfoForm.valid && this.passwordForm.valid;
  }
}