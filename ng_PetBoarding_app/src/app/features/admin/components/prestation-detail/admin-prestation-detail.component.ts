import { CommonModule } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { PrestationScheduleComponent } from '../../../../shared/components/prestation-schedule/prestation-schedule.component';
import { Prestation } from '../../../../shared/models/prestation.model';
import { PetType, PetTypeLabels } from '../../../pets/models/pet.model';
import { PrestationsService } from '../../../prestations/services/prestations.service';

@Component({
  selector: 'app-admin-prestation-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    PrestationScheduleComponent
  ],
  templateUrl: './admin-prestation-detail.component.html',
  styleUrl: './admin-prestation-detail.component.scss'
})
export class AdminPrestationDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly prestationsService = inject(PrestationsService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);

  prestation = signal<Prestation | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);
  isEditing = false;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    const isEdit = this.router.url.includes('/edit');
    this.isEditing = isEdit;

    if (id) {
      this.loadPrestation(id);
    }
  }

  loadPrestation(id?: string): void {
    const prestationId = id || this.route.snapshot.paramMap.get('id');
    if (!prestationId) return;

    this.isLoading.set(true);
    this.error.set(null);

    this.prestationsService.getPrestationById(prestationId).subscribe({
      next: (prestation) => {
        this.prestation.set(prestation);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erreur lors du chargement de la prestation:', error);
        this.error.set('Erreur lors du chargement de la prestation. Veuillez réessayer.');
        this.isLoading.set(false);
      }
    });
  }

  getPetTypeLabel(petType: PetType): string {
    return PetTypeLabels[petType] || petType;
  }

  formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes} min`;
    }
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    if (remainingMinutes === 0) {
      return `${hours}h`;
    }
    return `${hours}h ${remainingMinutes}min`;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  onEdit(): void {
    const id = this.prestation()?.id;
    if (id) {
      this.router.navigate(['/admin/prestations', id, 'edit']);
    }
  }

  onDelete(): void {
    const prestation = this.prestation();
    if (!prestation) return;

    const confirmRef = this.dialog.open(ConfirmDeleteDialog, {
      width: '400px',
      data: { prestationName: prestation.libelle }
    });

    confirmRef.afterClosed().subscribe((result) => {
      if (result && prestation.id) {
        this.deletePrestation(prestation.id);
      }
    });
  }

  private deletePrestation(id: string): void {
    this.prestationsService.deletePrestation(id).subscribe({
      next: (success) => {
        if (success) {
          this.snackBar.open('Prestation supprimée avec succès', 'Fermer', {
            duration: 3000
          });
          this.router.navigate(['/admin/prestations']);
        } else {
          this.snackBar.open('Erreur lors de la suppression', 'Fermer', {
            duration: 5000
          });
        }
      },
      error: (error) => {
        console.error('Erreur lors de la suppression:', error);
        this.snackBar.open('Erreur lors de la suppression de la prestation', 'Fermer', {
          duration: 5000
        });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/admin/prestations']);
  }
}

@Component({
  selector: 'confirm-delete-dialog',
  template: `
    <h2 mat-dialog-title>Confirmer la suppression</h2>
    <mat-dialog-content>
      <p>
        Êtes-vous sûr de vouloir supprimer la prestation
        <strong>{{ data.prestationName }}</strong> ?
      </p>
      <p>Cette action est irréversible.</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Annuler</button>
      <button mat-raised-button color="warn" (click)="onConfirm()">Supprimer</button>
    </mat-dialog-actions>
  `,
  imports: [MatDialogModule, MatButtonModule],
  standalone: true
})
export class ConfirmDeleteDialog {
  public data = inject(MAT_DIALOG_DATA) as { prestationName: string };

  constructor(public dialogRef: MatDialogRef<ConfirmDeleteDialog>) {}

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}
