import { ReservationDto } from '../../../shared/services/reservations-api.service';
import { Reservation, StatutReservation } from '../models/reservation.model';

/**
 * Mapper pour convertir entre DTOs backend et modèles frontend
 */
export class ReservationMapper {
  /**
   * Convertit un DTO backend vers un modèle frontend
   */
  static fromDto(dto: ReservationDto): Reservation {
    return {
      id: dto.id,
      userId: dto.userId,
      animalId: dto.animalId,
      animalNom: dto.animalName,
      animalType: 'CHIEN', // TODO: À enrichir avec les vraies données de l'animal
      prestationId: dto.serviceId,
      prestationLibelle: '', // TODO: À enrichir avec les données de prestation
      dateDebut: new Date(dto.startDate),
      dateFin: dto.endDate ? new Date(dto.endDate) : undefined,
      prix: dto.totalPrice,
      statut: this.mapStatusFromBackend(dto.status),
      commentaires: dto.comments,
      dateCreation: new Date(dto.createdAt),
      dateModification: dto.updatedAt ? new Date(dto.updatedAt) : undefined,
      slotsReserves: []
    };
  }

  /**
   * Convertit plusieurs DTOs backend vers modèles frontend
   */
  static fromDtoArray(dtos: ReservationDto[]): Reservation[] {
    return dtos.map(dto => this.fromDto(dto));
  }

  /**
   * Convertit un modèle frontend vers un DTO backend
   */
  static toDto(reservation: Reservation): ReservationDto {
    return {
      id: reservation.id,
      userId: reservation.userId,
      animalId: reservation.animalId,
      animalName: reservation.animalNom,
      serviceId: reservation.prestationId,
      startDate: reservation.dateDebut.toISOString(),
      endDate: reservation.dateFin?.toISOString(),
      comments: reservation.commentaires,
      status: this.mapStatusToBackend(reservation.statut),
      createdAt: reservation.dateCreation.toISOString(),
      updatedAt: reservation.dateModification?.toISOString(),
      totalPrice: reservation.prix
    };
  }

  /**
   * Mappe le statut du backend vers le frontend
   */
  private static mapStatusFromBackend(backendStatus: string): StatutReservation {
    switch (backendStatus.toUpperCase()) {
      case 'CREATED':
        return StatutReservation.CREATED;
      case 'VALIDATED':
        return StatutReservation.VALIDATED;
      case 'CANCEL_AUTO':
        return StatutReservation.CANCEL_AUTO;
      case 'CANCEL':
        return StatutReservation.CANCEL;
      case 'IN_PROGRESS':
        return StatutReservation.IN_PROGRESS;
      case 'COMPLETED':
        return StatutReservation.COMPLETED;
      default:
        return StatutReservation.CREATED;
    }
  }

  /**
   * Mappe le statut du frontend vers le backend
   */
  private static mapStatusToBackend(frontendStatus: StatutReservation): string {
    // Retourne toujours la valeur moderne (pas les valeurs deprecated)
    switch (frontendStatus) {
      case StatutReservation.CREATED:
      case StatutReservation.EN_ATTENTE:
        return 'CREATED';
      case StatutReservation.VALIDATED:
      case StatutReservation.CONFIRMEE:
        return 'VALIDATED';
      case StatutReservation.CANCEL_AUTO:
        return 'CANCEL_AUTO';
      case StatutReservation.CANCEL:
      case StatutReservation.ANNULEE:
        return 'CANCEL';
      case StatutReservation.IN_PROGRESS:
      case StatutReservation.EN_COURS:
        return 'IN_PROGRESS';
      case StatutReservation.COMPLETED:
      case StatutReservation.TERMINEE:
        return 'COMPLETED';
      default:
        return 'CREATED';
    }
  }

  /**
   * Enrichit une réservation avec les données de prestation
   */
  static enrichWithPrestationData(
    reservation: Reservation, 
    prestationLibelle: string,
    prixUnitaire: number,
    nombreJours: number = 1
  ): Reservation {
    return {
      ...reservation,
      prestationLibelle,
      prix: reservation.prix ?? (prixUnitaire * nombreJours)
    };
  }

  /**
   * Calcule le nombre de jours d'une réservation
   */
  static calculateNumberOfDays(dateDebut: Date, dateFin?: Date): number {
    if (!dateFin) return 1;
    
    const diffTime = dateFin.getTime() - dateDebut.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return Math.max(1, diffDays);
  }

  /**
   * Vérifie si une réservation nécessite un paiement
   */
  static requiresPayment(reservation: Reservation): boolean {
    return reservation.statut === StatutReservation.CREATED;
  }

  /**
   * Vérifie si une réservation peut être annulée
   */
  static canBeCancelled(reservation: Reservation): boolean {
    const nonCancellableStatuses = [
      StatutReservation.COMPLETED,
      StatutReservation.TERMINEE,
      StatutReservation.CANCEL,
      StatutReservation.ANNULEE,
      StatutReservation.CANCEL_AUTO
    ];
    
    return !nonCancellableStatuses.includes(reservation.statut);
  }
}