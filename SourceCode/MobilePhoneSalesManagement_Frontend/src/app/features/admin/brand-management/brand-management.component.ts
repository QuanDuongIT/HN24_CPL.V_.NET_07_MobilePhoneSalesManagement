import { Component } from '@angular/core';
import { BrandService } from './services/brand.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Brand, PagedResult } from './models/brand.model';
import { Observable } from 'rxjs';
import { AddOrUpdateBrandComponent } from "./add-or-update-brand/add-or-update-brand.component";

@Component({
  selector: 'app-brand-management',
  imports: [CommonModule, FormsModule, AddOrUpdateBrandComponent],
  templateUrl: './brand-management.component.html',
  styleUrls: ['./brand-management.component.css'],
  standalone: true
})
export class BrandManagementComponent {
  page$?: Observable<PagedResult<Brand>>;
  isAddBrandVisible = false;
  brandToUpdate?: Brand;
  page: number = 1;
  pageSize: number = 1;
  totalPages: number = 1;
  isLoading: boolean = false;

  constructor(private brandService: BrandService) { }

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.isLoading = true;
    // Gọi API lấy danh sách brand với phân trang
    this.page$ = this.brandService.getBrandsbyPage(this.page, this.pageSize);
    this.page$.subscribe(res => {
      this.pageSize = res.pageSize;
      this.totalPages = res.totalPages;
    }
    );
    this.isLoading = false;
  }

  onItemsPerPageChange(): void {
    this.page = 1;
    this.loadBrands();
  }

  showAddBrand(): void {
    this.isAddBrandVisible = true;
  }

  hideAddBrand(): void {
    this.brandToUpdate = undefined;
    this.isAddBrandVisible = false;
  }

  onAddBrand(newBrandName: string): void {
    console.log('New Brand Added:', newBrandName);
    this.loadBrands(); // Tải lại danh sách thương hiệu sau khi thêm thành công
  }

  onDeleteBrand(brandId: any, isActive: boolean): void {
    const confirmMessage = isActive
      ? 'Bạn chắc chắn muốn chuyển brand này vào thùng rác?'
      : 'Bạn chắc chắn muốn xóa brand này?';

    if (confirm(confirmMessage)) {
      this.brandService.deleteBrand(brandId).subscribe({
        next: () => {
          this.loadBrands(); // Tải lại danh sách sau khi xóa thành công
        },
        error: (err) => console.error(err),
      });
    }
  }

  updateBrand(brandId: string): void {
    this.isAddBrandVisible = true;
    this.brandService.getBrandById(brandId).subscribe((brand: Brand) => {
      this.brandToUpdate = brand;
    });
  }

  // Điều hướng tới trang tiếp theo
  nextPage(): void {
    console.log("fdsfs", this.totalPages)
    if (this.page < this.totalPages) {
      this.page++;
      this.loadBrands();
      this.page$?.subscribe(res =>

        console.log(res)
      )
    }
  }

  // Điều hướng tới trang trước
  previousPage(): void {
    if (this.page > 1) {
      this.page--;
      this.loadBrands();
    }
  }
  // trackBy để tránh render lại toàn bộ bảng
  trackByBrandId(index: number, brand: Brand): string {
    return brand.brandId;
  }
}
