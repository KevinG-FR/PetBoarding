import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { DurationPipe } from '../../../shared/pipes/duration.pipe';
import { CategorieAnimal } from '../models/prestation.model';
import { PrestationsService } from '../services/prestations.service';

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
  private dialogRef = inject(MatDialogRef<PrestationDetailComponent>);
  private data = inject(MAT_DIALOG_DATA);

  // Récupérer la prestation depuis les données du modal
  prestation = this.data.prestation;

  // Computed pour optimiser les appels répétés
  categoryInfo = computed(() =>
    this.prestationsService.getCategoryInfo(this.prestation.categorieAnimal)
  );

  // Informations détaillées de la prestation
  detailedInfo = computed(() => {
    const prestation = this.prestation;
    const baseInfo = {
      included: [] as string[],
      requirements: [] as string[],
      schedule: '',
      location: '',
      additionalInfo: [] as string[]
    };

    // Informations spécifiques selon le type de prestation
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
      case CategorieAnimal.CHIEN:
        return 'chip-chien';
      case CategorieAnimal.CHAT:
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
    // Fermer le modal et émettre un événement de réservation
    this.dialogRef.close('reserve');
  }
}
