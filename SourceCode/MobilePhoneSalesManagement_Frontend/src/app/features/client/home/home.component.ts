import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Product } from '../product/models/product.model';
import { ProductService } from '../product/services/product.service';

@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  newestProducts: Product[] = [];

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadNewestProducts();
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
}
