import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
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

  // Propriétés exposées pour le template
  filteredPrestations = this.prestationsService.getFilteredPrestations();
  allPrestations = this.prestationsService.getAllPrestations();

  // Computed pour les statistiques
  hasResults = computed(() => this.filteredPrestations().length > 0);
  isFiltered = computed(() => this.filteredPrestations().length < this.allPrestations().length);
  resultCount = computed(() => this.filteredPrestations().length);
  totalCount = computed(() => this.allPrestations().length);
}
