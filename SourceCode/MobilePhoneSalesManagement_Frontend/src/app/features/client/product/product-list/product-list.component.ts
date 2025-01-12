import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit {
  products: any[] = [];
  brands: string[] = ['Samsung', 'Apple', 'Xiaomi', 'Vsmart', 'Oppo', 'Vivo', 'Nokia', 'Huawei'];
  priceRanges = [
    { label: 'Dưới 2 triệu', value: 'under2m' },
    { label: '2 - 5 triệu', value: '2to5m' },
    { label: '5 - 10 triệu', value: '5to10m' },
    { label: '10 - 15 triệu', value: '10to15m' },
    { label: 'Trên 15 triệu', value: 'above15m' }
  ];
  screenSizes = [
    { label: 'Dưới 5 inch', value: 'under5' },
    { label: 'Trên 6 inch', value: 'above6' }
  ];
  internalMemories = [
    { label: 'Dưới 32GB', value: 'under32' },
    { label: '64GB hoặc 128GB', value: '64and128' },
    { label: '256GB hoặc 512GB', value: '256and512' },
    { label: 'Trên 512GB', value: 'above512' }
  ];
  filterRequest: any = {
    Brands: [],
    Prices: [],
    ScreenSizes: [],
    InternalMemory: [],
    Sort: ''
  };
  constructor(private productService: ProductService) { }
  ngOnInit() {
    this.getFilteredProducts();
  }
  getFilteredProducts() {
    console.log('filterRequest:', this.filterRequest);
    this.productService.filterProducts(this.filterRequest).subscribe(
      (data) => {
        this.products = data;
      },
      (error) => {
        console.error('Lỗi khi lấy danh sách sản phẩm:', error);
      }
    );
  }
  onBrandFilterChange(event: any) {
    const brand = event.target.value;
    if (event.target.checked) {
      this.filterRequest.Brands.push(brand);
    } else {
      this.filterRequest.Brands = this.filterRequest.Brands.filter((b: string) => b !== brand);
    }
    this.getFilteredProducts();
  }
  onPriceFilterChange(event: any) {
    const price = event.target.value;
    if (event.target.checked) {
      this.filterRequest.Prices.push(price);
    } else {
      this.filterRequest.Prices = this.filterRequest.Prices.filter((p: string) => p !== price);
    }
    this.getFilteredProducts();
  }
  onScreenSizeFilterChange(event: any) {
    const screenSize = event.target.value;
    if (event.target.checked) {
      this.filterRequest.ScreenSizes.push(screenSize);
    } else {
      this.filterRequest.ScreenSizes = this.filterRequest.ScreenSizes.filter((s: string) => s !== screenSize);
    }
    this.getFilteredProducts();
  }
  onInternalMemoryFilterChange(event: any) {
    const memory = event.target.value;
    if (event.target.checked) {
      this.filterRequest.InternalMemory.push(memory);
    } else {
      this.filterRequest.InternalMemory = this.filterRequest.InternalMemory.filter((m: string) => m !== memory);
    }
    this.getFilteredProducts();
  }
  onSortChange(event: any) {
    this.filterRequest.Sort = event.target.value;
    this.getFilteredProducts();
  }
}
