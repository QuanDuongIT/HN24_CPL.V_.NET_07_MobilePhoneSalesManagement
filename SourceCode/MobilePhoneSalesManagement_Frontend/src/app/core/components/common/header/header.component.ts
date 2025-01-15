import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../../../../features/auth/services/auth.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  isAuthenticated = false;

  constructor( private authService: AuthService , private cookieService: CookieService, private router: Router) {}

  ngOnInit(): void {
    this.authService.isAuthenticated.subscribe((status) => {
      this.isAuthenticated = status;
    });
  }
  
  
  logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
