import { ActivatedRoute } from '@angular/router';
import { Component, inject, OnInit } from '@angular/core';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [GalleryModule, TabsModule],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
})
export class MemberDetailComponent implements OnInit {
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);
  member?: Member;
  images: GalleryItem[] = [];

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    // Get the passed UserName from the Url route param
    const username = this.route.snapshot.paramMap.get('username');

    // If no UserName is passed then there's no need to continue trying to load the member
    if (!username) return;

    // Retrieve the Member's details
    this.memberService.getMember(username).subscribe({
      next: (member) => {
        this.member = member;
        member.photos.map((p) => {
          this.images.push(new ImageItem({ src: p.url, thumb: p.url }));
        });
      },
    });
  }
}
