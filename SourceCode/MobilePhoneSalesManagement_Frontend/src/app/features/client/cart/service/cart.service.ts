import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BASE_URL_API } from '../../../../app.config';
import { Observable } from 'rxjs';
import { AuthService } from '../../../auth/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  constructor(private http: HttpClient, private authService: AuthService) { }

  getCartItems(): Observable<any>  {
    const token = this.authService.getCookie('Authentication');
    const headers = new HttpHeaders({
      Authorization: `${token}`,
    });
    return this.http.get(`${BASE_URL_API}/carts`, {
      headers,
      withCredentials: true,
    });
  }
  updateCart(productId: number, quantity: number ): Observable<any>  {
    return this.http.put(`${BASE_URL_API}/carts/update?productId=${productId}&quantity=${quantity}`, null);
  }
  deleteCartItem(productId: number): Observable<any>  {
    return this.http.delete(`${BASE_URL_API}/carts/remove/${productId}`);
  }
}
