import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Member } from '../_models/member';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  private accountService = inject(AccountService);
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  resetUserParams() {
    this.userParams.set(new UserParams(this.user));
  }

  getMembers() {
    // This will check to see if the user exists already in the MemberCache map
    const response = this.memberCache.get(
      Object.values(this.userParams()).join('-')
    );

    // If it does, return the cached User. No need to run the rest of the method
    if (response) return this.setPaginatedResponse(response);

    let params = this.setPaginationHeaders(
      this.userParams().pageNumber,
      this.userParams().pageSize
    );

    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http
      .get<Member[]>(`${this.baseUrl}users`, { observe: 'response', params })
      .subscribe({
        next: (response) => {
          this.setPaginatedResponse(response);
          this.memberCache.set(
            Object.values(this.userParams()).join('-'),
            response
          );
        },
      });
  }

  private setPaginatedResponse(response: HttpResponse<Member[]>) {
    this.paginatedResult.set({
      items: response.body as Member[],
      pagination: JSON.parse(response.headers.get('Pagination')!),
    });
  }

  private setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return params;
  }

  getMember(username: string) {
    const member: Member = [...this.memberCache.values()]
      .reduce((array, element) => array.concat(element.body), [])
      .find((mem: Member) => mem.username === username);

    if (member) return of(member);

    // If no Member was found, then make an Api request to query the database for the Member
    return this.http.get<Member>(`${this.baseUrl}users/${username}`);
  }

  updateMember(member: Member) {
    // We want to update the Members array in the service to reflect any updates committed to reflect in the app
    return this.http
      .put(`${this.baseUrl}users`, member)
      .pipe
      // Iterate through the Members array to find the matching Member (by their Username) and swap out the old Member with the Updated Member
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => (m.username === member.username ? member : m))
      //   );
      // })
      ();
  }

  // Sets the main photo while also updating the Members signal object here to include this updated change so that anywhere this
  //  Members signal is referenced will also show the updated change
  setMainPhoto(photo: Photo) {
    return this.http
      .put(`${this.baseUrl}users/set-main-photo/${photo.id}`, {})
      .pipe
      // tap(() => {
      //   this.members.update((members) =>
      //     members.map((m) => {
      //       if (m.photos.includes(photo)) {
      //         m.photoUrl = photo.url; // Updates the current Main photo
      //       }

      //       return m;
      //     })
      //   );
      // })
      ();
  }

  deletePhoto(photo: Photo) {
    return this.http
      .delete(`${this.baseUrl}users/delete-photo/${photo.id}`)
      .pipe
      // tap(() => {
      //   this.members.update((members) => {
      //     members.map((m) => {
      //       if (m.photos.includes(photo)) {
      //         m.photos = m.photos.filter((x) => x.id !== photo.id);
      //       }

      //       return m;
      //     });
      //   });
      // })
      ();
  }
}
