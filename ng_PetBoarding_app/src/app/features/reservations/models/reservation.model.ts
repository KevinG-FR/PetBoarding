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
  paymentExpiryAt?: Date; // Date d'expiration pour le paiement (20 min)
  paidAt?: Date; // Date de paiement
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
  // Nouveaux statuts avec gestion temporelle
  CREATED = 'CREATED',           // Créée, créneaux temporairement réservés (20 min max)
  VALIDATED = 'VALIDATED',       // Payée et validée, créneaux définitivement réservés
  CANCEL_AUTO = 'CANCEL_AUTO',   // Annulation automatique après expiration
  CANCEL = 'CANCEL',             // Annulation manuelle par le client
  IN_PROGRESS = 'IN_PROGRESS',   // Service en cours
  COMPLETED = 'COMPLETED',       // Service terminé
  
  // Anciens statuts pour compatibilité
  EN_ATTENTE = 'CREATED',        // @deprecated - Utiliser CREATED
  CONFIRMEE = 'VALIDATED',       // @deprecated - Utiliser VALIDATED
  EN_COURS = 'IN_PROGRESS',      // @deprecated - Utiliser IN_PROGRESS
  TERMINEE = 'COMPLETED',        // @deprecated - Utiliser COMPLETED
  ANNULEE = 'CANCEL'             // @deprecated - Utiliser CANCEL
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

export interface ValidatePaymentRequest {
  amountPaid: number;
  paymentMethod?: string;
}
