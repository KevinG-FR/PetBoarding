import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Vaccination } from '../models/vaccination';

@Component({
  selector: 'app-vaccination',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './vaccination.component.html',
  styleUrl: './vaccination.component.scss'
})
export class VaccinationComponent {
  @Input({ required: true }) vaccination!: Vaccination;
  @Input() canEdit = true;
  @Input() canDelete = true;
  @Output() edit = new EventEmitter<Vaccination>();
  @Output() delete = new EventEmitter<Vaccination>();

  onEdit(): void {
    this.edit.emit(this.vaccination);
  }

  onDelete(): void {
    this.delete.emit(this.vaccination);
  }

  formatDate(date: Date): string {
    return new Intl.DateTimeFormat('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    }).format(new Date(date));
  }

  isExpired(): boolean {
    if (!this.vaccination.expiryDate) return false;
    return new Date(this.vaccination.expiryDate) < new Date();
  }

  isExpiringSoon(): boolean {
    if (!this.vaccination.expiryDate) return false;
    const today = new Date();
    const expiryDate = new Date(this.vaccination.expiryDate);
    const diffTime = expiryDate.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays <= 30 && diffDays > 0;
  }

  getStatusIcon(): string {
    if (this.isExpired()) return 'warning';
    if (this.isExpiringSoon()) return 'schedule';
    return 'check_circle';
  }

  getStatusColor(): string {
    if (this.isExpired()) return 'warn';
    if (this.isExpiringSoon()) return 'accent';
    return 'primary';
  }

  getStatusText(): string {
    if (this.isExpired()) return 'Expiré';
    if (this.isExpiringSoon()) return 'Expire bientôt';
    return 'Valide';
  }
}
