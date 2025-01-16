import { Component } from '@angular/core';
import { CartService } from './service/cart.service';
import { error } from 'jquery';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../../core/services/toast-service/toast.service';
import { log } from '@angular-devkit/build-angular/src/builders/ssr-dev-server';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent {
  cartItems: any[] = []
  count: number = 0
  totalBeforeDiscount: number = 0;
  discountAmount: number = 0; // Giảm giá có thể được nhập từ một mã giảm giá
  totalAmount: number = 0;
  discountCode: string = '';
  selectedProductIds: number[] = [];

  constructor (private cartService: CartService, private toastService: ToastService){
    this.discountCode = ""
  }

  ngOnInit(): void {
    this.fetchCartItems();
  }
  fetchCartItems (): void {
    this.cartService.getCartItems().subscribe(
      (res) => {
        this.cartItems = res;
        this.count = this.cartItems.length;
        this.calculateTotal();
      },
      (err) => {
        console.log(err);
      }
    )
  }
  calculateTotal(): void {
    this.totalBeforeDiscount = this.cartItems.reduce((total, item) => total + (item.price * item.quantity), 0);

    // Xử lý mã giảm giá động
    if (this.discountCode.startsWith('DISCOUNT_')) {
      const discountPercent = parseInt(this.discountCode.split('_')[1], 10);  // Lấy phần trăm từ mã (ví dụ: 10 từ DISCOUNT_10)
      if (!isNaN(discountPercent) && discountPercent > 0) {
        this.discountAmount = this.totalBeforeDiscount * (discountPercent / 100);  // Giảm theo phần trăm
      } else {
        this.discountAmount = 0;  // Nếu phần trăm không hợp lệ, không giảm giá
      }
    } else {
      this.discountAmount = 0;  // Nếu mã không phải là mã giảm giá hợp lệ
    }

    // Tính tổng cộng sau khi giảm giá
    this.totalAmount = this.totalBeforeDiscount - this.discountAmount;
  }
  applyDiscountCode(): void {
    // Kiểm tra mã giảm giá và tính toán
    if (this.discountCode.startsWith('DISCOUNT_')) {
      this.toastService.showSuccess('Áp dụng thành công');
      this.calculateTotal();
    } else {
      this.toastService.showError('Mã giảm giá không hợp lệ');
    }
  }
  onCheckboxChange(productId: number, event: any): void {
    if (event.target.checked) {
      // Nếu checkbox được chọn, thêm productId vào mảng
      this.selectedProductIds.push(productId);
    } else {
      // Nếu checkbox bị bỏ chọn, loại bỏ productId khỏi mảng
      this.selectedProductIds = this.selectedProductIds.filter(id => id !== productId);
    }
    console.log(this.selectedProductIds); // Hiển thị mảng các productId đã chọn
  }
  // Hàm chọn hoặc bỏ chọn tất cả các sản phẩm
  onSelectAll(event: any): void {
    if (event.target.checked) {
      // Chọn tất cả sản phẩm
      this.selectedProductIds = this.cartItems.map(item => item.productId);
    } else {
      // Bỏ chọn tất cả sản phẩm
      this.selectedProductIds = [];
    }
    console.log(this.selectedProductIds); // In ra mảng các productId đã chọn
  }
  // Format currency to Vietnamese VND
  formatCurrency(number: number): string {
    return number.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
  }

  // Decrease quantity
  dcQuantity(productId: number): void {
    this.cartService.updateCart(productId, -1).subscribe(
      (res) => {
        if (res.success) {
          this.toastService.showSuccess(res.message);
          this.fetchCartItems();
        } else {
          this.toastService.showError(res.message)
        }
      },
      (err) => {
        console.error(err);
      }
    )
  }

  // Increase quantity
  icQuantity(productId: number): void {
    this.cartService.updateCart(productId, 1).subscribe(
      (res) => {
        if (res.success) {
          this.toastService.showSuccess(res.message);
          this.fetchCartItems();
        } else {
          this.toastService.showError(res.message)
        }
      },
      (err) => {
        console.error(err);
      }
    )
  }

  // Remove item from cart
  removeItem(productId: number): void {
    this.cartService.deleteCartItem(productId).subscribe(
      (res) => {
        if (res.success) {
          this.toastService.showSuccess(res.message);
          this.fetchCartItems();
        } else {
          this.toastService.showError(res.message)
        }
      },
      (err) => {
        console.error(err);
      }
    )
  }
}
