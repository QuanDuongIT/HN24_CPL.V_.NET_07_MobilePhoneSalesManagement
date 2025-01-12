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

  // // Xóa người dùng theo danh sách ID
  // deleteBrandsByIdList(brandIds: number[]): Observable<any> {
  //   return this.http.delete(`${BASE_URL_API}/brands/delete-brands-by-id-list`, {
  //     body: brandIds,
  //   });
  // }

  // // Lọc người dùng theo số ngày hoạt động gần đây
  // filterByLastActive(days: number): Observable<any> {
  //   return this.http.get(`${BASE_URL_API}/brands/filter-by-last-active/${days}`);
  // }

  // // Lọc người dùng theo từ khóa tìm kiếm
  // filterByKeySearch(query: string): Observable<any> {
  //   return this.http.get(`${BASE_URL_API}/brands/filter-search/${query}`);
  // }

  // toggleBlockBrand(brandId: number): Observable<any> {
  //   return this.http.post<any>(`${BASE_URL_API}/brands/toggle-block/${brandId}`, null);
  // }
  // toggleBlockBrands(brandIds: number[]): Observable<any> {
  //   return this.http.post(`${BASE_URL_API}/brands/toggle-block-brands`, brandIds);
  // }
}
