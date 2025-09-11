import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';

import { UserService } from '../../services/user.service';
import { User } from '../../../auth/models/user.model';
import { ProfileType } from '../../../../shared/enums/profile-type.enum';

@Component({
  selector: 'app-user-detail',
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
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.scss']
})
export class UserDetailComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  user = signal<User | null>(null);
  loading = signal(false);
  saving = signal(false);
  deleting = signal(false);
  isEditMode = signal(false);
  
  userForm: FormGroup;
  readonly ProfileType = ProfileType;
  readonly adminProfileTypes = [ProfileType.Administrator, ProfileType.Employee];

  constructor() {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required]],
      profileType: ['', [Validators.required]],
      address: this.fb.group({
        streetNumber: [''],
        streetName: [''],
        city: [''],
        postalCode: [''],
        country: ['France'],
        complement: ['']
      })
    });
  }

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id');
    const isEditRoute = this.route.snapshot.url.some(segment => segment.path === 'edit');
    
    if (userId) {
      this.isEditMode.set(isEditRoute);
      this.loadUser(userId);
    } else {
      this.router.navigate(['/admin/users']);
    }
  }

  get profileTypeLabel(): string {
    const user = this.user();
    if (!user) return '';
    
    switch (user.profileType) {
      case ProfileType.Administrator:
        return 'Administrateur';
      case ProfileType.Employee:
        return 'Employé';
      default:
        return user.profileType;
    }
  }

  get profileTypeColor(): string {
    const user = this.user();
    if (!user) return 'primary';
    
    switch (user.profileType) {
      case ProfileType.Administrator:
        return 'primary';
      case ProfileType.Employee:
        return 'accent';
      default:
        return 'primary';
    }
  }

  get userInitials(): string {
    const user = this.user();
    if (!user) return '';
    return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
  }

  loadUser(userId: string): void {
    this.loading.set(true);
    
    this.userService.getUserById(userId).subscribe({
      next: (user) => {
        this.user.set(user);
        this.populateForm(user);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading user:', error);
        this.loading.set(false);
        alert('Erreur lors du chargement de l\'utilisateur');
        this.router.navigate(['/admin/users']);
      }
    });
  }

  populateForm(user: User): void {
    this.userForm.patchValue({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber,
      profileType: user.profileType,
      address: {
        streetNumber: user.address?.streetNumber || '',
        streetName: user.address?.streetName || '',
        city: user.address?.city || '',
        postalCode: user.address?.postalCode || '',
        country: user.address?.country || 'France',
        complement: user.address?.complement || ''
      }
    });
  }

  onEdit(): void {
    this.isEditMode.set(true);
    this.router.navigate(['/admin/users', this.user()?.id, 'edit']);
  }

  onCancelEdit(): void {
    this.isEditMode.set(false);
    const user = this.user();
    if (user) {
      this.populateForm(user);
    }
    this.router.navigate(['/admin/users', this.user()?.id]);
  }

  onSave(): void {
    if (this.userForm.valid) {
      const user = this.user();
      if (!user) return;

      this.saving.set(true);
      const formValue = this.userForm.value;

      this.userService.updateUser(user.id, formValue).subscribe({
        next: (updatedUser) => {
          this.user.set(updatedUser);
          this.isEditMode.set(false);
          this.saving.set(false);
          this.router.navigate(['/admin/users', updatedUser.id]);
        },
        error: (error) => {
          console.error('Error updating user:', error);
          this.saving.set(false);
          alert('Erreur lors de la mise à jour de l\'utilisateur');
        }
      });
    }
  }

  onDelete(): void {
    const user = this.user();
    if (!user) return;

    const confirmed = confirm(`Êtes-vous sûr de vouloir supprimer l'utilisateur ${user.firstName} ${user.lastName} ?`);
    
    if (!confirmed) return;

    this.deleting.set(true);
    
    this.userService.deleteUser(user.id).subscribe({
      next: () => {
        this.deleting.set(false);
        this.router.navigate(['/admin/users']);
      },
      error: (error) => {
        console.error('Error deleting user:', error);
        this.deleting.set(false);
        alert('Erreur lors de la suppression de l\'utilisateur');
      }
    });
  }

  onBack(): void {
    this.router.navigate(['/admin/users']);
  }

  getFieldErrorMessage(fieldName: string): string {
    const field = this.userForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `Le champ ${fieldName} est requis`;
      if (field.errors['email']) return 'Email invalide';
      if (field.errors['minlength']) return `Minimum ${field.errors['minlength'].requiredLength} caractères`;
    }
    return '';
  }
}