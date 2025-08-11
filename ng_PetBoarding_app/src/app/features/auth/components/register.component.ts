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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';

import { RegisterRequestDto } from '../../../shared/contracts/auth/register-request.dto';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  // Signals pour l'état du composant
  isSubmitting = signal(false);
  hidePassword = signal(true);
  hideConfirmPassword = signal(true);

  // Formulaire réactif
  registerForm: FormGroup;

  constructor() {
    this.registerForm = this.createForm();
  }

  /**
   * Création du formulaire avec validations
   */
  private createForm(): FormGroup {
    return this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        password: [
          '',
          [Validators.required, Validators.minLength(8), this.passwordStrengthValidator]
        ],
        confirmPassword: ['', [Validators.required]],
        firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
        lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
        phone: ['', [Validators.required, Validators.pattern(/^(?:\+33|0)[1-9](?:[0-9]{8})$/)]],
        acceptTerms: [false, [Validators.requiredTrue]],
        acceptNewsletter: [false]
      },
      {
        validators: this.passwordMatchValidator
      }
    );
  }

  /**
   * Validateur personnalisé pour la force du mot de passe
   */
  private passwordStrengthValidator(control: AbstractControl) {
    const password = control.value;
    if (!password) return null;

    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    const hasNumbers = /\d/.test(password);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);

    const validConditions = [hasUpperCase, hasLowerCase, hasNumbers, hasSpecialChar].filter(
      (condition) => condition
    ).length;

    if (validConditions < 3) {
      return { passwordWeak: true };
    }

    return null;
  }

  /**
   * Validateur pour vérifier que les mots de passe correspondent
   */
  private passwordMatchValidator(group: AbstractControl) {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;

    if (password && confirmPassword && password !== confirmPassword) {
      return { passwordMismatch: true };
    }

    return null;
  }

  /**
   * Basculer la visibilité du mot de passe
   */
  togglePasswordVisibility(): void {
    this.hidePassword.update((value) => !value);
  }

  /**
   * Basculer la visibilité de la confirmation du mot de passe
   */
  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword.update((value) => !value);
  }

  /**
   * Obtenir les messages d'erreur pour un champ
   */
  getErrorMessage(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    if (!field || !field.errors) return '';

    const errors = field.errors;

    switch (fieldName) {
      case 'email':
        if (errors['required']) return "L'email est requis";
        if (errors['email']) return "Format d'email invalide";
        break;

      case 'password':
        if (errors['required']) return 'Le mot de passe est requis';
        if (errors['minlength']) return 'Le mot de passe doit contenir au moins 8 caractères';
        if (errors['passwordWeak'])
          return 'Le mot de passe doit contenir au moins 3 des éléments suivants : majuscule, minuscule, chiffre, caractère spécial';
        break;

      case 'confirmPassword':
        if (errors['required']) return 'La confirmation du mot de passe est requise';
        break;

      case 'firstName':
        if (errors['required']) return 'Le prénom est requis';
        if (errors['minlength']) return 'Le prénom doit contenir au moins 2 caractères';
        if (errors['maxlength']) return 'Le prénom ne peut pas dépasser 50 caractères';
        break;

      case 'lastName':
        if (errors['required']) return 'Le nom est requis';
        if (errors['minlength']) return 'Le nom doit contenir au moins 2 caractères';
        if (errors['maxlength']) return 'Le nom ne peut pas dépasser 50 caractères';
        break;

      case 'phone':
        if (errors['required']) return 'Le téléphone est requis';
        if (errors['pattern']) return 'Format de téléphone invalide (ex: 06 12 34 56 78)';
        break;

      case 'acceptTerms':
        if (errors['required']) return "Vous devez accepter les conditions d'utilisation";
        break;
    }

    return '';
  }

  /**
   * Vérifier si le formulaire a l'erreur de correspondance des mots de passe
   */
  hasPasswordMismatchError(): boolean {
    return (
      this.registerForm.hasError('passwordMismatch') &&
      this.registerForm.get('confirmPassword')?.touched === true
    );
  }

  /**
   * Soumission du formulaire
   */
  onSubmit(): void {
    if (this.registerForm.valid && !this.isSubmitting()) {
      this.isSubmitting.set(true);

      const formValue = this.registerForm.value;
      const registerData: RegisterRequestDto = {
        email: formValue.email,
        password: formValue.password,
        confirmPassword: formValue.confirmPassword,
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        phone: formValue.phone,
        acceptTerms: formValue.acceptTerms,
        acceptNewsletter: formValue.acceptNewsletter
      };

      this.authService.register(registerData).subscribe({
        next: (response) => {
          if (response.success) {
            this.snackBar.open('Inscription réussie ! Bienvenue chez PetBoarding !', 'Fermer', {
              duration: 5000,
              panelClass: ['success-snackbar']
            });

            // Rediriger vers l'accueil ou le profil
            this.router.navigate(['/home']);
          } else {
            this.snackBar.open(response.message || "Erreur lors de l'inscription", 'Fermer', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
          }
        },
        error: (error) => {
          // eslint-disable-next-line no-console
          console.error('Erreur inscription:', error);
          this.snackBar.open('Une erreur est survenue. Veuillez réessayer.', 'Fermer', {
            duration: 5000,
            panelClass: ['error-snackbar']
          });
        },
        complete: () => {
          this.isSubmitting.set(false);
        }
      });
    } else {
      // Marquer tous les champs comme touchés pour afficher les erreurs
      Object.keys(this.registerForm.controls).forEach((key) => {
        this.registerForm.get(key)?.markAsTouched();
      });
    }
  }

  /**
   * Naviguer vers la page de connexion
   */
  goToLogin(): void {
    this.router.navigate(['/login']);
  }
}
