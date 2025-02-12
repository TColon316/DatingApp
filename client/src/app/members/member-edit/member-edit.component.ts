import {
  Component,
  HostListener,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { AccountService } from '../../_services/account.service';
import { DatePipe } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { PhotoEditorComponent } from '../photo-editor/photo-editor.component';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [
    DatePipe,
    FormsModule,
    PhotoEditorComponent,
    TabsModule,
    TimeagoModule,
  ],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  member?: Member;
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  private toastrService = inject(ToastrService);

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    const user = this.accountService.currentUser();

    if (!user) return;

    this.memberService.getMember(user.username).subscribe({
      next: (member) => (this.member = member),
    });
  }

  updateMember() {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: (_) => {
        // Display a message indicating that the Member was updated successfully
        this.toastrService.success('Profile updated successfully!');

        // This resets the form (hides the "Changes made" message and disables the Save button)
        this.editForm?.reset(this.member);
      },
    });
  }

  onMemberChange(event: Member) {
    this.member = event;
  }
}
