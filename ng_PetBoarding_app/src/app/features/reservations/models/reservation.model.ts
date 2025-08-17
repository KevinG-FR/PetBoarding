export interface Reservation {
  id: string;
  animalNom: string;
  animalType: 'CHIEN' | 'CHAT';
  prestationId: string;
  prestationLibelle: string;
  dateDebut: Date;
  dateFin: Date;
  prix: number;
  statut: StatutReservation;
  commentaires?: string;
  dateCreation: Date;
  dateReservation: Date;
}

export interface ReservationAvecPlanning {
  id: string;
  utilisateurId: string;
  animalId: string;
  animalNom: string;
  animalType: 'CHIEN' | 'CHAT';
  prestationId: string;
  prestationLibelle: string;
  dateDebut: Date;
  dateFin?: Date;
  prixTotal?: number;
  statut: StatutReservation;
  commentaires?: string;
  dateCreation: Date;
  dateModification?: Date;
  datesReservees: Date[];
  nombreJours: number;
}

export enum StatutReservation {
  EN_ATTENTE = 'EN_ATTENTE',
  CONFIRMEE = 'CONFIRMEE',
  EN_COURS = 'EN_COURS',
  TERMINEE = 'TERMINEE',
  ANNULEE = 'ANNULEE'
}

export interface ReservationFilters {
  dateDebut?: Date;
  dateFin?: Date;
  statut?: StatutReservation;
  animalType?: 'CHIEN' | 'CHAT';
}

export interface CreerReservationRequest {
  animalId: string;
  prestationId: string;
  dateDebut: Date;
  dateFin?: Date;
  commentaires?: string;
}

export interface PeriodeReservation {
  dateDebut: Date;
  dateFin: Date;
}
