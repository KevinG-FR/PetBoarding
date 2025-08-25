import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../../features/auth/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AppInitService {
  private readonly authService = inject(AuthService);

  initializeApp(): Observable<void> | Promise<void> | void {
    // eslint-disable-next-line no-console
    console.log('🚀 AppInitService: Initializing application...');
    return this.authService.initializeAuth();
  }
}
