import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginRequest } from '../models/login-request.model';
import { AuthService } from '../services/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  message: string = '';
  isSuccess: boolean = false;

  model: LoginRequest;
  constructor(
    private authService: AuthService,
    private cookieService: CookieService,
    private router: Router
  ) {
    this.model = {
      email: '',
      password: '',
    };
  }

  onFormSubmit(): void {
    this.authService.login(this.model).subscribe({
      next: (res) => {
        this.cookieService.set(
          'Authentication',
          `Bearer ${res.token}`,
          undefined,
          '/',
          undefined,
          true,
          'Strict'
        );
        this.authService.setUser({ email: this.model.email });
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        this.isSuccess = false;
        this.message = 'Thông tin tài khoản không hợp lệ!';
      },
    });
  }
}
