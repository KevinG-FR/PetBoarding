import { Injectable, signal } from '@angular/core';
import { Pet, PetGender, PetType } from '../models/pet.model';

@Injectable({
  providedIn: 'root'
})
export class PetService {
  private readonly _pets = signal<Pet[]>([]);
  private readonly _isLoading = signal(false);

  pets = this._pets.asReadonly();
  isLoading = this._isLoading.asReadonly();

  loadUserPets(): void {
    this._isLoading.set(true);

    const mockPets: Pet[] = [
      {
        id: '1',
        name: 'Max',
        type: PetType.DOG,
        breed: 'Golden Retriever',
        age: 3,
        weight: 28,
        color: 'Doré',
        gender: PetGender.MALE,
        isNeutered: true,
        microchipNumber: '982000123456789',
        medicalNotes: 'Allergie aux puces. Traitement mensuel requis.',
        specialNeeds: "Très actif, a besoin de beaucoup d'exercice",
        photoUrl: 'assets/images/pets/max-golden.jpg',
        vaccinations: [
          {
            id: 'v1',
            name: 'Vaccin CHPPI',
            date: new Date('2024-03-15'),
            expiryDate: new Date('2025-03-15'),
            veterinarian: 'Dr. Martin - Clinique Vétérinaire du Parc'
          },
          {
            id: 'v2',
            name: 'Vaccin Rage',
            date: new Date('2024-03-15'),
            expiryDate: new Date('2027-03-15'),
            veterinarian: 'Dr. Martin - Clinique Vétérinaire du Parc'
          }
        ],
        emergencyContact: {
          name: 'Dr. Martin',
          phone: '01 23 45 67 89',
          relationship: 'Vétérinaire'
        },
        createdAt: new Date('2022-01-15'),
        updatedAt: new Date('2024-03-15')
      },
      {
        id: '2',
        name: 'Luna',
        type: PetType.CAT,
        breed: 'Maine Coon',
        age: 2,
        weight: 4.5,
        color: 'Gris et blanc',
        gender: PetGender.FEMALE,
        isNeutered: true,
        microchipNumber: '982000987654321',
        medicalNotes: 'Aucun problème médical connu.',
        specialNeeds: 'Timide avec les étrangers, préfère les environnements calmes',
        photoUrl: 'assets/images/pets/luna-mainecoon.jpg',
        vaccinations: [
          {
            id: 'v3',
            name: 'Vaccin TCL',
            date: new Date('2024-02-10'),
            expiryDate: new Date('2025-02-10'),
            veterinarian: 'Dr. Leroy - Cabinet Vétérinaire des Tilleuls'
          },
          {
            id: 'v4',
            name: 'Vaccin Leucose',
            date: new Date('2024-02-10'),
            expiryDate: new Date('2025-02-10'),
            veterinarian: 'Dr. Leroy - Cabinet Vétérinaire des Tilleuls'
          }
        ],
        emergencyContact: {
          name: 'Dr. Leroy',
          phone: '01 34 56 78 90',
          relationship: 'Vétérinaire'
        },
        createdAt: new Date('2023-06-20'),
        updatedAt: new Date('2024-02-10')
      },
      {
        id: '3',
        name: 'Charlie',
        type: PetType.RABBIT,
        breed: 'Lapin Nain',
        age: 1,
        weight: 1.2,
        color: 'Blanc avec taches marron',
        gender: PetGender.MALE,
        isNeutered: false,
        medicalNotes: 'Jeune lapin en bonne santé.',
        specialNeeds: 'Régime alimentaire strict - légumes verts uniquement',
        photoUrl: 'assets/images/pets/charlie-rabbit.jpg',
        vaccinations: [
          {
            id: 'v5',
            name: 'Vaccin Myxomatose',
            date: new Date('2024-07-01'),
            expiryDate: new Date('2024-12-01'),
            veterinarian: 'Dr. Dubois - Clinique NAC Plus'
          }
        ],
        emergencyContact: {
          name: 'Famille Dubois',
          phone: '01 45 67 89 01',
          relationship: 'Famille'
        },
        createdAt: new Date('2024-05-10'),
        updatedAt: new Date('2024-07-01')
      }
    ];

    this._pets.set(mockPets);
    this._isLoading.set(false);
  }

  getPets(): Pet[] {
    if (this._pets().length === 0) {
      this.loadUserPets();
    }
    return this._pets();
  }

  getPetById(id: string): Pet | null {
    return this._pets().find((p) => p.id === id) || null;
  }

  getPetsByType(type: PetType): Pet[] {
    return this._pets().filter((p) => p.type === type);
  }

  updatePet(petId: string, updates: Partial<Pet>): Pet {
    const pets = this._pets();
    const petIndex = pets.findIndex((p) => p.id === petId);

    if (petIndex === -1) {
      throw new Error('Animal non trouvé');
    }

    const updatedPet = {
      ...pets[petIndex],
      ...updates,
      updatedAt: new Date()
    };

    const updatedPets = [...pets];
    updatedPets[petIndex] = updatedPet;

    this._pets.set(updatedPets);

    return updatedPet;
  }

  addPet(petData: Omit<Pet, 'id' | 'createdAt' | 'updatedAt'>): Pet {
    const newPet: Pet = {
      ...petData,
      id: Date.now().toString(),
      createdAt: new Date(),
      updatedAt: new Date()
    };

    const pets = [...this._pets(), newPet];
    this._pets.set(pets);

    return newPet;
  }

  deletePet(petId: string): boolean {
    const pets = this._pets().filter((p) => p.id !== petId);
    this._pets.set(pets);

    return true;
  }
}
