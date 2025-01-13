import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { BASE_URL_API } from '../../../../app.config';
import { RequestBrand } from '../models/add-brand-request.model';

@Injectable({
  providedIn: 'root'
})
export class BrandService {
  constructor(private http: HttpClient) { }

  getBrands(): Observable<any> {
    return this.http.get<any>(`${BASE_URL_API}/Brands/get-all-brands`);
  }
  getBrandsbyPage(pageNumber: number, pageSize: number): Observable<any> {
    return this.http.get<any>(`${BASE_URL_API}/Brands/get-all-brands-by-page?pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }
  getBrandById(brandId: string): Observable<any> {
    return this.http.get(`${BASE_URL_API}/Brands/get-brand-by-id/${brandId}`);
  }

  addBrand(brand: any): Observable<any> {
    return this.http.post(`${BASE_URL_API}/Brands/add-new-brand`, brand);
  }

  updateBrand(brandId: string, brand: any): Observable<any> {
    return this.http.put(`${BASE_URL_API}/Brands/update-brand/${brandId}`, brand);
  }

  deleteBrand(brandId: string): Observable<any> {
    return this.http.delete(`${BASE_URL_API}/Brands/delete-brand-by-id/${brandId}`);
  }

}
