import { Component, inject, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MemberCardComponent } from '../member-card/member-card.component';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-list',
  standalone: true,
  imports: [MemberCardComponent],
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  memberService = inject(MembersService);

  ngOnInit(): void {
    // Check if the Members array in the MemberService already contains Members. If not, then call the LoadMembers() method
    if (this.memberService.members().length === 0) this.loadMembers();
  }

  // Get all members
  loadMembers() {
    this.memberService.getMembers();
  }
}
