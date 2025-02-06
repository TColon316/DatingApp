import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';

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
}
