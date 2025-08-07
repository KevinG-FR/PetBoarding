import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { PrestationFiltersComponent } from './components/prestation-filters.component';
import { PrestationsListComponent } from './components/prestations-list.component';

@Component({
  selector: 'app-prestations',
  standalone: true,
  imports: [CommonModule, MatIconModule, PrestationFiltersComponent, PrestationsListComponent],
  templateUrl: './prestations.component.html',
  styleUrl: './prestations.component.scss'
})
export class PrestationsComponent {}
