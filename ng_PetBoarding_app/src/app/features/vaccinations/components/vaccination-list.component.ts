import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';

import { Vaccination } from '../models/vaccination';
import { VaccinationComponent } from './vaccination.component';

@Component({
  selector: 'app-vaccination-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    VaccinationComponent
  ],
  templateUrl: './vaccination-list.component.html',
  styleUrl: './vaccination-list.component.scss'
})
export class VaccinationListComponent {
  @Input({ required: true }) vaccinations: Vaccination[] = [];
  @Input() canAdd = true;
  @Input() canEdit = true;
  @Input() canDelete = true;
  @Output() addVaccination = new EventEmitter<void>();
  @Output() editVaccination = new EventEmitter<Vaccination>();
  @Output() deleteVaccination = new EventEmitter<Vaccination>();

  onAddVaccination(): void {
    this.addVaccination.emit();
  }

  onEditVaccination(vaccination: Vaccination): void {
    this.editVaccination.emit(vaccination);
  }

  onDeleteVaccination(vaccination: Vaccination): void {
    this.deleteVaccination.emit(vaccination);
  }

  get sortedVaccinations(): Vaccination[] {
    return [...this.vaccinations].sort((a, b) => {
      return new Date(b.date).getTime() - new Date(a.date).getTime();
    });
  }

  get hasExpiredVaccinations(): boolean {
    return this.vaccinations.some((v) => v.expiryDate && new Date(v.expiryDate) < new Date());
  }

  get hasExpiringSoonVaccinations(): boolean {
    return this.vaccinations.some((v) => {
      if (!v.expiryDate) return false;
      const today = new Date();
      const expiryDate = new Date(v.expiryDate);
      const diffTime = expiryDate.getTime() - today.getTime();
      const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
      return diffDays <= 30 && diffDays > 0;
    });
  }
}
