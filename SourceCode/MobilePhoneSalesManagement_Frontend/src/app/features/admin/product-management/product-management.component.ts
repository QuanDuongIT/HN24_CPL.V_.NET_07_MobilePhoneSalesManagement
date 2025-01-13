import { Component, OnInit } from '@angular/core';
import { ProductService } from './services/product.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { AddOrUpdateProductComponent } from './add-or-update-product/add-or-update-product.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { Product } from './models/product';
import { PagedResult } from '../brand-management/models/brand.model';

@Component({
  selector: 'app-product-management',
  imports: [CommonModule, FormsModule, AddOrUpdateProductComponent, NgxPaginationModule],
  templateUrl: './product-management.component.html',
  styleUrls: ['./product-management.component.css'],
  standalone: true,
})
export class ProductManagementComponent implements OnInit {
  productSubscription?: Subscription;
  page$?: Observable<PagedResult<Product>>;
  isAddProductVisible = false;
  productToUpdate?: Product;
  page: number = 1;
  pageSize: number = 10;
  totalPages: number = 1;
  isLoading: boolean = false;

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  // Hàm tải các trang cần thiết
  loadProducts(): void {
    this.isLoading = true;

    // Tải trang hiện tại trước
    this.page$ = this.productService.getProducts(this.page, this.pageSize);

    // Sau khi tải trang hiện tại, mới tải các trang liền kề và các trang đầu/cuối nếu cần
    this.page$.subscribe(res => {
      this.pageSize = res.pageSize;
      this.totalPages = res.totalPages;

      // Tải trang trước và sau nếu cần
      if (this.page > 1) {
        this.productService.getProducts(this.page - 1, this.pageSize).subscribe();
      }

      if (this.page < this.totalPages) {
        this.productService.getProducts(this.page + 1, this.pageSize).subscribe();
      }

      // Tải trang đầu nếu đang ở gần trang đầu
      if (this.page > 2) {
        this.productService.getProducts(1, this.pageSize).subscribe();
      }

      // Tải trang cuối nếu đang ở gần trang cuối
      if (this.page < this.totalPages - 1) {
        this.productService.getProducts(this.totalPages, this.pageSize).subscribe();
      }

      console.log(res.items); // Log sản phẩm
    });

    this.isLoading = false;
  }

  onItemsPerPageChange(): void {
    this.page = 1;
    this.loadProducts();
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
      },
      error: (err) => console.error('Error loading product:', err),
    });
  }

  // Điều hướng tới trang tiếp theo
  nextPage(): void {
    if (this.page < this.totalPages) {
      this.page++;
      this.loadProducts();
    }
  }

  // Điều hướng tới trang trước
  previousPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadProducts();
    }
  }

  // trackBy để tránh render lại toàn bộ bảng
  trackByProductId(index: number, product: Product): string {
    return product.productId;
  }
}
