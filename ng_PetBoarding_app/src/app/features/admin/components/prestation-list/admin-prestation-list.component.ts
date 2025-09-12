import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  OnInit,
  computed,
  inject,
  signal
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { Prestation } from '../../../../shared/models/prestation.model';
import { PetType } from '../../../pets/models/pet.model';
import { PrestationsService } from '../../../prestations/services/prestations.service';
import { AdminPrestationFiltersComponent } from '../prestation-filters/admin-prestation-filters.component';
import { AdminPrestationItemComponent } from '../prestation-item/admin-prestation-item.component';

export interface PrestationFilters {
  categorieAnimal?: PetType;
  searchText?: string;
  estDisponible?: boolean;
}

@Component({
  selector: 'app-admin-prestation-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    AdminPrestationItemComponent,
    AdminPrestationFiltersComponent
  ],
  templateUrl: './admin-prestation-list.component.html',
  styleUrl: './admin-prestation-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminPrestationListComponent implements OnInit {
  private readonly prestationsService = inject(PrestationsService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);
  private readonly router = inject(Router);

  prestations = signal<Prestation[]>([]);
  filters = signal<PrestationFilters>({});
  isLoading = signal(false);
  error = signal<string | null>(null);

  filteredPrestations = computed(() => {
    const currentFilters = this.filters();
    let filtered = this.prestations();

    if (currentFilters.categorieAnimal !== undefined) {
      filtered = filtered.filter((p) => p.categorieAnimal === currentFilters.categorieAnimal);
    }

    if (currentFilters.searchText) {
      const searchText = currentFilters.searchText.toLowerCase();
      filtered = filtered.filter(
        (p) =>
          p.libelle.toLowerCase().includes(searchText) ||
          p.description.toLowerCase().includes(searchText)
      );
    }

    if (currentFilters.estDisponible !== undefined) {
      filtered = filtered.filter((p) => p.disponible === currentFilters.estDisponible);
    }

    return filtered;
  });

  ngOnInit(): void {
    this.loadPrestations();
  }

  loadPrestations(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.prestationsService.loadPrestations().subscribe({
      next: (prestations) => {
        this.prestations.set(prestations);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Erreur lors du chargement des prestations:', error);
        this.error.set('Erreur lors du chargement des prestations. Veuillez réessayer.');
        this.isLoading.set(false);
      }
    });
  }

  onFiltersChange(newFilters: PrestationFilters): void {
    this.filters.set(newFilters);
  }

  onAddPrestation(): void {
    this.router.navigate(['/admin/prestations/add']);
  }

  onViewPrestation(id: string): void {
    this.router.navigate(['/admin/prestations', id]);
  }

  onEditPrestation(id: string): void {
    this.router.navigate(['/admin/prestations', id, 'edit']);
  }

  onDeletePrestation(id: string): void {
    const prestation = this.prestations().find((p) => p.id === id);
    if (!prestation) return;

    const dialogRef = this.dialog.open(ConfirmDeleteDialog, {
      width: '400px',
      data: { prestationName: prestation.libelle }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.deletePrestation(id);
      }
    });
  }

  private deletePrestation(id: string): void {
    this.prestationsService.deletePrestation(id).subscribe({
      next: (success) => {
        if (success) {
          this.prestations.update((prestations) => prestations.filter((p) => p.id !== id));
          this.snackBar.open('Prestation supprimée avec succès', 'Fermer', {
            duration: 3000
          });
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
