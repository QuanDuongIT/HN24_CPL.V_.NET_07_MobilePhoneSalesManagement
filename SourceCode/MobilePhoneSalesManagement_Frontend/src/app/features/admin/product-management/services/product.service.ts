import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { BASE_URL_API } from '../../../../app.config';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient) { }

  getProducts(): Observable<any> {
    return this.http.get<any>(`${BASE_URL_API}/Products/get-all-products`);
  }

  getProductById(productId: string): Observable<any> {
    return this.http.get(`${BASE_URL_API}/Products/get-product-by-id/${productId}`);
  }

  addProduct(product: any): Observable<any> {
    return this.http.post(`${BASE_URL_API}/Products/add-new-product`, product);
  }

  updateProduct(productId: string, product: any): Observable<any> {
    return this.http.put(`${BASE_URL_API}/Products/update-product/${productId}`, product);
  }

  deleteProduct(productId: string): Observable<any> {
    return this.http.delete(`${BASE_URL_API}/Products/delete-product-by-id/${productId}`);
  }

  getSpecificationTypes(): Observable<import("../models/specificationType").specificationType[]> {
    return this.http.get<any>(`${BASE_URL_API}/SpecificationTypes/get-all-specificationTypes`);
  }
}
