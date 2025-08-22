import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, of, tap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginRequestDto } from '../../../shared/contracts/auth/login-request.dto';
import { LoginResponseDto, UserDto } from '../../../shared/contracts/auth/login-response.dto';
import { RegisterRequestDto } from '../../../shared/contracts/auth/register-request.dto';
import { RegisterResponseDto } from '../../../shared/contracts/auth/register-response.dto';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

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

  constructor() {
    this.checkStoredAuth();
  }

  register(registerData: RegisterRequestDto): Observable<RegisterResponseDto> {
    this._isLoading.set(true);

    return this.http.post<RegisterResponseDto>(`${this.apiUrl}/register`, registerData).pipe(
      tap((response: RegisterResponseDto) => {
        if (response.success && response.token) {
          localStorage.setItem('auth_token', response.token);
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

  login(
    email: string,
    password: string,
    rememberMe: boolean = false
  ): Observable<LoginResponseDto> {
    this._isLoading.set(true);

    const loginData: LoginRequestDto = {
      email,
      password,
      rememberMe
    };

    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, loginData).pipe(
      tap((response: LoginResponseDto) => {
        if (response.success && response.token && response.user) {
          localStorage.setItem('auth_token', response.token);

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
    localStorage.removeItem('auth_token');
    this._isAuthenticated.set(false);
    this._currentUser.set(null);

    this.router.navigate(['/home']);
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  private checkStoredAuth(): void {
    const token = localStorage.getItem('auth_token');
    if (token) {
      this._isAuthenticated.set(true);
    }
  }

  toggleAuthForTesting(): void {
    const isAuth = this._isAuthenticated();
    if (isAuth) {
      this.logout();
    } else {
      this._isAuthenticated.set(true);
      localStorage.setItem('auth_token', 'mock-jwt-token');
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
