import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, throwError } from 'rxjs';
import { User } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { LoginRequest } from '../models/login-request.model';
import { LoginResponse } from '../models/login-response.model';
import { BASE_URL_API } from '../../../app.config';
import { RegisterModel } from '../models/register-model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  $user = new BehaviorSubject<User | undefined>(undefined);
  constructor(private http: HttpClient,
    private cookieService: CookieService
  ) { }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${BASE_URL_API}/authentication/login-user`, request);
  }

  setUser(user: User): void {
    this.$user.next(user);
    localStorage.setItem('user-email', user.email);
  }

  user(): Observable<User | undefined> {
    return this.$user.asObservable();
  }

  getUser(): User | undefined {
    const email = localStorage.getItem("user-email");

    if (email) {
      return {
        email: email
      };
    }

    return undefined;
  }

  logout(): void {
    //localStorage.removeItem("user-email");
    localStorage.clear();
    this.cookieService.delete("Authentication", "/");
    this.$user.next(undefined);
  }

  register(model: any): Observable<any> {
    return this.http.post(`${BASE_URL_API}/authentication/register-user`, model)
      .pipe(
        catchError((error) => {
          // Kiểm tra lỗi trả về từ server và in chi tiết lỗi ra console
          console.error('API Error:', error);
          
          if (error.error && error.error.errors) {
            // Hiển thị các lỗi từ server (ví dụ, lỗi validation)
            console.log('Validation Errors:', error.error.errors);
          }
  
          // Nếu bạn muốn trả về lỗi từ API cho component xử lý
          return throwError(error); // Trả về lỗi cho phần còn lại của code (component, service)
        })
      );
  }

  verifyEmail(email: string, code: string ): Observable<any> {
    console.log('heheheh');
    
    return this.http.get(`${BASE_URL_API}/authentication/verify-email?email=${email}&code=${code}`);
  }
  
}