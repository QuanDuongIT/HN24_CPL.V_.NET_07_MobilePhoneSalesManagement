import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../../../../features/auth/services/auth.service';
import { CartService } from '../../../../features/client/cart/service/cart.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  isAuthenticated = false;
  count: number = 0;
  cartItems: any[] = []

  constructor( private authService: AuthService , private cookieService: CookieService, private router: Router, private cartService: CartService) {}

  ngOnInit(): void {
    this.authService.isAuthenticated.subscribe((status) => {
      this.isAuthenticated = status;
    });
    this.fetchCartItems();
  }
  fetchCartItems (): void {
    this.cartService.getCartItems().subscribe(
      (res) => {
        this.cartItems = res;
        this.count = this.cartItems.length;
      },
      (err) => {
        console.log(err);
      }
    )
  }
  
  
  logout(): void {
    this.authService.logout();
    this.router.navigateByUrl('/login');
  }
}
