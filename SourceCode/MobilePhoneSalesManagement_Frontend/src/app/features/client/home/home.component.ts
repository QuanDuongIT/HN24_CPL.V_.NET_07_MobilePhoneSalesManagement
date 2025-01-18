import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Product } from '../../admin/product-management/models/product';
import { ProductService } from '../product/services/product.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  newestProducts: Product[] = [];
  discountedProducts: Product[] = [];

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadNewestProducts();
    this.loadDiscountedProducts();
  }

  loadNewestProducts(): void {
    this.productService.newestProducts().subscribe(
      (products) => {
        this.newestProducts = products;
      },
      (error) => {
        console.error('Error loading newest products:', error);
      }
    );
  }
  loadDiscountedProducts(): void {
    this.productService.discountedProducts().subscribe(
      (products) => {
        this.discountedProducts = products;
      },
      (error) => {
        console.error('Error loading newest products:', error);
      }
    );
  }
}
