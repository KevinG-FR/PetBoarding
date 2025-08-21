import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

interface ChangePasswordData {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-change-password-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './change-password-dialog.component.html',
  styleUrl: './change-password-dialog.component.scss'
})
export class ChangePasswordDialogComponent {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<ChangePasswordDialogComponent>);
  private readonly snackBar = inject(MatSnackBar);

  // Signals
  private readonly _hideCurrentPassword = signal(true);
  private readonly _hideNewPassword = signal(true);
  private readonly _hideConfirmPassword = signal(true);
  private readonly _isSubmitting = signal(false);

  // Propriétés calculées
  readonly hideCurrentPassword = this._hideCurrentPassword.asReadonly();
  readonly hideNewPassword = this._hideNewPassword.asReadonly();
  readonly hideConfirmPassword = this._hideConfirmPassword.asReadonly();
  readonly isSubmitting = this._isSubmitting.asReadonly();

  // Formulaire
  passwordForm: FormGroup;

  constructor() {
    this.passwordForm = this.createForm();
  }

  private createForm(): FormGroup {
    return this.fb.group(
      {
        currentPassword: ['', [Validators.required]],
        newPassword: [
          '',
          [Validators.required, Validators.minLength(8), this.passwordStrengthValidator]
        ],
        confirmPassword: ['', [Validators.required]]
      },
      { validators: this.passwordMatchValidator }
    );
  }

  private passwordStrengthValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
    if (!value) return null;

    const hasUppercase = /[A-Z]/.test(value);
    const hasLowercase = /[a-z]/.test(value);
    const hasNumber = /\d/.test(value);
    const hasSpecialChar = /[!@#$%^&*()_+\-=[\]{};':"\\|,.<>?]/.test(value);

    const passwordValid = hasUppercase && hasLowercase && hasNumber && hasSpecialChar;

    return passwordValid ? null : { passwordStrength: true };
  }

  private passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const newPassword = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    return newPassword === confirmPassword ? null : { passwordMismatch: true };
  }

  toggleCurrentPasswordVisibility(): void {
    this._hideCurrentPassword.set(!this._hideCurrentPassword());
  }

  toggleNewPasswordVisibility(): void {
    this._hideNewPassword.set(!this._hideNewPassword());
  }

  toggleConfirmPasswordVisibility(): void {
    this._hideConfirmPassword.set(!this._hideConfirmPassword());
  }

  hasMinLength(): boolean {
    const newPassword = this.passwordForm.get('newPassword')?.value || '';
    return newPassword.length >= 8;
  }

  hasUppercase(): boolean {
    const newPassword = this.passwordForm.get('newPassword')?.value || '';
    return /[A-Z]/.test(newPassword);
  }

  hasLowercase(): boolean {
    const newPassword = this.passwordForm.get('newPassword')?.value || '';
    return /[a-z]/.test(newPassword);
  }

  hasNumber(): boolean {
    const newPassword = this.passwordForm.get('newPassword')?.value || '';
    return /\d/.test(newPassword);
  }

  hasSpecialChar(): boolean {
    const newPassword = this.passwordForm.get('newPassword')?.value || '';
    return /[!@#$%^&*()_+\-=[\]{};':"\\|,.<>?]/.test(newPassword);
  }

  onSubmit(): void {
    if (this.passwordForm.valid && !this._isSubmitting()) {
      this._isSubmitting.set(true);

      const formValue = this.passwordForm.value;

      this.changePassword(formValue);
    } else {
      this.markFormGroupTouched();
    }
  }

  private changePassword(_passwordData: ChangePasswordData): void {
    setTimeout(() => {
      try {
        this.snackBar.open('Mot de passe modifié avec succès !', 'Fermer', {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'top',
          panelClass: ['success-snackbar']
        });

        this.dialogRef.close(true);
      } catch (_error) {
        this.snackBar.open('Erreur lors du changement de mot de passe', 'Fermer', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top',
          panelClass: ['error-snackbar']
        });
        this._isSubmitting.set(false);
      }
    }, 1500);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  private markFormGroupTouched(): void {
    Object.keys(this.passwordForm.controls).forEach((key: string) => {
      this.passwordForm.get(key)?.markAsTouched();
    });
  }

  getErrorMessage(fieldName: string): string {
    const control = this.passwordForm.get(fieldName);

    if (control?.hasError('required')) {
      return 'Ce champ est obligatoire';
    }

    if (control?.hasError('minlength')) {
      return 'Le mot de passe doit contenir au moins 8 caractères';
    }

    if (control?.hasError('passwordStrength')) {
      return 'Le mot de passe ne respecte pas les critères de sécurité';
    }

    if (fieldName === 'confirmPassword' && this.passwordForm.hasError('passwordMismatch')) {
      return 'Les mots de passe ne correspondent pas';
    }

    return '';
  }

  hasError(fieldName: string): boolean {
    const control = this.passwordForm.get(fieldName);
    const formErrors =
      fieldName === 'confirmPassword' && this.passwordForm.hasError('passwordMismatch');

    return !!(control?.invalid && control?.touched) || formErrors;
  }
}
