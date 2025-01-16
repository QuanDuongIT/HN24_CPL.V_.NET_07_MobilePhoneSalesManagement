import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { Brand } from '../models/brand.model';
import { BrandService } from '../services/brand.service';
import { RequestBrand } from '../models/add-brand-request.model';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-add-or-update-brand',
  imports: [FormsModule, CommonModule],
  templateUrl: './add-or-update-brand.component.html',
  styleUrl: './add-or-update-brand.component.css'
})
export class AddOrUpdateBrandComponent {
  @Output() close = new EventEmitter<void>();
  @Output() add = new EventEmitter<string>();
  @Input() brandToUpdate?: Brand;

  model: RequestBrand;
  addBrandSubscription?: Subscription;

  constructor(private brandService: BrandService, private toastr: ToastrService) {
    this.model = {
      name: '',
      imageUrl: '',
      isActive: true
    };
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['brandToUpdate'] && this.brandToUpdate) {
      this.model.name = this.brandToUpdate.name;
      this.model.imageUrl = this.brandToUpdate.imageUrl;
      this.model.isActive = this.brandToUpdate.isActive;
    }
  }

  closeModal() {
    this.close.emit();
  }

  onFormSubmit() {
    if (this.brandToUpdate) {
      this.updateBrand();
    } else {
      this.addBrandSubscription = this.brandService.addBrand(this.model).subscribe({
        next: response => {
          this.add.emit(this.model.name);
          this.closeModal();
        },
        error: err => {
          console.log(err);
          if (err.error && err.error.Message) {
            this.toastr.error(err.error.Message, 'Lỗi');
          } else {
            this.toastr.error('Đã xảy ra lỗi khi thêm thương hiệu.', 'Lỗi');
          }
        }
      });
    }
  }


  updateBrand() {
    if (this.brandToUpdate) {
      this.addBrandSubscription = this.brandService.updateBrand(this.brandToUpdate.brandId, this.model).subscribe({
        next: response => {
          this.toastr.success('Thương hiệu đã được cập nhật thành công!', 'Thành công');
          this.add.emit(this.model.name);
          this.closeModal();
        },
        error: err => {
          console.log(err);
          if (err.error && err.error.Message) {
            this.toastr.error(err.error.Message, 'Lỗi');
          } else {
            this.toastr.error('Đã xảy ra lỗi khi thêm thương hiệu.', 'Lỗi');
          }
        }
      });
    }
  }
}
