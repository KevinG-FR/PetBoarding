import { Injectable, inject } from '@angular/core';
import { AuthService } from '../../auth/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private readonly authService = inject(AuthService);

  constructor() {}
}
