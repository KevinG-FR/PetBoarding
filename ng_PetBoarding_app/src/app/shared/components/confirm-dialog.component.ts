import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmButtonText?: string;
  cancelButtonText?: string;
  confirmButtonColor?: 'primary' | 'accent' | 'warn';
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="confirm-dialog">
      <h2 mat-dialog-title class="d-flex align-items-center">
        <mat-icon class="me-2 text-warning">warning</mat-icon>
        {{ data.title }}
      </h2>
      
      <mat-dialog-content class="py-3">
        <p class="mb-0">{{ data.message }}</p>
      </mat-dialog-content>
      
      <mat-dialog-actions class="justify-content-end gap-2">
        <button mat-button (click)="onCancel()">
          {{ data.cancelButtonText || 'Annuler' }}
        </button>
        <button 
          mat-raised-button 
          [color]="data.confirmButtonColor || 'primary'"
          (click)="onConfirm()">
          {{ data.confirmButtonText || 'Confirmer' }}
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .confirm-dialog {
      min-width: 300px;
    }
    
    mat-dialog-actions {
      padding: 16px 24px;
    }
    
    .gap-2 {
      gap: 0.5rem;
    }
  `]
})
export class ConfirmDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
  ) {}

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}