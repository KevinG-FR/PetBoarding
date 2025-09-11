import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { UserService } from '../../services/user.service';
import { User } from '../../../auth/models/user.model';
import { ProfileType } from '../../../../shared/enums/profile-type.enum';
import { UserItemComponent } from '../user-item/user-item.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatFormFieldModule,
    FormsModule,
    UserItemComponent
  ],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  users = signal<User[]>([]);
  loading = signal(false);
  selectedFilter = signal<'Administrator' | 'Employee' | ''>('');
  
  readonly ProfileType = ProfileType;

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    const filter = this.selectedFilter() || undefined;
    
    this.userService.getAdminAndEmployeeUsers(filter).subscribe({
      next: (users) => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.loading.set(false);
      }
    });
  }

  onFilterChange(): void {
    this.loadUsers();
  }

  onAddUser(): void {
    this.router.navigate(['/admin/users/add']);
  }

  onUserDeleted(): void {
    this.loadUsers();
  }
}