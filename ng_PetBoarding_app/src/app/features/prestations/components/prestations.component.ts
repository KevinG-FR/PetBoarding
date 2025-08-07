import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { PrestationFilters } from '../models/prestation.model';
import { PrestationFiltersComponent } from './prestation-filters.component';
import { PrestationsListComponent } from './prestations-list.component';

@Component({
  selector: 'app-prestations',
  standalone: true,
  imports: [CommonModule, MatIconModule, PrestationFiltersComponent, PrestationsListComponent],
  templateUrl: './prestations.component.html',
  styleUrl: './prestations.component.scss'
})
export class PrestationsComponent {
  // Gestion centralisée des filtres
  filters = signal<PrestationFilters>({});

  onFiltersChanged(newFilters: PrestationFilters) {
    this.filters.set(newFilters);
  }
}
