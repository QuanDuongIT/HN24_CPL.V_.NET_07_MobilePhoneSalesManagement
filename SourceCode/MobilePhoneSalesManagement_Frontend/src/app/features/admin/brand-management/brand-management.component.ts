import { Component } from '@angular/core';
import { BrandService } from './services/brand.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Brand } from './models/brand.model';
import { Observable, Subscription } from 'rxjs';
import { AddOrUpdateBrandComponent } from "./add-or-update-brand/add-or-update-brand.component";
import { NgxPaginationModule } from 'ngx-pagination';

@Component({
  selector: 'app-brand-management',
  imports: [CommonModule, FormsModule, AddOrUpdateBrandComponent, NgxPaginationModule],
  templateUrl: './brand-management.component.html',
  styleUrls: ['./brand-management.component.css'],
  standalone: true
})
export class BrandManagementComponent {
  brandSubscription?: Subscription;
  brands$?: Observable<Brand[]>;
  isAddBrandVisible = false;
  brandToUpdate?: Brand;
  brandName: string = '';
  page: number = 1; // Trang hiện tại
  itemsPerPage: number = 5; // Số lượng thương hiệu trên mỗi trang (mặc định là 5)

  constructor(private brandService: BrandService) { }

  ngOnInit(): void {
    this.loadBrands();
  }

  loadBrands(): void {
    this.brands$ = this.brandService.getBrands();
  }

  onItemsPerPageChange(): void {
    // Reset về trang đầu tiên khi số lượng hiển thị trên mỗi trang thay đổi
    this.page = 1;
    console.log(`Items per page updated to: ${this.itemsPerPage}`);
  }

  showAddBrand() {
    this.isAddBrandVisible = true;
  }

  hideAddBrand() {
    this.brandToUpdate = undefined;
    this.isAddBrandVisible = false;
  }

  onAddBrand(newBrandName: string) {
    console.log('New Brand Added:', newBrandName);
    this.loadBrands(); // Tải lại danh sách thương hiệu sau khi thêm thành công
  }

  onDeleteBrand(brandId: any, isActive: boolean) {
    const confirmMessage = isActive
      ? 'Bạn chắc chắn muốn chuyển brand này vào thùng rác?'
      : 'Bạn chắc chắn muốn xóa brand này?';

    if (confirm(confirmMessage)) {
      this.brandSubscription = this.brandService.deleteBrand(brandId).subscribe({
        next: () => {
          this.brands$ = this.brandService.getBrands();
        },
        error: (err) => console.log(err),
      });
    }
  }

  updateBrand(brandId: string) {
    this.isAddBrandVisible = true;
    this.brandService.getBrandById(brandId).subscribe((brand: Brand) => {
      this.brandToUpdate = brand;
    });
  }
}
