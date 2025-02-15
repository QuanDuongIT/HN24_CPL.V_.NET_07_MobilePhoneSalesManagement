import { Component } from '@angular/core';
import { Product } from '../../../admin/product-management/models/product';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ProductService } from '../../../admin/product-management/services/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-item',
  imports: [CommonModule, RouterModule],
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.css'
})
export class ProductItemComponent {
  pagination: any;
  reviews: any;
  changePage(arg0: number) {
    throw new Error('Method not implemented.');
  }
  filterRequest: any;
  toggleWishList(arg0: any) {
    throw new Error('Method not implemented.');
  }
  addToCart(arg0: any) {
    throw new Error('Method not implemented.');
  }
  onSortChange($event: Event) {
    throw new Error('Method not implemented.');
  }
  onInternalMemoryFilterChange($event: Event) {
    throw new Error('Method not implemented.');
  }
  screenSizes: any;
  internalMemories: any;
  onPriceFilterChange($event: Event) {
    throw new Error('Method not implemented.');
  }
  brands: any;
  priceRanges: any;
  onBrandFilterChange($event: Event) {
    throw new Error('Method not implemented.');
  }
  product?: Product;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    console.log(id);
    if (id) {
      this.productService.getProductById(id).subscribe((data: any) => {
        this.product = data;
        this.reviews = data.reviews;
        console.log(this.product)
      });
    }
  }
}
