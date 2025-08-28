import { inject, Injectable, signal } from '@angular/core';
import { map, Observable, of, switchMap, catchError, tap, throwError } from 'rxjs';
import { PlanningService } from '../../prestations/services/planning.service';
import { PrestationsService } from '../../prestations/services/prestations.service';
import { ReservationsApiService, CreateReservationRequest } from '../../../shared/services/reservations-api.service';
import { ReservationMapper } from '../mappers/reservation.mapper';
import {
  CreerReservationRequest,
  Reservation,
  ReservationFilters,
  StatutReservation,
  ReservationAvecSlots
} from '../models/reservation.model';

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
  private planningService = inject(PlanningService);
  private prestationsService = inject(PrestationsService);
  private reservationsApi = inject(ReservationsApiService);
  
  // Signals pour l'état
  private reservations = signal<Reservation[]>([]);
  private loading = signal<boolean>(false);
  private error = signal<string | null>(null);

  // Getters pour exposer les signals
  getAllReservations() {
    return this.reservations.asReadonly();
  }

  getLoading() {
    return this.loading.asReadonly();
  }

  getError() {
    return this.error.asReadonly();
  }

  /**
   * Charge toutes les réservations depuis l'API
   */
  loadReservations(): Observable<Reservation[]> {
    this.loading.set(true);
    this.error.set(null);

    return this.reservationsApi.getAllReservations().pipe(
      map(response => {
        if (response.success && response.data) {
          return ReservationMapper.fromDtoArray(response.data);
        }
        return [];
      }),
      tap(reservations => {
        this.reservations.set(reservations);
        this.loading.set(false);
      }),
      catchError(error => {
        console.error('Erreur lors du chargement des réservations:', error);
        this.error.set('Impossible de charger les réservations');
        this.loading.set(false);
        return of([]);
      })
    );
  }

  /**
   * Charge les réservations d'un utilisateur spécifique
   */
  loadUserReservations(userId: string): Observable<Reservation[]> {
    this.loading.set(true);
    this.error.set(null);

    return this.reservationsApi.getUserReservations(userId).pipe(
      map(response => {
        if (response.success && response.data) {
          return ReservationMapper.fromDtoArray(response.data);
        }
        return [];
      }),
      tap(reservations => {
        this.reservations.set(reservations);
        this.loading.set(false);
      }),
      catchError(error => {
        console.error('Erreur lors du chargement des réservations utilisateur:', error);
        this.error.set('Impossible de charger les réservations');
        this.loading.set(false);
        return of([]);
      })
    );
  }

  /**
   * Récupère une réservation par son ID
   */
  getReservationById(id: string): Observable<Reservation | null> {
    return this.reservationsApi.getReservationById(id).pipe(
      map(response => {
        if (response.success && response.data) {
          return ReservationMapper.fromDto(response.data);
        }
        return null;
      }),
      catchError(error => {
        console.error('Erreur lors du chargement de la réservation:', error);
        return of(null);
      })
    );
  }

  /**
   * Crée une nouvelle réservation avec gestion du planning et des slots
   */
  creerReservationAvecPlanning(request: CreerReservationRequest): Observable<Reservation | null> {
    return this.prestationsService.getPrestationById(request.prestationId).pipe(
      switchMap((prestation) => {
        if (!prestation) {
          return throwError(() => new Error('Prestation non trouvée'));
        }

        // Vérifier la disponibilité
        return this.planningService.verifierDisponibilite({
          prestationId: request.prestationId,
          startDate: request.dateDebut,
          endDate: request.dateFin,
          quantity: 1
        }).pipe(
          switchMap((disponibilite) => {
            if (!disponibilite.isAvailable) {
              return throwError(() => new Error('Créneaux non disponibles'));
            }

            // Créer la réservation via l'API
            const createRequest: CreateReservationRequest = {
              userId: request.userId,
              animalId: request.animalId,
              animalName: request.animalName,
              serviceId: request.prestationId,
              startDate: request.dateDebut.toISOString(),
              endDate: request.dateFin?.toISOString(),
              comments: request.commentaires
            };

            return this.reservationsApi.createReservation(createRequest).pipe(
              switchMap((response) => {
                const newReservation = ReservationMapper.fromDto(response.reservation);
                
                // Réserver les créneaux dans le planning
                return this.planningService.reserverCreneaux(
                  request.prestationId,
                  request.dateDebut,
                  request.dateFin || null,
                  1
                ).pipe(
                  map((reservationReussie) => {
                    if (reservationReussie) {
                      // Mettre à jour le cache local
                      const reservationsActuelles = this.reservations();
                      this.reservations.set([...reservationsActuelles, newReservation]);
                      return newReservation;
                    }
                    return null;
                  })
                );
              })
            );
          })
        );
      }),
      catchError(error => {
        console.error('Erreur lors de la création de la réservation:', error);
        this.error.set(error.message || 'Erreur lors de la création de la réservation');
        return of(null);
      })
    );
  }

  /**
   * Annule une réservation et libère les créneaux
   */
  annulerReservationAvecPlanning(reservationId: string): Observable<boolean> {
    const reservationsActuelles = this.reservations();
    const reservation = reservationsActuelles.find((r) => r.id === reservationId);

    if (!reservation || !ReservationMapper.canBeCancelled(reservation)) {
      return of(false);
    }

    return this.reservationsApi.cancelReservation(reservationId).pipe(
      switchMap((response) => {
        if (response.success) {
          // Libérer les créneaux dans le planning
          return this.planningService.annulerReservations(
            reservation.prestationId,
            reservation.dateDebut,
            reservation.dateFin || null,
            1
          ).pipe(
            map((annulationReussie) => {
              if (annulationReussie) {
                // Mettre à jour le statut local
                const reservationIndex = reservationsActuelles.findIndex((r) => r.id === reservationId);
                if (reservationIndex !== -1) {
                  reservationsActuelles[reservationIndex] = {
                    ...reservationsActuelles[reservationIndex],
                    statut: StatutReservation.CANCEL,
                    dateModification: new Date()
                  };
                  this.reservations.set([...reservationsActuelles]);
                }
              }
              return annulationReussie;
            })
          );
        }
        return of(false);
      }),
      catchError(error => {
        console.error('Erreur lors de l\'annulation:', error);
        this.error.set('Erreur lors de l\'annulation de la réservation');
        return of(false);
      })
    );
  }

  /**
   * Valide le paiement d'une réservation
   */
  validerPaiement(reservationId: string, amountPaid: number): Observable<Reservation | null> {
    return this.reservationsApi.validatePayment(reservationId, { 
      amountPaid,
      paymentMethod: 'card' 
    }).pipe(
      map(response => {
        if (response.success && response.data) {
          const updatedReservation = ReservationMapper.fromDto(response.data);
          
          // Mettre à jour le cache local
          const reservationsActuelles = this.reservations();
          const index = reservationsActuelles.findIndex(r => r.id === reservationId);
          if (index !== -1) {
            reservationsActuelles[index] = {
              ...updatedReservation,
              paidAt: new Date()
            };
            this.reservations.set([...reservationsActuelles]);
          }
          
          return updatedReservation;
        }
        return null;
      }),
      catchError(error => {
        console.error('Erreur lors de la validation du paiement:', error);
        this.error.set('Erreur lors de la validation du paiement');
        return of(null);
      })
    );
  }

  // Méthodes utilitaires (conservées du service original)
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

  getStatuts(): StatutReservation[] {
    return Object.values(StatutReservation);
  }

  getStatutInfo(statut: StatutReservation): StatutInfo {
    switch (statut) {
      case StatutReservation.CREATED:
      case StatutReservation.EN_ATTENTE:
        return {
          label: 'En attente de paiement',
          icon: 'fas fa-clock',
          color: '#ff9800',
          bgColor: '#fff3e0'
        };
      case StatutReservation.VALIDATED:
      case StatutReservation.CONFIRMEE:
        return {
          label: 'Validée',
          icon: 'fas fa-check-circle',
          color: '#4caf50',
          bgColor: '#e8f5e8'
        };
      case StatutReservation.IN_PROGRESS:
      case StatutReservation.EN_COURS:
        return {
          label: 'En cours',
          icon: 'fas fa-play-circle',
          color: '#2196f3',
          bgColor: '#e3f2fd'
        };
      case StatutReservation.COMPLETED:
      case StatutReservation.TERMINEE:
        return {
          label: 'Terminée',
          icon: 'fas fa-check-double',
          color: '#8bc34a',
          bgColor: '#f1f8e9'
        };
      case StatutReservation.CANCEL:
      case StatutReservation.ANNULEE:
        return {
          label: 'Annulée',
          icon: 'fas fa-times-circle',
          color: '#f44336',
          bgColor: '#ffebee'
        };
      case StatutReservation.CANCEL_AUTO:
        return {
          label: 'Expirée',
          icon: 'fas fa-stopwatch',
          color: '#9e9e9e',
          bgColor: '#f5f5f5'
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

  /**
   * Vérifie si une réservation a expiré (20 minutes écoulées)
   */
  isReservationExpired(reservation: Reservation): boolean {
    if (!reservation.paymentExpiryAt || reservation.statut !== StatutReservation.CREATED) {
      return false;
    }
    
    return new Date() > reservation.paymentExpiryAt;
  }

  /**
   * Obtient le temps restant pour le paiement en minutes
   */
  getPaymentTimeRemaining(reservation: Reservation): number {
    if (!reservation.paymentExpiryAt || reservation.statut !== StatutReservation.CREATED) {
      return 0;
    }
    
    const now = new Date().getTime();
    const expiry = reservation.paymentExpiryAt.getTime();
    const diffMinutes = Math.max(0, Math.floor((expiry - now) / (1000 * 60)));
    
    return diffMinutes;
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
      .filter((r) => r.statut === StatutReservation.COMPLETED)
      .reduce((total, r) => total + (r.prix || 0), 0);
  }
}