import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { PrestationsService } from '../services/prestations.service';
import { PrestationItemComponent } from './prestation-item.component';

@Component({
  selector: 'app-prestations-list',
  standalone: true,
  imports: [CommonModule, MatIconModule, PrestationItemComponent],
  templateUrl: './prestations-list.component.html',
  styleUrl: './prestations-list.component.scss'
})
export class PrestationsListComponent {
  prestationsService = inject(PrestationsService);
}
