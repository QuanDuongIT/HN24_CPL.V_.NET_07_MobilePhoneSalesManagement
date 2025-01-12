import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { map, Observable, Subscription } from 'rxjs';
import { ProductService } from '../services/product.service';
import { RequestProduct } from '../models/add-product-request.model';
import { CommonModule } from '@angular/common';
import { Product } from '../models/product';
import { Brand } from '../../brand-management/models/brand.model';
import { BrandService } from '../../brand-management/services/brand.service';
import { productSpecifications, specificationType } from '../models/specificationType';
import { ProductSpecificationWithEditMode } from '../models/add-specificationType-request';

@Component({
  selector: 'app-add-or-update-product',
  imports: [FormsModule, CommonModule],
  templateUrl: './add-or-update-product.component.html',
  styleUrl: './add-or-update-product.component.css'
})
export class AddOrUpdateProductComponent {

  @Output() close = new EventEmitter<void>();
  @Output() add = new EventEmitter<Product>();
  @Input() productToUpdate?: Product;

  model: RequestProduct;
  brands$?: Observable<Brand[]>;
  specificationTypes$?: Observable<specificationType[]>; // Danh sách SpecificationType
  selectedSpecificationType?: specificationType; // Giá trị được chọn trong select
  addProductSubscription?: Subscription;
  updatedProductSpecifications?: ProductSpecificationWithEditMode[] = [];
  // Default Specification Type
  private defaultSpecificationType: specificationType = {
    specificationTypeId: '0',
    name: 'new specification type'
  };
  constructor(private productService: ProductService, private brandService: BrandService) {
    this.model = {
      name: '',
      imageUrl: '',
      brandId: 0,
      color: '',
      description: '',
      discount: 0,
      manufacturer: '',
      oldPrice: 0,
      price: 0,
      stockQuantity: 0,
      isActive: true,
      productSpecifications: []
    };
  }

  ngOnInit(): void {

    this.specificationTypes$ = this.productService.getSpecificationTypes();
    this.specificationTypes$ = this.productService.getSpecificationTypes().pipe(
      map((types: specificationType[]) => [this.defaultSpecificationType, ...types])
    );
    this.brands$ = this.brandService.getBrands();
  }

  addSpecification() {

    console.log(this.model.productSpecifications)
    if (this.selectedSpecificationType) {
      this.model.productSpecifications.push({
        specificationTypeId: this.selectedSpecificationType.specificationTypeId,
        value: '',
        specificationType: { name: this.selectedSpecificationType.name }
      });
      this.selectedSpecificationType = undefined; // Reset sau khi thêm
    }
  }

  removeSpecification(index: number): void {
    if (index > -1) {
      this.model.productSpecifications.splice(index, 1);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['productToUpdate'] && this.productToUpdate) {
      // Khi productToUpdate thay đổi, cập nhật lại form
      this.model = this.productToUpdate;

      // Thêm thuộc tính editMode mà không làm thay đổi dữ liệu gốc
      this.updatedProductSpecifications = this.model.productSpecifications.map(spec => ({
        ...spec,  // Lấy tất cả các thuộc tính gốc
        editMode: false  // Thêm thuộc tính mới
      }));

      console.log(this.updatedProductSpecifications);

    }

    console.log(this.productToUpdate);
    console.log(this.model);
  }

  closeModal() {
    this.close.emit(); // Phát sự kiện đóng modal
  }

  submitProduct() {
    this.add.emit();
    this.closeModal();
  }


  onFormSubmit() {

    console.log(this.model);
    if (this.productToUpdate)
      this.updateProduct();
    else
      this.addProductSubscription = this.productService.addProduct(this.model).subscribe({
        next: response => {
          this.add.emit();
          this.closeModal();
        },
        error: err => {
          console.log(err);
        }
      });
  }
  updateProduct() {
    // Gửi yêu cầu cập nhật thương hiệu
    if (this.productToUpdate)
      this.addProductSubscription = this.productService.updateProduct(this.productToUpdate?.productId, this.model).subscribe({
        next: response => {
          this.closeModal();
          this.add.emit();
        },
        error: err => {
          console.log(err);
        }
      });
  }

}

