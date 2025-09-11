import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../../auth/models/user.model';
import { ProfileType } from '../../../shared/enums/profile-type.enum';
import { environment } from '../../../../environments/environment';

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

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/users`;

  getUsers(profileTypeFilter?: ProfileType): Observable<User[]> {
    const params: Record<string, string> = {};
    if (profileTypeFilter) {
      params['profileType'] = profileTypeFilter;
    }
    return this.http.get<User[]>(this.apiUrl, { params });
  }

  getAdminAndEmployeeUsers(profileTypeFilter?: 'Administrator' | 'Employee'): Observable<User[]> {
    let url = `${this.apiUrl}/admin-employees`;
    if (profileTypeFilter) {
      url += `?profileType=${profileTypeFilter}`;
    }
    return this.http.get<User[]>(url);
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.apiUrl, user);
  }

  updateUser(id: string, user: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, user);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}