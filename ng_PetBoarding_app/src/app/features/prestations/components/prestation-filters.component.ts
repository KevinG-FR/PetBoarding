import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { CategorieAnimal } from '../models/prestation.model';
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
    this.updateFilters();
  }

  onSearchChange(event: Event) {
    const target = event.target as HTMLInputElement;
    this.searchText.set(target.value);
    this.updateFilters();
  }

  clearFilters() {
    this.selectedCategory.set(null);
    this.searchText.set('');
    this.prestationsService.clearFilters();
  }

  hasActiveFilters(): boolean {
    return !!this.selectedCategory() || !!this.searchText();
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

  private updateFilters() {
    this.prestationsService.updateFilters({
      categorieAnimal: this.selectedCategory() || undefined,
      searchText: this.searchText() || undefined
    });
  }
}
