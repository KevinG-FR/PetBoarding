import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { PetType, PetTypeLabels } from '../../../pets/models/pet.model';

export interface PrestationFilters {
  categorieAnimal?: PetType;
  searchText?: string;
  estDisponible?: boolean;
}

@Component({
  selector: 'app-admin-prestation-filters',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatCheckboxModule
  ],
  templateUrl: './admin-prestation-filters.component.html',
  styleUrl: './admin-prestation-filters.component.scss'
})
export class AdminPrestationFiltersComponent implements OnInit {
  @Input() filters: PrestationFilters = {};
  @Output() filtersChange = new EventEmitter<PrestationFilters>();

  filtersForm: FormGroup;
  petTypes = Object.values(PetType).map((type) => ({
    value: type,
    label: PetTypeLabels[type]
  }));

  constructor(private fb: FormBuilder) {
    this.filtersForm = this.fb.group({
      searchText: [''],
      categorieAnimal: [null],
      estDisponible: [null]
    });
  }

  ngOnInit(): void {
    // Initialiser le formulaire avec les filtres fournis
    this.filtersForm.patchValue({
      searchText: this.filters.searchText || '',
      categorieAnimal: this.filters.categorieAnimal || null,
      estDisponible: this.filters.estDisponible !== undefined ? this.filters.estDisponible : null
    });

    // S'abonner aux changements du formulaire
    this.filtersForm.valueChanges.subscribe((value) => {
      const cleanFilters: PrestationFilters = {};

      if (value.searchText && value.searchText.trim()) {
        cleanFilters.searchText = value.searchText.trim();
      }

      if (value.categorieAnimal !== null && value.categorieAnimal !== undefined) {
        cleanFilters.categorieAnimal = value.categorieAnimal;
      }

      if (value.estDisponible !== null && value.estDisponible !== undefined) {
        cleanFilters.estDisponible = value.estDisponible;
      }

      this.filtersChange.emit(cleanFilters);
    });
  }

  clearFilters(): void {
    this.filtersForm.reset({
      searchText: '',
      categorieAnimal: null,
      estDisponible: null
    });
  }

  hasActiveFilters(): boolean {
    const formValue = this.filtersForm.value;
    return !!(
      (formValue.searchText && formValue.searchText.trim()) ||
      (formValue.categorieAnimal !== null && formValue.categorieAnimal !== undefined) ||
      (formValue.estDisponible !== null && formValue.estDisponible !== undefined)
    );
  }
}
