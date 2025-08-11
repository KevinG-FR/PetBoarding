import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { User } from '../../auth/models/user.model';

@Component({
  selector: 'app-profile-info',
  standalone: true,
  imports: [MatButtonModule, MatIconModule],
  templateUrl: './profile-info.component.html',
  styleUrl: './profile-info.component.scss'
})
export class ProfileInfoComponent {
  @Input({ required: true }) user!: User;
  @Output() editProfile = new EventEmitter<void>();

  onEditProfile(): void {
    this.editProfile.emit();
  }
}
