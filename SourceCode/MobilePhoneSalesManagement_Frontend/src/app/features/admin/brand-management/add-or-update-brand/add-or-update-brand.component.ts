import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { Brand } from '../models/brand.model';
import { BrandService } from '../services/brand.service';
import { RequestBrand } from '../models/add-brand-request.model';
import { CommonModule } from '@angular/common';

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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['brandToUpdate'] && this.brandToUpdate) {
      // Khi brandToUpdate thay đổi, cập nhật lại form
      this.model.name = this.brandToUpdate.name;
      this.model.imageUrl = this.brandToUpdate.imageUrl;
      this.model.isActive = this.brandToUpdate.isActive;
    }

    console.log(this.brandToUpdate);
  }

  closeModal() {
    this.close.emit(); // Phát sự kiện đóng modal
  }

  submitBrand() {
    this.add.emit();
    this.closeModal();
  }

  constructor(private brandService: BrandService) {
    this.model = {
      name: '',
      imageUrl: '',
      isActive: true
    }
  }

  onFormSubmit() {
    if (this.brandToUpdate)
      this.updateBrand();
    else
      this.addBrandSubscription = this.brandService.addBrand(this.model).subscribe({
        next: response => {
          this.add.emit(this.model.name);
          this.closeModal();
        },
        error: err => {
          console.log(err);
        }
      });
  }
  updateBrand() {
    // Gửi yêu cầu cập nhật thương hiệu
    if (this.brandToUpdate)
      this.addBrandSubscription = this.brandService.updateBrand(this.brandToUpdate?.brandId, this.model).subscribe({
        next: response => {
          this.closeModal();
          this.add.emit(this.model.name);
        },
        error: err => {
          console.log(err);
        }
      });
  }
}
