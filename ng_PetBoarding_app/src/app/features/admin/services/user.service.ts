import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import {
  ProfileType,
  ProfileTypeToBackendValue,
  mapBackendProfileType
} from '../../../shared/enums/profile-type.enum';
import { User } from '../../auth/models/user.model';
import { AuthService } from '../../auth/services/auth.service';

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  profileType: ProfileType;
  password: string;
  address?: {
    streetNumber: string;
    streetName: string;
    city: string;
    postalCode: string;
    country: string;
    complement?: string;
  };
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  profileType: ProfileType;
  address?: {
    streetNumber: string;
    streetName: string;
    city: string;
    postalCode: string;
    country: string;
    complement?: string;
  };
}

// Interfaces pour le backend (avec les valeurs numériques)
interface CreateUserBackendDto {
  firstname: string;
  lastname: string;
  email: string;
  phoneNumber: string;
  profileType: number;
  passwordHash: string;
}

interface UpdateUserBackendDto {
  firstname: string;
  lastname: string;
  email: string;
  phoneNumber: string;
  profileType: number;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);
  private readonly apiUrl = `${environment.apiUrl}/api/users`;

  getUsers(profileTypeFilter?: ProfileType): Observable<User[]> {
    const params: Record<string, string> = {};
    if (profileTypeFilter) {
      params['profileType'] = ProfileTypeToBackendValue[profileTypeFilter].toString();
    }
    return this.http.get<User[]>(this.apiUrl, { params }).pipe(
      map((users) =>
        users.map(
          (user) =>
            ({
              ...user,
              profileType: mapBackendProfileType(user.profileType)
            }) as User
        )
      )
    );
  }

  getAdminAndEmployeeUsers(profileTypeFilter?: 'Administrator' | 'Employee'): Observable<User[]> {
    let url = `${this.apiUrl}/admin-employees`;
    if (profileTypeFilter) {
      const backendValue = ProfileTypeToBackendValue[profileTypeFilter as ProfileType];
      url += `?profileType=${backendValue}`;
    }
    return this.http.get<any>(url).pipe(
      map((response) =>
        response.users.map(
          (user: any) =>
            ({
              ...user,
              profileType: mapBackendProfileType(user.profileType)
            }) as User
        )
      )
    );
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(
        (response) =>
          ({
            ...response.user,
            profileType: mapBackendProfileType(response.user.profileType)
          }) as User
      )
    );
  }

  createUser(user: CreateUserRequest): Observable<User> {
    const backendDto: CreateUserBackendDto = {
      firstname: user.firstName,
      lastname: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber,
      profileType: ProfileTypeToBackendValue[user.profileType],
      passwordHash: user.password
    };

    return this.http.post<User>(this.apiUrl, backendDto).pipe(
      map(
        (response) =>
          ({
            ...response,
            profileType: mapBackendProfileType(response.profileType)
          }) as User
      )
    );
  }

  updateUser(id: string, user: UpdateUserRequest): Observable<User> {
    const backendDto: UpdateUserBackendDto = {
      firstname: user.firstName,
      lastname: user.lastName,
      email: user.email,
      phoneNumber: user.phoneNumber,
      profileType: ProfileTypeToBackendValue[user.profileType]
    };

    return this.http.put<any>(`${this.apiUrl}/${id}/profile`, backendDto).pipe(
      map(
        (response) =>
          ({
            ...response.user,
            profileType: mapBackendProfileType(response.user.profileType)
          }) as User
      )
    );
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
