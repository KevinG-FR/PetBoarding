import { CommonModule } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { Pet } from '../models/pet.model';
import { PetCardComponent } from './pet-card.component';

@Component({
  selector: 'app-pets-list',
  standalone: true,
  imports: [CommonModule, PetCardComponent],
  templateUrl: './pets-list.component.html',
  styleUrl: './pets-list.component.scss'
})
export class PetsListComponent {
  // Inputs
  pets = input.required<Pet[]>();
  loading = input<boolean>(false);
  highlightedPetId = input<string | null>(null);

  // Outputs
  petEdit = output<Pet>();
  petDelete = output<Pet>();
  petViewDetails = output<Pet>();

  onPetEdit(pet: Pet): void {
    this.petEdit.emit(pet);
  }

  onPetDelete(pet: Pet): void {
    this.petDelete.emit(pet);
  }

  onPetViewDetails(pet: Pet): void {
    this.petViewDetails.emit(pet);
  }
}
