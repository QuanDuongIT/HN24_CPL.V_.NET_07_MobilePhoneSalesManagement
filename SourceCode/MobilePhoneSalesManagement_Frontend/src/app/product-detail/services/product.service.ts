import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private apiUrl = 'https://localhost:7001/api/ProductDetails/get-product-details/1';  // Replace with your API URL

    constructor(private http: HttpClient) { }

    getProductById(id: string): Observable<any> {
        return this.http.get(`${this.apiUrl}/${id}`);  // Fetch product details by ID
    }
}
