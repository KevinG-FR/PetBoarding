import { CommonModule } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router } from '@angular/router';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog.component';
import { Pet, PetGenderLabels, PetType, PetTypeLabels } from '../models/pet.model';
import { PetService } from '../services/pet.service';

@Component({
  selector: 'app-pet-card',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatButtonModule,
    MatChipsModule,
    MatBadgeModule,
    MatTooltipModule
  ],
  templateUrl: './pet-card.component.html',
  styleUrl: './pet-card.component.scss'
})
export class PetCardComponent {
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);
  private readonly petService = inject(PetService);

  // Inputs
  pet = input.required<Pet>();
  highlighted = input<boolean>(false);

  // Labels publics
  petTypeLabels = PetTypeLabels;
  petGenderLabels = PetGenderLabels;

  onEdit(): void {
    this.router.navigate(['/profile/pets', this.pet().id, 'edit']);
  }

  onDelete(): void {
    const pet = this.pet();
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Supprimer un animal',
        message: `Êtes-vous sûr de vouloir supprimer ${pet.name} ? Cette action est irréversible.`,
        confirmButtonText: 'Supprimer',
        confirmButtonColor: 'warn'
      }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed) {
        this.deletePet();
      }
    });
  }

  private deletePet(): void {
    const pet = this.pet();
    this.petService.deletePet(pet.id).subscribe({
      next: (success) => {
        if (success) {
          this.snackBar.open(`${pet.name} a été supprimé avec succès`, 'Fermer', {
            duration: 5000
          });
        } else {
          this.snackBar.open('Erreur lors de la suppression', 'Fermer', {
            duration: 5000
          });
        }
      },
      error: () => {
        this.snackBar.open('Erreur lors de la suppression', 'Fermer', {
          duration: 5000
        });
      }
    });
  }

  onViewDetails(): void {
    this.router.navigate(['/profile/pets', this.pet().id]);
  }

  getPetIcon(type: PetType): string {
    const icons = {
      [PetType.DOG]: 'pets',
      [PetType.CAT]: 'pets',
      [PetType.BIRD]: 'flutter_dash',
      [PetType.RABBIT]: 'cruelty_free',
      [PetType.HAMSTER]: 'pets'
    };
    return icons[type] || 'pets';
  }

  getAgeDisplay(age: number): string {
    if (age === 0) return "Moins d'un an";
    if (age === 1) return '1 an';
    return `${age} ans`;
  }

  getVaccinationStatusClass(): string {
    const upToDate = this.areVaccinationsUpToDate();
    return upToDate ? 'status-chip status-chip--success' : 'status-chip status-chip--warning';
  }

  getVaccinationStatusText(): string {
    return this.areVaccinationsUpToDate() ? 'À jour' : 'À vérifier';
  }

  areVaccinationsUpToDate(): boolean {
    const now = new Date();
    return this.pet().vaccinations.every((v) => !v.expiryDate || v.expiryDate > now);
  }

  isVaccinationExpired(vaccination: { expiryDate?: Date }): boolean {
    if (!vaccination.expiryDate) return false;
    return vaccination.expiryDate < new Date();
  }

  isVaccinationExpiringSoon(vaccination: { expiryDate?: Date }): boolean {
    if (!vaccination.expiryDate) return false;
    const now = new Date();
    const oneMonthFromNow = new Date();
    oneMonthFromNow.setMonth(now.getMonth() + 1);
    return vaccination.expiryDate <= oneMonthFromNow && vaccination.expiryDate > now;
  }

  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(new Date(date));
  }
}
