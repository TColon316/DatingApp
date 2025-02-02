import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  standalone: true,
  imports: [],
  templateUrl: './server-error.component.html',
  styleUrl: './server-error.component.css',
})
export class ServerErrorComponent {
  error: any;

  //We MUST create a constructor in this specific case because the Constructor is the ONLY place in which we can access
  // the navigation properties
  constructor(private router: Router) {
    // Retrieve the NavigationExtras
    const navigation = this.router.getCurrentNavigation();

    // error in the NavigationExtras is the property we're trying to access. The '?' are to subdue the "possible undefined" error
    this.error = navigation?.extras?.state?.['error'];
  }
}
