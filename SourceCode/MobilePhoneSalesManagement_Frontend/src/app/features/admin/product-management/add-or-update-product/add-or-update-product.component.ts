import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { map, Observable, Subscription } from 'rxjs';
import { ProductService } from '../services/product.service';
import { RequestProduct } from '../models/add-product-request.model';
import { CommonModule } from '@angular/common';
import { Product } from '../models/product';
import { Brand } from '../../brand-management/models/brand.model';
import { BrandService } from '../../brand-management/services/brand.service';
import { SpecificationTypeManagementComponent } from '../../specification-type-management/specification-type-management.component';
import { ProductSpecificationWithEditMode } from '../../specification-type-management/models/add-specificationType-request';
import { specificationType } from '../../specification-type-management/models/specificationType';

@Component({
  selector: 'app-add-or-update-product',
  imports: [FormsModule, CommonModule, SpecificationTypeManagementComponent],
  templateUrl: './add-or-update-product.component.html',
  styleUrl: './add-or-update-product.component.css'
})
export class AddOrUpdateProductComponent {



  @Output() close = new EventEmitter<void>();
  @Output() add = new EventEmitter<Product>();
  @Input() productToUpdate?: Product;
  @Input() isAddProductVisible?: Product;

  model: RequestProduct;
  brands$?: Observable<Brand[]>;
  selectedColor: string = '';
  specificationTypes$?: Observable<specificationType[]>; // Danh sách SpecificationType
  selectedSpecificationType?: specificationType; // Giá trị được chọn trong select
  addProductSubscription?: Subscription;
  updatedProductSpecifications?: ProductSpecificationWithEditMode[] = [];
  colors: { id: number, color: string }[] = [];
  presetColors: string[] = [
    'red', 'blue', 'green', 'yellow', 'orange',
    'purple', 'pink', 'gray', 'white', 'black',
    'gold', 'silver', 'lavender'
  ];

  isAddSpecificationType = true;


  constructor(private productService: ProductService, private brandService: BrandService) {
    this.model = {
      name: '',
      imageUrl: '',
      brandId: 0,
      colors: '',
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

    );
    this.brands$ = this.brandService.getBrands();
  }
  // Thêm màu sắc mới
  addColor() {
    if (this.selectedColor) {
      const newColor = { id: Date.now(), color: this.selectedColor }; // Sử dụng timestamp làm id
      this.colors.push(newColor);
      this.selectedColor = ''; // Reset ô nhập
    }
  }

  // Kiểm tra xem màu đã được chọn chưa
  // Kiểm tra xem màu đã được chọn chưa
  isColorSelected(color: string): boolean {
    return this.colors.some(c => c.color === color);  // Kiểm tra nếu có màu trùng trong mảng colors
  }

  resetForm() {
    // Reset model về giá trị mặc định
    this.model = {
      name: '',
      imageUrl: '',
      brandId: 0,
      colors: '',
      description: '',
      discount: 0,
      manufacturer: '',
      oldPrice: 0,
      price: 0,
      stockQuantity: 0,
      isActive: true,
      productSpecifications: []
    };
    this.selectedSpecificationType = undefined;

    this.colors = [];

  }
  showOnhowOnSpecificationType() {
    this.isAddSpecificationType = true;
    this.specificationTypes$?.subscribe(res =>

      console.log(res)
    )
  }
  // Thêm màu từ preset màu
  selectPresetColor(color: string) {
    // Kiểm tra xem màu đã có trong mảng colors chưa
    const colorIndex = this.colors.findIndex(c => c.color === color);

    if (colorIndex === -1) {
      // Nếu màu chưa có, thêm màu vào mảng
      this.colors.push({ id: Date.now(), color: color }); // Sử dụng timestamp làm id cho mỗi màu
    } else {
      // Nếu màu đã có, xóa màu khỏi mảng
      this.colors.splice(colorIndex, 1);
    }
  }


  // Xóa màu khỏi danh sách đã chọn
  removeColor(index: number): void {
    this.colors.splice(index, 1);
    this.colors.splice(index, 1);
  }

  filterSpecificationTypes() {
    this.specificationTypes$ = this.specificationTypes$?.pipe(
      map(types =>
        types.filter(type =>
          !this.model.productSpecifications.some(spec => spec.specificationTypeId === type.specificationTypeId)
        )
      )
    );
  }

  addSpecification() {
    if (this.selectedSpecificationType) {
      // Thêm thông số mới vào model.productSpecifications
      this.model.productSpecifications.push({
        specificationTypeId: this.selectedSpecificationType.specificationTypeId,
        value: '',
        specificationType: { name: this.selectedSpecificationType.name }
      });

      // Xóa thông số đã thêm ra khỏi danh sách specificationTypes$
      this.filterSpecificationTypes();

      // Reset sau khi thêm
      this.selectedSpecificationType = undefined;
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

      // Chuyển đổi chuỗi color từ API thành mảng đối tượng
      if (this.model.colors) {
        this.colors = this.model.colors.split(',').map(color => ({
          id: Date.now(),  // Tạo id tạm thời cho mỗi màu
          color: color.trim()  // Loại bỏ khoảng trắng thừa
        }));
      }

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
    this.model.colors = this.colors.map(color => color.color).join(', ');
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

  hideAddSpecificationType() {
    this.isAddSpecificationType = false;
    this.specificationTypes$ = this.productService.getSpecificationTypes();
  }
}

