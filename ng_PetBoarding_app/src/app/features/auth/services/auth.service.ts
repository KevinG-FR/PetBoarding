import { HttpClient } from '@angular/common/http';
import { Injectable, Injector, inject, signal } from '@angular/core';
import { Observable, catchError, of, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginRequestDto } from '../../../shared/contracts/auth/login-request.dto';
import { LoginResponseDto, UserDto } from '../../../shared/contracts/auth/login-response.dto';
import { RegisterRequestDto } from '../../../shared/contracts/auth/register-request.dto';
import { RegisterResponseDto } from '../../../shared/contracts/auth/register-response.dto';
import { TokenService } from '../../../shared/services/token.service';
import { GetProfileResponseDto } from '../../profile/contracts/update-profile.dto';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly injector = inject(Injector);
  private readonly tokenService = inject(TokenService);

  // Signals pour l'état d'authentification
  private readonly _isAuthenticated = signal(false);
  private readonly _isLoading = signal(false);
  private readonly _currentUser = signal<User | null>(null);

  // API base URL (configuré selon votre backend)
  private readonly apiUrl = `${environment.apiUrl}/api/users`;

  // Getters publics
  isAuthenticated = this._isAuthenticated.asReadonly();
  isLoading = this._isLoading.asReadonly();
  currentUser = this._currentUser.asReadonly();

  constructor() {}

  initializeAuth(): Observable<void> {
    return new Observable((observer) => {
      if (this.tokenService.getToken()) {
        this.getUserProfile().subscribe({
          next: (getProfileResponseDto: GetProfileResponseDto) => {
            const user = this.mapUserDtoToUser(getProfileResponseDto.user);
            this._currentUser.set(user);
            this._isAuthenticated.set(true); // 🔥 Important: définir isAuthenticated à true
            observer.next();
            observer.complete();
          },
          error: () => {
            // Ne pas échouer l'initialisation de l'app pour un problème d'auth
            observer.next();
            observer.complete();
          }
        });
      } else {
        observer.next();
        observer.complete();
      }
    });
  }

  register(registerData: RegisterRequestDto): Observable<RegisterResponseDto> {
    this._isLoading.set(true);

    return this.http.post<RegisterResponseDto>(`${this.apiUrl}/register`, registerData).pipe(
      tap((response: RegisterResponseDto) => {
        if (response.success && response.token) {
          this.tokenService.setToken(response.token);
          this._isAuthenticated.set(true);
        }
      }),
      catchError((_error: unknown) => {
        return of({
          success: false,
          message: "Une erreur est survenue lors de l'inscription"
        });
      }),
      tap(() => this._isLoading.set(false))
    );
  }

  login(email: string, password: string): Observable<LoginResponseDto> {
    this._isLoading.set(true);

    const loginData: LoginRequestDto = {
      email,
      password
    };

    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, loginData).pipe(
      tap((response: LoginResponseDto) => {
        if (response.success && response.token && response.user) {
          this.tokenService.setToken(response.token);

          const user: User = this.mapUserDtoToUser(response.user);
          this._currentUser.set(user);
          this._isAuthenticated.set(true);
        }
      }),
      catchError((_error: unknown) => {
        return of({
          success: false,
          message: 'Une erreur est survenue lors de la connexion'
        });
      }),
      tap(() => this._isLoading.set(false))
    );
  }

  logout(): void {
    this.clearAuthData();
    // Navigation différée pour éviter la dépendance circulaire
    setTimeout(async () => {
      const { Router } = await import('@angular/router');
      const router = this.injector.get(Router);
      router.navigate(['/home']);
    }, 0);
  }

  getToken(): string | null {
    return this.tokenService.getToken();
  }

  getUserProfile(): Observable<GetProfileResponseDto> {
    return this.http.get<GetProfileResponseDto>(`${this.apiUrl}/profile`).pipe(
      catchError((error: unknown) => {
        // Vérifier si c'est une erreur d'authentification
        if (error && typeof error === 'object' && 'status' in error) {
          const httpError = error as { status: number };
          if (httpError.status === 401 || httpError.status === 403) {
            this.clearAuthData();
          }
        }

        throw error;
      })
    );
  }

  private clearAuthData(): void {
    this.tokenService.clearAll();
    this._isAuthenticated.set(false);
    this._currentUser.set(null);
  }

  toggleAuthForTesting(): void {
    const isAuth = this._isAuthenticated();
    if (isAuth) {
      this.logout();
    } else {
      // Pour les tests, on peut soit :
      // 1. Utiliser un vrai token si disponible
      // 2. Ou créer un utilisateur de test temporaire
      const testToken = 'test-jwt-token';
      this.tokenService.setToken(testToken);
      this.tokenService.setRememberMe(true);
      this._isAuthenticated.set(true);

      // Essayer de récupérer le profil réel, sinon utiliser un profil de test
      this._isLoading.set(true);
      this.getUserProfile().subscribe({
        next: (getProfileResponseDto: GetProfileResponseDto) => {
          const user = this.mapUserDtoToUser(getProfileResponseDto.user);
          this._currentUser.set(user);
          this._isLoading.set(false);
        },
        error: () => {
          // Si l'API n'est pas disponible, créer un utilisateur de test
          const testUser: User = {
            id: 'test-user-id',
            email: 'test@example.com',
            firstName: 'John',
            lastName: 'Doe',
            phoneNumber: '+33123456789',
            profileType: 'CLIENT',
            status: 'ACTIVE',
            createdAt: new Date(),
            updatedAt: new Date()
          };
          this._currentUser.set(testUser);
          this._isAuthenticated.set(true);
          this._isLoading.set(false);
        }
      });
    }
  }

  private mapUserDtoToUser(userDto: UserDto): User {
    return {
      id: userDto.id,
      email: userDto.email,
      firstName: userDto.firstName,
      lastName: userDto.lastName,
      phoneNumber: userDto.phoneNumber,
      profileType: userDto.profileType,
      status: userDto.status,
      address: userDto.address
        ? {
            streetNumber: userDto.address.streetNumber,
            streetName: userDto.address.streetName,
            city: userDto.address.city,
            postalCode: userDto.address.postalCode,
            country: userDto.address.country,
            complement: userDto.address.complement
          }
        : undefined,
      createdAt: new Date(userDto.createdAt),
      updatedAt: new Date(userDto.updatedAt)
    };
  }
}
