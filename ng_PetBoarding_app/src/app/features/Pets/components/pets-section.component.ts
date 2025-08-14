import { CommonModule } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Pet } from '../models/pet.model';
import { PetsListComponent } from './pets-list.component';

@Component({
  selector: 'app-pets-section',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatBadgeModule, PetsListComponent],
  templateUrl: './pets-section.component.html',
  styleUrl: './pets-section.component.scss'
})
export class PetsSectionComponent {
  // Inputs
  pets = input.required<Pet[]>();
  loading = input<boolean>(false);
  highlightedPetId = input<string | null>(null);

  // Outputs
  addPet = output<void>();

  // Computed properties
  petsCount = computed(() => this.pets().length);

  onAddPet(): void {
    this.addPet.emit();
  }
}
