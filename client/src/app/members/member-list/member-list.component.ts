import { AccountService } from '../../_services/account.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from '../member-card/member-card.component';
import { MembersService } from '../../_services/members.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { UserParams } from '../../_models/userParams';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [ButtonsModule, FormsModule, MemberCardComponent, PaginationModule],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  memberService = inject(MembersService);
  genderList = [
    //This will be used for a Select field
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];

  ngOnInit(): void {
    // Check if the Members array in the MemberService already contains Members. If not, then call the LoadMembers() method
    if (!this.memberService.paginatedResult()) this.loadMembers();
  }

  // Get all members
  loadMembers() {
    this.memberService.getMembers();
  }

  // Reset the Form parameters
  resetFilters() {
    this.memberService.resetUserParams();
    this.loadMembers();
  }

  // Logic to handle when the user changes the paginated page
  pageChanged(event: any) {
    if (this.memberService.userParams().pageNumber !== event.page) {
      // Set the original pageNumber to the new one the user has selected
      this.memberService.userParams().pageNumber = event.page;

      // Get the next list of paginated results for the user
      this.loadMembers();
    }
  }
}
