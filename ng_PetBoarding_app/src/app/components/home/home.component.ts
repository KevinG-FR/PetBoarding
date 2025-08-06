import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatIconModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  services = signal([
    {
      icon: 'hotel',
      title: 'Hébergement',
      description: 'Pension complète dans un environnement chaleureux et sécurisé.'
    },
    {
      icon: 'pets',
      title: 'Soins personnalisés',
      description: 'Attention individuelle adaptée aux besoins de votre animal.'
    },
    {
      icon: 'medical_services',
      title: 'Suivi vétérinaire',
      description: 'Surveillance médicale par des professionnels qualifiés.'
    },
    {
      icon: 'videocam',
      title: 'Nouvelles quotidiennes',
      description: 'Photos et vidéos de votre compagnon chaque jour.'
    }
  ]);
}
