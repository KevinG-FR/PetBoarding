import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';

import { User } from '../../auth/models/user.model';
import { UpdateProfileRequestDto } from '../contracts/update-profile.dto';
import { ProfileService } from '../services/profile.service';
import { ChangePasswordDialogComponent } from './change-password-dialog.component';

@Component({
  selector: 'app-profile-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './profile-edit.component.html',
  styleUrl: './profile-edit.component.scss'
})
export class ProfileEditComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly profileService = inject(ProfileService);
  private readonly router = inject(Router);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);

  // Signals
  private readonly _isSubmitting = signal(false);
  private readonly _hasChanges = signal(false);

  // Propriétés calculées
  readonly isSubmitting = this._isSubmitting.asReadonly();
  readonly hasChanges = this._hasChanges.asReadonly();
  readonly currentUser = this.profileService.currentUser;

  // Formulaire
  profileForm!: FormGroup;

  ngOnInit(): void {
    this.initializeForm();
    this.setupFormChangeDetection();
  }

  /**
   * Initialiser le formulaire avec les données de l'utilisateur
   */
  private initializeForm(): void {
    const user = this.currentUser();

    this.profileForm = this.fb.group({
      firstName: [
        user?.firstName || '',
        [Validators.required, Validators.minLength(2), Validators.maxLength(50)]
      ],
      lastName: [
        user?.lastName || '',
        [Validators.required, Validators.minLength(2), Validators.maxLength(50)]
      ],
      email: [user?.email || '', [Validators.required, Validators.email]],
      phone: [user?.phone || '', [Validators.required, Validators.pattern(/^[+]?[0-9\s\-()]+$/)]],
      // Groupe pour l'adresse
      address: this.fb.group({
        streetNumber: [user?.address?.streetNumber || '', [Validators.required]],
        streetName: [user?.address?.streetName || '', [Validators.required]],
        city: [user?.address?.city || '', [Validators.required]],
        postalCode: [user?.address?.postalCode || '', [Validators.required]],
        country: [user?.address?.country || 'France', [Validators.required]],
        complement: [user?.address?.complement || '']
      })
    });
  }

  /**
   * Configurer la détection des changements dans le formulaire
   */
  private setupFormChangeDetection(): void {
    this.profileForm.valueChanges.subscribe(() => {
      this._hasChanges.set(this.profileForm.dirty);
    });
  }

  /**
   * Soumettre le formulaire
   */
  onSubmit(): void {
    if (this.profileForm.valid && !this._isSubmitting()) {
      this._isSubmitting.set(true);

      const formValue = this.profileForm.value;
      const profileData: UpdateProfileRequestDto = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email,
        phone: formValue.phone,
        address: formValue.address
      };

      this.updateProfile(profileData);
    } else {
      this.markFormGroupTouched();
    }
  }

  /**
   * Mettre à jour le profil via l'API
   */
  private updateProfile(profileData: UpdateProfileRequestDto): void {
    this.profileService.updateUserProfile(profileData).subscribe({
      next: (_updatedUser: User) => {
        this.snackBar.open('Profil mis à jour avec succès !', 'Fermer', {
          duration: 3000,
          horizontalPosition: 'center',
          verticalPosition: 'top',
          panelClass: ['success-snackbar']
        });

        this.profileForm.markAsPristine();
        this._hasChanges.set(false);
        this._isSubmitting.set(false);

        // Rediriger vers le profil après la mise à jour
        this.router.navigate(['/profile']);
      },
      error: (error: Error) => {
        this.snackBar.open(error.message, 'Fermer', {
          duration: 5000,
          horizontalPosition: 'center',
          verticalPosition: 'top',
          panelClass: ['error-snackbar']
        });
        this._isSubmitting.set(false);
      }
    });
  }

  /**
   * Annuler les modifications
   */
  onCancel(): void {
    if (this._hasChanges()) {
      const confirmCancel = confirm(
        'Vous avez des modifications non sauvegardées. Voulez-vous vraiment annuler ?'
      );
      if (confirmCancel) {
        this.router.navigate(['/profile']);
      }
    } else {
      this.router.navigate(['/profile']);
    }
  }

  /**
   * Ouvrir le dialogue de changement de mot de passe
   */
  openChangePasswordDialog(): void {
    const dialogRef = this.dialog.open(ChangePasswordDialogComponent, {
      width: '500px',
      disableClose: true,
      autoFocus: true
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        // Le mot de passe a été changé avec succès
        // Aucune action supplémentaire nécessaire car le snackbar est déjà affiché
      }
    });
  }

  /**
   * Marquer tous les champs du formulaire comme touchés
   */
  private markFormGroupTouched(): void {
    Object.keys(this.profileForm.controls).forEach((key) => {
      this.profileForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Obtenir le message d'erreur pour un champ
   */
  getErrorMessage(fieldName: string): string {
    const control = this.profileForm.get(fieldName);

    if (control?.hasError('required')) {
      return 'Ce champ est obligatoire';
    }

    if (control?.hasError('email')) {
      return 'Veuillez entrer un email valide';
    }

    if (control?.hasError('minlength')) {
      const minLength = control.errors?.['minlength']?.requiredLength;
      return `Minimum ${minLength} caractères requis`;
    }

    if (control?.hasError('maxlength')) {
      const maxLength = control.errors?.['maxlength']?.requiredLength;
      return `Maximum ${maxLength} caractères autorisés`;
    }

    if (control?.hasError('pattern')) {
      if (fieldName === 'phone') {
        return 'Format de téléphone invalide';
      }
    }

    return '';
  }

  /**
   * Vérifier si un champ a une erreur
   */
  hasError(fieldName: string): boolean {
    const control = this.profileForm.get(fieldName);
    return !!(control?.invalid && control?.touched);
  }

  /**
   * Vérifier si un champ d'adresse a une erreur
   */
  hasAddressError(fieldName: string): boolean {
    const control = this.profileForm.get(`address.${fieldName}`);
    return !!(control?.invalid && control?.touched);
  }

  /**
   * Obtenir le message d'erreur pour un champ d'adresse
   */
  getAddressErrorMessage(fieldName: string): string {
    const control = this.profileForm.get(`address.${fieldName}`);

    if (control?.hasError('required')) {
      return 'Ce champ est obligatoire';
    }

    return '';
  }
}
