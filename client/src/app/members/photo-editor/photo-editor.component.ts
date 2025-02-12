import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { MembersService } from '../../_services/members.service';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-photo-editor',
  standalone: true,
  imports: [FileUploadModule, NgClass, NgFor, NgIf, NgStyle, DecimalPipe],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css',
})
export class PhotoEditorComponent implements OnInit {
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  member = input.required<Member>();
  uploader?: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  memberChange = output<Member>();

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo).subscribe({
      next: (_) => {
        // Get a copy of the Input Member signal
        const updatedMember = { ...this.member() };

        // Filter out the deleted Photo
        updatedMember.photos = updatedMember.photos.filter(
          (x) => x.id !== photo.id
        );

        // Emit the updated Member to the Parent component (Update the photos visible)
        this.memberChange.emit(updatedMember);
      },
    });
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: (_) => {
        // Get the CurrentUser
        const user = this.accountService.currentUser();

        // If User IS NOT NULL, then update the CurrentUsers Main photo and then pass the updated User object back to update the Login pic
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }

        // Get a copy of the input Member property
        const updatedMember = { ...this.member() };

        // Update the input Member's new Main photo
        updatedMember.photoUrl = photo.url;

        // Reset the IsMain flag for the old Main photo and update it for the new Main photo
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });

        // Emit the Updated Member so the updated values will reflect across the app
        this.memberChange.emit(updatedMember);
      },
    });
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: `${this.baseUrl}users/add-photo`,
      authToken: `Bearer ${this.accountService.currentUser()?.token}`,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      // Need this becaues we're sending up our authentication in a header rather then using cookies
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      const photo = JSON.parse(response);

      // Get the member that the photo is being uploaded for
      const updatedMember = { ...this.member() };

      // Add the photo to the Photo table related to the previously pulled Member
      updatedMember.photos.push(photo);

      // Push this change up to the Parent Member signal object so the change will trickle back down to here, the Child component
      this.memberChange.emit(updatedMember);

      // Set the Users first photo to main if it is their first time registering
      if (photo.isMain) {
        const user = this.accountService.currentUser();

        if (user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }

        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });

        this.memberChange.emit(updatedMember);
      }
    };
  }
}
