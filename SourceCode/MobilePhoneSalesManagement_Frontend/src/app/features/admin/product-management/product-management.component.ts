import { Component, OnInit } from '@angular/core';
import { ProductService } from './services/product.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { AddOrUpdateProductComponent } from './add-or-update-product/add-or-update-product.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { Product } from './models/product';

@Component({
  selector: 'app-product-management',
  imports: [CommonModule, FormsModule, AddOrUpdateProductComponent, NgxPaginationModule],
  templateUrl: './product-management.component.html',
  styleUrls: ['./product-management.component.css'],
  standalone: true,
})
export class ProductManagementComponent implements OnInit {
  productSubscription?: Subscription;
  products$?: Observable<Product[]>; // Observable danh sách sản phẩm
  isAddProductVisible = false; // Hiển thị modal thêm/cập nhật sản phẩm
  productToUpdate?: Product; // Sản phẩm cần cập nhật
  page: number = 1; // Trang hiện tại
  itemsPerPage: number = 5; // Số sản phẩm hiển thị trên mỗi trang
  productName: string = ''; // Bộ lọc tên sản phẩm (nếu có)

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  // Lấy danh sách sản phẩm từ ProductService
  loadProducts(): void {
    this.products$ = this.productService.getProducts();
    this.products$.subscribe({
      next: (products) => console.log('Products loaded:', products),
      error: (err) => console.error('Error loading products:', err),
    });
  }

  // Xử lý thay đổi số lượng sản phẩm hiển thị trên mỗi trang
  onItemsPerPageChange(): void {
    this.page = 1; // Reset về trang đầu tiên
    console.log(`Items per page updated to: ${this.itemsPerPage}`);
  }

  // Hiển thị modal thêm sản phẩm
  showAddProduct(): void {
    this.isAddProductVisible = true;
  }

  // Ẩn modal thêm sản phẩm
  hideAddProduct(): void {
    this.productToUpdate = undefined;
    this.isAddProductVisible = false;
  }

  // Xử lý thêm sản phẩm mới
  onAddProduct(newProduct: Product): void {
    console.log('New Product Added:', newProduct);
    this.loadProducts(); // Tải lại danh sách sản phẩm
    this.hideAddProduct(); // Ẩn modal
  }

  // Xóa sản phẩm
  onDeleteProduct(productId: string, isActive: boolean): void {
    const confirmMessage = isActive
      ? 'Bạn chắc chắn muốn chuyển product này vào thùng rác?'
      : 'Bạn chắc chắn muốn xóa product này?';

    if (confirm(confirmMessage)) {
      this.productSubscription = this.productService.deleteProduct(productId).subscribe({
        next: () => {
          console.log(`Product ${productId} deleted successfully.`);
          this.loadProducts(); // Tải lại danh sách sản phẩm
        },
        error: (err) => console.error('Error deleting product:', err),
      });
    }
  }

  // Cập nhật sản phẩm
  updateProduct(productId: string): void {
    this.isAddProductVisible = true;
    this.productService.getProductById(productId).subscribe({
      next: (product: Product) => {
        this.productToUpdate = product;
        console.log('Product loaded for update:', product);
      },
      error: (err) => console.error('Error loading product:', err),
    });
  }
}
