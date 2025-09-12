import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  EventEmitter,
  inject,
  Input,
  Output
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { Prestation } from '../../../../shared/models/prestation.model';
import { DurationPipe } from '../../../../shared/pipes/duration.pipe';
import { PetType, PetTypeLabels } from '../../../pets/models/pet.model';
import { PrestationsService } from '../../../prestations/services/prestations.service';

@Component({
  selector: 'app-admin-prestation-item',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatChipsModule, DurationPipe],
  templateUrl: './admin-prestation-item.component.html',
  styleUrl: './admin-prestation-item.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminPrestationItemComponent {
  @Input() prestation!: Prestation;
  @Output() view = new EventEmitter<string>();
  @Output() edit = new EventEmitter<string>();
  @Output() delete = new EventEmitter<string>();

  private prestationsService = inject(PrestationsService);

  categoryInfo = computed(() => {
    const mappedType = this.prestationsService.mapBackendPetType(this.prestation.categorieAnimal);
    return this.prestationsService.getCategoryInfo(mappedType);
  });

  getPetTypeLabel(petType: PetType): string {
    return PetTypeLabels[petType] || petType;
  }


  getCategoryChipClass(): string {
    const mappedType = this.prestationsService.mapBackendPetType(this.prestation.categorieAnimal);
    switch (mappedType) {
      case PetType.DOG:
        return 'chip-chien';
      case PetType.CAT:
        return 'chip-chat';
      case PetType.BIRD:
        return 'chip-oiseau';
      case PetType.RABBIT:
        return 'chip-lapin';
      case PetType.HAMSTER:
        return 'chip-hamster';
      default:
        return 'chip-default';
    }
  }

  onView(): void {
    this.view.emit(this.prestation.id);
  }

  onEdit(): void {
    this.edit.emit(this.prestation.id);
  }

  onDelete(): void {
    this.delete.emit(this.prestation.id);
  }
}
