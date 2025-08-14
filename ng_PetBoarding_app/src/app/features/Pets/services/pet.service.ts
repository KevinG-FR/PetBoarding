import { Injectable, signal } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Pet, PetGender, PetType } from '../models/pet.model';

@Injectable({
  providedIn: 'root'
})
export class PetService {
  private pets = signal<Pet[]>([
    // Données de test - à remplacer par des appels API
    {
      id: '1',
      name: 'Rex',
      type: PetType.DOG,
      breed: 'Berger Allemand',
      age: 3,
      weight: 30,
      color: 'Noir et feu',
      gender: PetGender.MALE,
      isNeutered: true,
      microchipNumber: '123456789012345',
      medicalNotes: 'Allergique aux puces',
      specialNeeds: "Besoin de beaucoup d'exercice",
      vaccinations: [
        {
          id: '1',
          name: 'Rage',
          date: new Date('2024-01-15'),
          expiryDate: new Date('2025-01-15'),
          veterinarian: 'Dr. Martin'
        },
        {
          id: '2',
          name: 'DHPP',
          date: new Date('2024-02-01'),
          expiryDate: new Date('2025-02-01'),
          veterinarian: 'Dr. Martin'
        }
      ],
      emergencyContact: {
        name: 'Dr. Martin',
        phone: '01 23 45 67 89',
        relationship: 'Vétérinaire'
      },
      createdAt: new Date('2023-01-01'),
      updatedAt: new Date('2024-01-15')
    },
    {
      id: '2',
      name: 'Minou',
      type: PetType.CAT,
      breed: 'Persan',
      age: 2,
      weight: 4.5,
      color: 'Blanc',
      gender: PetGender.FEMALE,
      isNeutered: true,
      vaccinations: [
        {
          id: '3',
          name: 'Typhus',
          date: new Date('2024-03-01'),
          expiryDate: new Date('2025-03-01'),
          veterinarian: 'Dr. Dupont'
        }
      ],
      createdAt: new Date('2023-06-01'),
      updatedAt: new Date('2024-03-01')
    }
  ]);

  getPets(): Observable<Pet[]> {
    return of(this.pets());
  }

  getPetById(id: string): Observable<Pet | undefined> {
    const pet = this.pets().find((p) => p.id === id);
    return of(pet);
  }

  updatePet(id: string, petData: Partial<Pet>): Observable<Pet> {
    const currentPets = this.pets();
    const index = currentPets.findIndex((p) => p.id === id);

    if (index === -1) {
      throw new Error('Pet not found');
    }

    const updatedPet: Pet = {
      ...currentPets[index],
      ...petData,
      updatedAt: new Date()
    };

    const newPets = [...currentPets];
    newPets[index] = updatedPet;

    this.pets.set(newPets);
    return of(updatedPet);
  }

  addPet(petData: Omit<Pet, 'id' | 'createdAt' | 'updatedAt'>): Observable<Pet> {
    const newPet: Pet = {
      ...petData,
      id: Math.random().toString(36).substr(2, 9),
      createdAt: new Date(),
      updatedAt: new Date()
    };

    this.pets.update((pets) => [...pets, newPet]);
    return of(newPet);
  }

  deletePet(id: string): Observable<void> {
    this.pets.update((pets) => pets.filter((p) => p.id !== id));
    return of(void 0);
  }
}
