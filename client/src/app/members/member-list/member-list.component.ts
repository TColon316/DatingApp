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
  private memberService = inject(MembersService);

  members: Member[] = [];

  ngOnInit(): void {
    this.loadMembers();
  }

  // Get all members
  loadMembers() {
    this.memberService.getMembers().subscribe({
      next: (members) => (this.members = members),
    });
  }
}
