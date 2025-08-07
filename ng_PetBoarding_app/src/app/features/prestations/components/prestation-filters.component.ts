import { CommonModule } from '@angular/common';
import { Component, computed, inject, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CategorieAnimal, PrestationFilters } from '../models/prestation.model';
import { CategoryInfo, PrestationsService } from '../services/prestations.service';

@Component({
  selector: 'app-prestation-filters',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './prestation-filters.component.html',
  styleUrl: './prestation-filters.component.scss'
})
export class PrestationFiltersComponent {
  private prestationsService = inject(PrestationsService);
  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  filtersChanged = output<PrestationFilters>();

  categories = this.prestationsService.getCategoriesAnimaux();
  selectedCategory = signal<CategorieAnimal | null>(null);
  searchText = signal<string>('');

  // Signal computed pour les informations de catégorie
  categoriesInfo = computed(() => {
    const infos = new Map<CategorieAnimal, CategoryInfo>();
    this.categories.forEach((category) => {
      infos.set(category, this.prestationsService.getCategoryInfo(category));
    });
    return infos;
  });

  onCategoryChange(category: CategorieAnimal | null) {
    this.selectedCategory.set(category);
    this.emitFilters(); // Pas de debounce pour la catégorie
  }

  onSearchChange(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchText.set(target.value);

    // Debounce de 300ms pour la recherche textuelle
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
    }

    this.searchTimeout = setTimeout(() => {
      this.emitFilters();
    }, 300);
  }

  clearFilters() {
    this.selectedCategory.set(null);
    this.searchText.set('');

    // Annuler le timeout en cours si nécessaire
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
      this.searchTimeout = null;
    }

    this.emitFilters();
  }

  hasActiveFilters(): boolean {
    const searchText = this.searchText().trim();
    return !!this.selectedCategory() || searchText.length >= 3;
  }

  getCategoryLabel(category: CategorieAnimal): string {
    return this.categoriesInfo().get(category)?.label || category;
  }

  getCategoryIcon(category: CategorieAnimal): string {
    return this.categoriesInfo().get(category)?.icon || 'fas fa-paw';
  }

  getCategoryColor(category: CategorieAnimal): string {
    return this.categoriesInfo().get(category)?.color || '#666666';
  }

  private emitFilters() {
    const searchText = this.searchText().trim();
    const filters: PrestationFilters = {
      categorieAnimal: this.selectedCategory() || undefined,
      // N'inclure le texte de recherche que s'il y a au moins 3 caractères
      searchText: searchText.length >= 3 ? searchText : undefined
    };
    this.filtersChanged.emit(filters);
  }
}
