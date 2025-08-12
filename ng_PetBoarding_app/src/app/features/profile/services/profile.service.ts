import { Injectable, inject, signal } from '@angular/core';
import { Observable, delay, of, tap } from 'rxjs';
import { AuthService } from '../../auth/services/auth.service';
import { Pet, PetGender, PetType } from '../../pets/models/pet.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly authService = inject(AuthService);

  // Signals pour l'état du profil
  private readonly _pets = signal<Pet[]>([]);
  private readonly _isLoading = signal(false);

  // Getters publics
  pets = this._pets.asReadonly();
  isLoading = this._isLoading.asReadonly();

  /**
   * Charger les animaux de l'utilisateur connecté
   */
  loadUserPets(): Observable<Pet[]> {
    this._isLoading.set(true);

    // Simulation d'un appel API avec des données factices
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
        createdAt: new Date('2024-05-10'),
        updatedAt: new Date('2024-07-01')
      }
    ];

    return of(mockPets).pipe(
      delay(800),
      tap((pets) => {
        this._pets.set(pets);
        this._isLoading.set(false);
      })
    );
  }

  /**
   * Obtenir un animal par son ID
   */
  getPetById(id: string): Observable<Pet | null> {
    const pet = this._pets().find((p) => p.id === id) || null;
    return of(pet);
  }

  /**
   * Mettre à jour les informations d'un animal
   */
  updatePet(petId: string, updates: Partial<Pet>): Observable<Pet> {
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

    return of(updatedPet).pipe(delay(500));
  }

  /**
   * Ajouter un nouvel animal
   */
  addPet(petData: Omit<Pet, 'id' | 'createdAt' | 'updatedAt'>): Observable<Pet> {
    const newPet: Pet = {
      ...petData,
      id: Date.now().toString(),
      createdAt: new Date(),
      updatedAt: new Date()
    };

    const pets = [...this._pets(), newPet];
    this._pets.set(pets);

    return of(newPet).pipe(delay(500));
  }

  /**
   * Supprimer un animal
   */
  deletePet(petId: string): Observable<boolean> {
    const pets = this._pets().filter((p) => p.id !== petId);
    this._pets.set(pets);

    return of(true).pipe(delay(300));
  }
}
