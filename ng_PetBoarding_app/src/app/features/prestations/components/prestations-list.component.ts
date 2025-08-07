import { CommonModule } from '@angular/common';
import { Component, computed, inject, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { PrestationFilters } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';
import { PrestationItemComponent } from './prestation-item.component';

@Component({
  selector: 'app-prestations-list',
  standalone: true,
  imports: [CommonModule, MatIconModule, PrestationItemComponent],
  templateUrl: './prestations-list.component.html',
  styleUrl: './prestations-list.component.scss'
})
export class PrestationsListComponent {
  private prestationsService = inject(PrestationsService);

  // Input pour recevoir les filtres du parent
  filters = input<PrestationFilters>({});

  // Données de base
  allPrestations = this.prestationsService.getAllPrestations();

  // Prestations filtrées basées sur les filtres reçus
  filteredPrestations = computed(() => {
    return this.prestationsService.createFilteredPrestations(this.allPrestations(), this.filters());
  });

  // Computed pour les statistiques
  hasResults = computed(() => this.filteredPrestations().length > 0);
  isFiltered = computed(() => this.filteredPrestations().length < this.allPrestations().length);
  resultCount = computed(() => this.filteredPrestations().length);
  totalCount = computed(() => this.allPrestations().length);
}
