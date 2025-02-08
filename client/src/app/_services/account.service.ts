import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  // Attempt to log the user into the application
  login(model: any) {
    return this.http.post<User>(`${this.baseUrl}account/login`, model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  //
  register(model: any) {
    return this.http.post<User>(`${this.baseUrl}account/register`, model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }

        return user;
      })
    );
  }

  setCurrentUser(user: User) {
    // Create a new Item in local storage called "user" and store the data that was returned
    localStorage.setItem('user', JSON.stringify(user));

    // Set the CurrentUser variable to the returned User
    this.currentUser.set(user);
  }

  // Resets CurrentUser info
  logout() {
    // Remove the current users data from the local storage
    localStorage.removeItem('user');

    // Reset the CurrentUser variable back to NULL
    this.currentUser.set(null);
  }
}
