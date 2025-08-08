import { Injectable, signal } from '@angular/core';
import { Reservation, ReservationFilters, StatutReservation } from '../models/reservation.model';

export interface StatutInfo {
  label: string;
  icon: string;
  color: string;
  bgColor: string;
}

@Injectable({
  providedIn: 'root'
})
export class ReservationsService {
  private reservations = signal<Reservation[]>([
    {
      id: '1',
      animalNom: 'Rex',
      animalType: 'CHIEN',
      prestationId: '1',
      prestationLibelle: 'Pension complète',
      dateDebut: new Date('2025-08-15'),
      dateFin: new Date('2025-08-20'),
      prix: 175, // 5 jours × 35€
      statut: StatutReservation.CONFIRMEE,
      commentaires: 'Rex adore jouer avec les autres chiens',
      dateCreation: new Date('2025-07-20'),
      dateReservation: new Date('2025-07-20')
    },
    {
      id: '2',
      animalNom: 'Minou',
      animalType: 'CHAT',
      prestationId: '6',
      prestationLibelle: 'Pension chat',
      dateDebut: new Date('2025-08-10'),
      dateFin: new Date('2025-08-12'),
      prix: 50, // 2 jours × 25€
      statut: StatutReservation.EN_COURS,
      commentaires: 'Chat très timide, préfère la tranquillité',
      dateCreation: new Date('2025-07-25'),
      dateReservation: new Date('2025-07-25')
    },
    {
      id: '3',
      animalNom: 'Buddy',
      animalType: 'CHIEN',
      prestationId: '2',
      prestationLibelle: 'Garderie journée',
      dateDebut: new Date('2025-07-30'),
      dateFin: new Date('2025-07-30'),
      prix: 25,
      statut: StatutReservation.TERMINEE,
      dateCreation: new Date('2025-07-25'),
      dateReservation: new Date('2025-07-25')
    },
    {
      id: '4',
      animalNom: 'Bella',
      animalType: 'CHIEN',
      prestationId: '4',
      prestationLibelle: 'Toilettage complet',
      dateDebut: new Date('2025-08-25'),
      dateFin: new Date('2025-08-25'),
      prix: 45,
      statut: StatutReservation.EN_ATTENTE,
      commentaires: 'Première visite, chienne un peu anxieuse',
      dateCreation: new Date('2025-08-01'),
      dateReservation: new Date('2025-08-01')
    },
    {
      id: '5',
      animalNom: 'Whiskers',
      animalType: 'CHAT',
      prestationId: '5',
      prestationLibelle: 'Garde à domicile',
      dateDebut: new Date('2025-07-20'),
      dateFin: new Date('2025-07-22'),
      prix: 60, // 3 visites × 20€
      statut: StatutReservation.TERMINEE,
      dateCreation: new Date('2025-07-10'),
      dateReservation: new Date('2025-07-10')
    },
    {
      id: '6',
      animalNom: 'Max',
      animalType: 'CHIEN',
      prestationId: '1',
      prestationLibelle: 'Pension complète',
      dateDebut: new Date('2025-09-05'),
      dateFin: new Date('2025-09-12'),
      prix: 245, // 7 jours × 35€
      statut: StatutReservation.CONFIRMEE,
      commentaires: 'Chien très sociable et énergique',
      dateCreation: new Date('2025-08-05'),
      dateReservation: new Date('2025-08-05')
    },
    // Nouveaux exemples pour la pagination
    {
      id: '7',
      animalNom: 'Luna',
      animalType: 'CHAT',
      prestationId: '6',
      prestationLibelle: 'Pension chat',
      dateDebut: new Date('2025-08-28'),
      dateFin: new Date('2025-09-02'),
      prix: 125, // 5 jours × 25€
      statut: StatutReservation.CONFIRMEE,
      commentaires: 'Chatte calme et affectueuse',
      dateCreation: new Date('2025-08-10'),
      dateReservation: new Date('2025-08-10')
    },
    {
      id: '8',
      animalNom: 'Rocky',
      animalType: 'CHIEN',
      prestationId: '3',
      prestationLibelle: 'Promenade individuelle',
      dateDebut: new Date('2025-08-12'),
      dateFin: new Date('2025-08-12'),
      prix: 20,
      statut: StatutReservation.TERMINEE,
      dateCreation: new Date('2025-08-08'),
      dateReservation: new Date('2025-08-08')
    },
    {
      id: '9',
      animalNom: 'Simba',
      animalType: 'CHAT',
      prestationId: '4',
      prestationLibelle: 'Toilettage chat',
      dateDebut: new Date('2025-09-15'),
      dateFin: new Date('2025-09-15'),
      prix: 35,
      statut: StatutReservation.EN_ATTENTE,
      commentaires: 'Chat persan, toilettage spécialisé requis',
      dateCreation: new Date('2025-08-15'),
      dateReservation: new Date('2025-08-15')
    },
    {
      id: '10',
      animalNom: 'Charlie',
      animalType: 'CHIEN',
      prestationId: '2',
      prestationLibelle: 'Garderie journée',
      dateDebut: new Date('2025-08-20'),
      dateFin: new Date('2025-08-20'),
      prix: 25,
      statut: StatutReservation.CONFIRMEE,
      dateCreation: new Date('2025-08-15'),
      dateReservation: new Date('2025-08-15')
    },
    {
      id: '11',
      animalNom: 'Daisy',
      animalType: 'CHIEN',
      prestationId: '1',
      prestationLibelle: 'Pension complète',
      dateDebut: new Date('2025-09-20'),
      dateFin: new Date('2025-09-25'),
      prix: 175, // 5 jours × 35€
      statut: StatutReservation.CONFIRMEE,
      commentaires: 'Chienne très douce avec les enfants',
      dateCreation: new Date('2025-08-20'),
      dateReservation: new Date('2025-08-20')
    },
    {
      id: '12',
      animalNom: 'Mittens',
      animalType: 'CHAT',
      prestationId: '5',
      prestationLibelle: 'Garde à domicile',
      dateDebut: new Date('2025-08-30'),
      dateFin: new Date('2025-09-01'),
      prix: 40, // 2 visites × 20€
      statut: StatutReservation.EN_COURS,
      dateCreation: new Date('2025-08-25'),
      dateReservation: new Date('2025-08-25')
    },
    {
      id: '13',
      animalNom: 'Bruno',
      animalType: 'CHIEN',
      prestationId: '4',
      prestationLibelle: 'Toilettage complet',
      dateDebut: new Date('2025-07-15'),
      dateFin: new Date('2025-07-15'),
      prix: 45,
      statut: StatutReservation.TERMINEE,
      dateCreation: new Date('2025-07-10'),
      dateReservation: new Date('2025-07-10')
    },
    {
      id: '14',
      animalNom: 'Princess',
      animalType: 'CHAT',
      prestationId: '6',
      prestationLibelle: 'Pension chat',
      dateDebut: new Date('2025-10-05'),
      dateFin: new Date('2025-10-10'),
      prix: 125, // 5 jours × 25€
      statut: StatutReservation.EN_ATTENTE,
      commentaires: 'Chat de race, soins particuliers requis',
      dateCreation: new Date('2025-09-01'),
      dateReservation: new Date('2025-09-01')
    },
    {
      id: '15',
      animalNom: 'Zeus',
      animalType: 'CHIEN',
      prestationId: '3',
      prestationLibelle: 'Promenade individuelle',
      dateDebut: new Date('2025-08-18'),
      dateFin: new Date('2025-08-18'),
      prix: 20,
      statut: StatutReservation.CONFIRMEE,
      dateCreation: new Date('2025-08-12'),
      dateReservation: new Date('2025-08-12')
    }
  ]);

  // Getters pour exposer les signals
  getAllReservations() {
    return this.reservations.asReadonly();
  }

  // Méthode pour créer des filtres locaux par composant
  createFilteredReservations(reservations: Reservation[], filters: ReservationFilters) {
    return reservations.filter((reservation) => {
      // Filtre par date de début
      if (filters.dateDebut) {
        if (reservation.dateDebut < filters.dateDebut) {
          return false;
        }
      }

      // Filtre par date de fin
      if (filters.dateFin) {
        if (reservation.dateDebut > filters.dateFin) {
          return false;
        }
      }

      // Filtre par statut
      if (filters.statut && reservation.statut !== filters.statut) {
        return false;
      }

      // Filtre par type d'animal
      if (filters.animalType && reservation.animalType !== filters.animalType) {
        return false;
      }

      return true;
    });
  }

  // Méthodes CRUD (pour plus tard)
  getReservationById(id: string): Reservation | undefined {
    return this.reservations().find((r) => r.id === id);
  }

  getStatuts(): StatutReservation[] {
    return Object.values(StatutReservation);
  }

  // Méthodes utilitaires pour l'affichage
  getStatutInfo(statut: StatutReservation): StatutInfo {
    switch (statut) {
      case StatutReservation.EN_ATTENTE:
        return {
          label: 'En attente',
          icon: 'fas fa-clock',
          color: '#ff9800',
          bgColor: '#fff3e0'
        };
      case StatutReservation.CONFIRMEE:
        return {
          label: 'Confirmée',
          icon: 'fas fa-check-circle',
          color: '#4caf50',
          bgColor: '#e8f5e8'
        };
      case StatutReservation.EN_COURS:
        return {
          label: 'En cours',
          icon: 'fas fa-play-circle',
          color: '#2196f3',
          bgColor: '#e3f2fd'
        };
      case StatutReservation.TERMINEE:
        return {
          label: 'Terminée',
          icon: 'fas fa-check-double',
          color: '#8bc34a',
          bgColor: '#f1f8e9'
        };
      case StatutReservation.ANNULEE:
        return {
          label: 'Annulée',
          icon: 'fas fa-times-circle',
          color: '#f44336',
          bgColor: '#ffebee'
        };
      default:
        return {
          label: statut,
          icon: 'fas fa-question-circle',
          color: '#666666',
          bgColor: '#f5f5f5'
        };
    }
  }

  getAnimalIcon(type: 'CHIEN' | 'CHAT'): string {
    return type === 'CHIEN' ? 'fas fa-dog' : 'fas fa-cat';
  }

  getAnimalColor(type: 'CHIEN' | 'CHAT'): string {
    return type === 'CHIEN' ? '#8bc34a' : '#ff9800';
  }

  // Méthodes de statistiques
  getReservationsCount(): number {
    return this.reservations().length;
  }

  getReservationsParStatut(statut: StatutReservation): number {
    return this.reservations().filter((r) => r.statut === statut).length;
  }

  getChiffreAffaires(): number {
    return this.reservations()
      .filter((r) => r.statut === StatutReservation.TERMINEE)
      .reduce((total, r) => total + r.prix, 0);
  }
}
