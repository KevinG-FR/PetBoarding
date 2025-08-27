import { Injectable, inject, signal } from '@angular/core';
import { Observable, catchError, map, of, tap } from 'rxjs';
import { PetApiService } from '../../../shared/services/pet-api.service';
import { PetDto, CreatePetRequest, UpdatePetRequest } from '../../../shared/contracts/pets/pet.dto';
import { Pet, PetGender, PetType } from '../models/pet.model';
import { PetFormData } from '../models/pet-form.model';

// Mappers pour convertir entre les enums frontend (strings) et backend (integers)
const BackendPetTypeMap = {
  [PetType.DOG]: 1,    // Chien
  [PetType.CAT]: 2,    // Chat  
  [PetType.BIRD]: 3,   // Oiseau
  [PetType.RABBIT]: 4, // Lapin
  [PetType.HAMSTER]: 5 // Hamster
} as const;

const FrontendPetTypeMap = {
  1: PetType.DOG,
  2: PetType.CAT,
  3: PetType.BIRD,
  4: PetType.RABBIT,
  5: PetType.HAMSTER
} as const;

const BackendPetGenderMap = {
  [PetGender.MALE]: 1,
  [PetGender.FEMALE]: 2
} as const;

const FrontendPetGenderMap = {
  1: PetGender.MALE,
  2: PetGender.FEMALE
} as const;

@Injectable({
  providedIn: 'root'
})
export class PetService {
  private readonly petApiService = inject(PetApiService);

  // Signaux pour stocker les pets en cache local
  private readonly _pets = signal<Pet[]>([]);
  private readonly _isLoading = signal(false);
  private readonly _error = signal<string | null>(null);

  // Getters publics readonly
  pets = this._pets.asReadonly();
  isLoading = this._isLoading.asReadonly();
  error = this._error.asReadonly();

  /**
   * Convertit un PetDto en Pet
   */
  private mapDtoToPet(dto: PetDto): Pet {
    return {
      id: dto.id,
      name: dto.name,
      type: FrontendPetTypeMap[dto.type as keyof typeof FrontendPetTypeMap],
      breed: dto.breed,
      age: dto.age,
      weight: dto.weight,
      color: dto.color,
      gender: FrontendPetGenderMap[dto.gender as keyof typeof FrontendPetGenderMap],
      isNeutered: dto.isNeutered,
      microchipNumber: dto.microchipNumber,
      medicalNotes: dto.medicalNotes,
      specialNeeds: dto.specialNeeds,
      photoUrl: dto.photoUrl,
      emergencyContact: dto.emergencyContact ? {
        name: dto.emergencyContact.name,
        phone: dto.emergencyContact.phone,
        relationship: dto.emergencyContact.relationship
      } : undefined,
      // Pour l'instant, pas de vaccinations dans l'API - on met un tableau vide
      vaccinations: [],
      createdAt: new Date(dto.createdAt),
      updatedAt: new Date(dto.updatedAt)
    };
  }

  /**
   * Convertit un PetFormData en CreatePetRequest
   */
  private mapFormDataToCreateRequest(formData: PetFormData): CreatePetRequest {
    return {
      name: formData.name,
      type: BackendPetTypeMap[formData.type],
      breed: formData.breed,
      age: formData.age,
      weight: formData.weight,
      color: formData.color,
      gender: BackendPetGenderMap[formData.gender],
      isNeutered: formData.isNeutered,
      microchipNumber: formData.microchipNumber,
      medicalNotes: formData.medicalNotes,
      specialNeeds: formData.specialNeeds,
      photoUrl: formData.photoUrl,
      emergencyContactName: formData.emergencyContact?.name,
      emergencyContactPhone: formData.emergencyContact?.phone,
      emergencyContactRelationship: formData.emergencyContact?.relationship
    };
  }

  /**
   * Convertit des updates Pet en UpdatePetRequest
   */
  private mapPetToUpdateRequest(pet: Pet): UpdatePetRequest {
    return {
      name: pet.name,
      type: BackendPetTypeMap[pet.type],
      breed: pet.breed,
      age: pet.age,
      weight: pet.weight,
      color: pet.color,
      gender: BackendPetGenderMap[pet.gender],
      isNeutered: pet.isNeutered,
      microchipNumber: pet.microchipNumber,
      medicalNotes: pet.medicalNotes,
      specialNeeds: pet.specialNeeds,
      photoUrl: pet.photoUrl,
      emergencyContactName: pet.emergencyContact?.name,
      emergencyContactPhone: pet.emergencyContact?.phone,
      emergencyContactRelationship: pet.emergencyContact?.relationship
    };
  }

  /**
   * Charge tous les pets de l'utilisateur connecté depuis l'API
   */
  loadUserPets(filters?: { type?: PetType }): Observable<Pet[]> {
    this._isLoading.set(true);
    this._error.set(null);

    return this.petApiService.getPets(filters).pipe(
      map((response) => response.pets.map((dto) => this.mapDtoToPet(dto))),
      tap((pets) => {
        this._pets.set(pets);
        this._isLoading.set(false);
      }),
      catchError((error) => {
        this._error.set('Erreur lors du chargement des animaux');
        this._isLoading.set(false);
        console.error('Erreur lors du chargement des pets:', error);
        return of([]);
      })
    );
  }

  /**
   * Récupère tous les pets (depuis le cache local ou charge depuis l'API)
   */
  getPets(): Pet[] {
    if (this._pets().length === 0) {
      // Si pas de données en cache, charge depuis l'API
      this.loadUserPets().subscribe();
    }
    return this._pets();
  }

  /**
   * Récupère un pet par son ID depuis l'API
   */
  getPetById(id: string): Observable<Pet | null> {
    return this.petApiService.getPetById(id).pipe(
      map((response) => this.mapDtoToPet(response.pet)),
      catchError((error) => {
        console.error('Erreur lors de la récupération du pet:', error);
        return of(null);
      })
    );
  }

  /**
   * Version synchrone pour récupérer un pet depuis le cache
   */
  getPetByIdFromCache(id: string): Pet | null {
    return this._pets().find((p) => p.id === id) || null;
  }

  /**
   * Récupère les pets par type depuis le cache local
   */
  getPetsByType(type: PetType): Pet[] {
    return this._pets().filter((p) => p.type === type);
  }

  /**
   * Crée un nouveau pet
   */
  addPet(petData: PetFormData): Observable<Pet | null> {
    const request = this.mapFormDataToCreateRequest(petData);

    return this.petApiService.createPet(request).pipe(
      tap((response) => {
        if (response.pet) {
          // Recharge les pets après création
          this.loadUserPets().subscribe();
        }
      }),
      map((response) => response.pet ? this.mapDtoToPet(response.pet) : null),
      catchError((error) => {
        console.error('Erreur lors de la création du pet:', error);
        this._error.set('Erreur lors de la création de l\'animal');
        return of(null);
      })
    );
  }

  /**
   * Met à jour un pet existant
   */
  updatePet(petId: string, updates: Partial<Pet>): Observable<Pet | null> {
    // Récupérer le pet actuel pour créer la requête complète
    const currentPet = this.getPetByIdFromCache(petId);
    if (!currentPet) {
      this._error.set('Animal non trouvé');
      return of(null);
    }

    // Appliquer les mises à jour
    const updatedPet = { ...currentPet, ...updates };
    const request = this.mapPetToUpdateRequest(updatedPet);

    return this.petApiService.updatePet(petId, request).pipe(
      tap((response) => {
        if (response.pet) {
          // Recharge les pets après mise à jour
          this.loadUserPets().subscribe();
        }
      }),
      map((response) => response.pet ? this.mapDtoToPet(response.pet) : null),
      catchError((error) => {
        console.error('Erreur lors de la mise à jour du pet:', error);
        this._error.set('Erreur lors de la mise à jour de l\'animal');
        return of(null);
      })
    );
  }

  /**
   * Supprime un pet
   */
  deletePet(petId: string): Observable<boolean> {
    return this.petApiService.deletePet(petId).pipe(
      tap((response) => {
        if (response.success) {
          // Recharge les pets après suppression
          this.loadUserPets().subscribe();
        }
      }),
      map((response) => response.success),
      catchError((error) => {
        console.error('Erreur lors de la suppression du pet:', error);
        this._error.set('Erreur lors de la suppression de l\'animal');
        return of(false);
      })
    );
  }
}
