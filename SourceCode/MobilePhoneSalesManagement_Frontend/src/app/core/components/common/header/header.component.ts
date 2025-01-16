import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../../../../features/auth/services/auth.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule,FormsModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  isAuthenticated = false;
  searchKeyword: string = '';
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
  

  onSearch(): void {
    if (this.searchKeyword) {
      // Chuyển hướng đến trang danh sách sản phẩm và gửi từ khóa qua query params
      this.router.navigate(['/products'], { queryParams: { search: this.searchKeyword } });
    }
  }
}
