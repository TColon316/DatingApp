import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  members = signal<Member[]>([]);

  getMembers() {
    return this.http.get<Member[]>(`${this.baseUrl}users`).subscribe({
      next: (members) => this.members.set(members),
    });
  }

  getMember(username: string) {
    // Check the Members array to see if Member has already been pulled from the Database
    const member = this.members().find((x) => x.username === username);

    // Check if a Member was found
    if (member !== undefined) return of(member);

    // If no Member was found, then make an Api request to query the database for the Member
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }

  updateMember(member: Member) {
    // We want to update the Members array in the service to reflect any updates committed to reflect in the app
    return this.http.put(`${this.baseUrl}users`, member).pipe(
      // Iterate through the Members array to find the matching Member (by their Username) and swap out the old Member with the Updated Member
      tap(() => {
        this.members.update((members) =>
          members.map((m) => (m.username === member.username ? member : m))
        );
      })
    );
  }

  // Sets the main photo while also updating the Members signal object here to include this updated change so that anywhere this
  //  Members signal is referenced will also show the updated change
  setMainPhoto(photo: Photo) {
    return this.http
      .put(`${this.baseUrl}users/set-main-photo/${photo.id}`, {})
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((m) => {
              if (m.photos.includes(photo)) {
                m.photoUrl = photo.url; // Updates the current Main photo
              }

              return m;
            })
          );
        })
      );
  }

  deletePhoto(photo: Photo) {
    return this.http
      .delete(`${this.baseUrl}users/delete-photo/${photo.id}`)
      .pipe(
        tap(() => {
          this.members.update((members) =>
            members.map((m) => {
              if (m.photos.includes(photo)) {
                m.photos = m.photos.filter((x) => x.id !== photo.id);
              }

              return m;
            })
          );
        })
      );
  }
}
