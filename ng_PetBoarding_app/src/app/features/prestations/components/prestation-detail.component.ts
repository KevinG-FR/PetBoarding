import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
  MatDialogRef
} from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { switchMap } from 'rxjs';
import { DurationPipe } from '../../../shared/pipes/duration.pipe';
import { RoleService } from '../../../shared/services/role.service';
import { AuthService } from '../../auth/services/auth.service';
import { BasketService } from '../../basket/services/basket.service';
import { Pet, PetType } from '../../pets/models/pet.model';
import { ReservationsService } from '../../reservations/services/reservations.service';
import { Prestation } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';
import { ReservationCompleteDialogComponent } from './reservation-complete-dialog.component';

interface ReservationResult {
  action?: string;
  pet?: Pet;
  dates?: {
    dateDebut: Date;
    dateFin: Date;
  };
}

@Component({
  selector: 'app-prestation-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDialogModule,
    MatDividerModule,
    DurationPipe
  ],
  templateUrl: './prestation-detail.component.html',
  styleUrl: './prestation-detail.component.scss'
})
export class PrestationDetailComponent {
  private prestationsService = inject(PrestationsService);
  private basketService = inject(BasketService);
  private reservationsService = inject(ReservationsService);
  private authService = inject(AuthService);
  private roleService = inject(RoleService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private dialogRef = inject(MatDialogRef<PrestationDetailComponent>);
  private data = inject(MAT_DIALOG_DATA);
  private dialog = inject(MatDialog);

  // Getters pour les rôles
  canMakeReservations = this.roleService.canMakeReservations;

  prestation: Prestation = this.data.prestation;

  categoryInfo = computed(() =>
    this.prestationsService.getCategoryInfo(this.prestation.categorieAnimal)
  );

  detailedInfo = computed(() => {
    const prestation = this.prestation;
    const baseInfo = {
      included: [] as string[],
      requirements: [] as string[],
      schedule: '',
      location: '',
      additionalInfo: [] as string[]
    };

    const libelle = prestation.libelle.toLowerCase();

    if (libelle.includes('pension')) {
      baseInfo.included = [
        'Hébergement en box individuel spacieux',
        'Repas selon les habitudes de votre animal',
        'Promenades quotidiennes',
        'Surveillance vétérinaire',
        'Soins et câlins'
      ];
      baseInfo.requirements = [
        'Carnet de vaccination à jour',
        'Traitement antiparasitaire récent',
        'Nourriture habituelle de votre animal'
      ];
      baseInfo.schedule = 'Check-in: 8h-18h, Check-out: 8h-19h';
      baseInfo.location = 'Centre de pension principal';
    } else if (libelle.includes('garderie')) {
      baseInfo.included = [
        'Accueil en journée',
        'Socialisation avec autres animaux',
        'Activités ludiques',
        'Repas de midi inclus',
        'Surveillance continue'
      ];
      baseInfo.requirements = [
        'Sociabilité avec autres animaux',
        'Vaccination à jour',
        'Bon état de santé'
      ];
      baseInfo.schedule = 'Lun-Ven: 7h30-18h30, Sam: 8h-17h';
      baseInfo.location = 'Espace garderie';
    } else if (libelle.includes('toilettage')) {
      baseInfo.included = [
        'Bain avec shampooing adapté',
        'Séchage et brossage',
        'Coupe des griffes',
        'Nettoyage des oreilles',
        'Parfum pour animaux'
      ];
      baseInfo.requirements = [
        'Animal calme et manipulable',
        'Pas de problème de peau',
        'Dernière vaccination récente'
      ];
      baseInfo.schedule = 'Sur rendez-vous uniquement';
      baseInfo.location = 'Salon de toilettage';
    } else if (libelle.includes('promenade')) {
      baseInfo.included = [
        "Promenade adaptée à l'animal",
        'Exercice et stimulation',
        'Retour avec compte-rendu',
        'Photos pendant la sortie',
        'Eau fraîche fournie'
      ];
      baseInfo.requirements = [
        'Animal bien éduqué en laisse',
        'Sociabilité avec autres chiens',
        'Pas de troubles comportementaux'
      ];
      baseInfo.schedule = 'Tous les jours: 8h-12h et 14h-18h';
      baseInfo.location = 'Parcs et espaces verts environnants';
    } else if (libelle.includes('domicile')) {
      baseInfo.included = [
        'Visite à votre domicile',
        'Repas et soins selon vos consignes',
        'Compagnie et jeux',
        'Arrosage plantes (bonus)',
        'Compte-rendu détaillé'
      ];
      baseInfo.requirements = [
        "Clés ou code d'accès",
        'Consignes détaillées',
        'Coordonnées vétérinaire',
        'Nourriture et accessoires disponibles'
      ];
      baseInfo.schedule = 'Flexible selon vos besoins';
      baseInfo.location = 'À votre domicile';
    } else if (libelle.includes('consultation')) {
      baseInfo.included = [
        'Évaluation comportementale',
        'Conseils personnalisés',
        "Plan d'action détaillé",
        'Suivi téléphonique',
        'Documentation remise'
      ];
      baseInfo.requirements = [
        'Questionnaire préalable rempli',
        'Historique médical disponible',
        'Présence du propriétaire'
      ];
      baseInfo.schedule = 'Lun-Ven: 9h-17h sur RDV';
      baseInfo.location = 'Cabinet de consultation';
    }

    baseInfo.additionalInfo = [
      'Assurance responsabilité civile professionnelle',
      'Personnel formé et expérimenté',
      'Urgences vétérinaires 24h/24',
      "Possibilité d'annulation 24h avant"
    ];

    return baseInfo;
  });

  getCategoryChipClass(): string {
    switch (this.prestation.categorieAnimal) {
      case PetType.DOG:
        return 'chip-chien';
      case PetType.CAT:
        return 'chip-chat';
      default:
        return '';
    }
  }

  getPrestationIcon(): string {
    const libelle = this.prestation.libelle.toLowerCase();
    if (libelle.includes('pension')) return 'hotel';
    if (libelle.includes('garderie')) return 'groups';
    if (libelle.includes('toilettage')) return 'content_cut';
    if (libelle.includes('promenade')) return 'directions_walk';
    if (libelle.includes('domicile')) return 'home';
    if (libelle.includes('consultation')) return 'psychology';
    return 'room_service';
  }

  onClose(): void {
    this.dialogRef.close();
  }

  onReserver(): void {
    const prestation = this.prestation;
    const dialogRef = this.dialog.open(ReservationCompleteDialogComponent, {
      data: { prestation },
      width: '1000px',
      maxWidth: '95vw',
      height: 'auto',
      maxHeight: '90vh'
    });

    dialogRef.afterClosed().subscribe((result: ReservationResult) => {
      if (result?.action === 'reserve' && result.pet && result.dates) {
        const currentUser = this.authService.currentUser();

        if (!currentUser) {
          this.snackBar.open('Vous devez être connecté pour faire une réservation', 'Fermer', {
            duration: 5000
          });
          return;
        }

        const reservationRequest = {
          userId: currentUser.id,
          prestationId: prestation.id,
          animalId: result.pet.id,
          animalName: result.pet.name,
          dateDebut: result.dates.dateDebut,
          dateFin: result.dates.dateFin,
          commentaires: ''
        };

        this.reservationsService
          .creerReservationAvecPlanning(reservationRequest)
          .pipe(
            switchMap((reservation) => {
              if (!reservation) {
                throw new Error('Impossible de créer la réservation');
              }

              return this.basketService.addItemToBasket(reservation.id);
            })
          )
          .subscribe({
            next: () => {
              this.dialogRef.close();

              const snackBarRef = this.snackBar.open(
                `Réservation créée et ajoutée au panier !`,
                'Voir le panier',
                {
                  duration: 5000
                }
              );

              snackBarRef.onAction().subscribe(() => {
                this.router.navigate(['/basket']);
              });
            },
            error: (error) => {
              this.snackBar.open(
                error.message || 'Erreur lors de la création de la réservation',
                'Fermer',
                { duration: 5000 }
              );
            }
          });
      }
    });
  }
}
