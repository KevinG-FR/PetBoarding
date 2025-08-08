import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-about',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule, RouterModule],
  templateUrl: './about.component.html',
  styleUrl: './about.component.scss'
})
export class AboutComponent {
  // Données de l'équipe
  teamMembers = [
    {
      name: 'Sarah Martinez',
      role: 'Fondatrice & Directrice',
      description: "Vétérinaire de formation avec 15 ans d'expérience dans les soins animaliers.",
      avatar: '👩‍⚕️',
      specialties: ['Soins vétérinaires', 'Comportement animal', "Gestion d'équipe"]
    },
    {
      name: 'Thomas Dubois',
      role: 'Responsable Pension',
      description: 'Spécialiste en éducation canine et gestion des établissements animaliers.',
      avatar: '👨‍💼',
      specialties: ['Éducation canine', 'Gestion pension', 'Sécurité animale']
    },
    {
      name: 'Emma Wilson',
      role: 'Toiletteuse & Soins',
      description:
        'Toiletteuse professionnelle certifiée avec une passion pour le bien-être animal.',
      avatar: '👩‍🔬',
      specialties: ['Toilettage professionnel', 'Soins esthétiques', 'Relaxation animale']
    }
  ];

  // Statistiques de l'entreprise
  stats = [
    { value: '2500+', label: 'Animaux accueillis', icon: 'pets' },
    { value: '8', label: "Années d'expérience", icon: 'star' },
    { value: '98%', label: 'Clients satisfaits', icon: 'thumb_up' },
    { value: '24/7', label: 'Surveillance', icon: 'visibility' }
  ];

  // Services principaux
  services = [
    {
      title: 'Pension Complète',
      description: 'Hébergement sécurisé avec promenades quotidiennes et soins personnalisés.',
      icon: 'home',
      features: [
        'Surveillance 24h/24',
        'Promenades quotidiennes',
        'Repas adaptés',
        'Soins médicaux'
      ]
    },
    {
      title: 'Garde à la Journée',
      description: 'Service de garde flexible pour vos animaux pendant vos absences courtes.',
      icon: 'schedule',
      features: ['Horaires flexibles', 'Activités ludiques', 'Socialisation', 'Rapports quotidiens']
    },
    {
      title: 'Toilettage',
      description:
        'Services de toilettage professionnel pour le bien-être et la beauté de vos compagnons.',
      icon: 'content_cut',
      features: ['Bain et shampoing', 'Coupe et brushing', 'Soins des ongles', 'Nettoyage oreilles']
    },
    {
      title: 'Soins Vétérinaires',
      description: 'Soins médicaux préventifs et curatifs assurés par notre équipe qualifiée.',
      icon: 'medical_services',
      features: ['Vaccinations', 'Vermifugation', "Soins d'urgence", 'Suivi médical']
    }
  ];
}
