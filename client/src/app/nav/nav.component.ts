import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [
    BsDropdownModule,
    FormsModule,
    RouterLink,
    RouterLinkActive,
    TitleCasePipe,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  accountService = inject(AccountService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  model: any = {};

  login() {
    this.accountService.login(this.model).subscribe({
      next: (_) => this.router.navigateByUrl('/members'),
      error: (error) => this.toastr.error(error.error),
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
