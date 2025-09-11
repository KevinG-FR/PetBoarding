import { Component, Input, Output, EventEmitter, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';

import { User } from '../../../auth/models/user.model';
import { ProfileType } from '../../../../shared/enums/profile-type.enum';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-item',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatMenuModule,
    MatDividerModule
  ],
  templateUrl: './user-item.component.html',
  styleUrls: ['./user-item.component.scss']
})
export class UserItemComponent {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);

  @Input({ required: true }) user!: User;
  @Output() userDeleted = new EventEmitter<void>();

  deleting = signal(false);
  
  readonly ProfileType = ProfileType;

  get profileTypeLabel(): string {
    switch (this.user.profileType) {
      case ProfileType.Administrator:
        return 'Administrateur';
      case ProfileType.Employee:
        return 'Employé';
      default:
        return this.user.profileType;
    }
  }

  get profileTypeColor(): string {
    switch (this.user.profileType) {
      case ProfileType.Administrator:
        return 'primary';
      case ProfileType.Employee:
        return 'accent';
      default:
        return 'primary';
    }
  }

  get userInitials(): string {
    return `${this.user.firstName.charAt(0)}${this.user.lastName.charAt(0)}`.toUpperCase();
  }

  onViewDetails(): void {
    this.router.navigate(['/admin/users', this.user.id]);
  }

  onEdit(): void {
    this.router.navigate(['/admin/users', this.user.id, 'edit']);
  }

  async onDelete(): Promise<void> {
    const confirmed = confirm(`Êtes-vous sûr de vouloir supprimer l'utilisateur ${this.user.firstName} ${this.user.lastName} ?`);
    
    if (!confirmed) {
      return;
    }

    this.deleting.set(true);
    
    this.userService.deleteUser(this.user.id).subscribe({
      next: () => {
        this.deleting.set(false);
        this.userDeleted.emit();
      },
      error: (error) => {
        console.error('Error deleting user:', error);
        this.deleting.set(false);
        alert('Erreur lors de la suppression de l\'utilisateur');
      }
    });
  }
}