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

export interface PeriodeReservation {
  dateDebut: Date;
  dateFin: Date;
}
