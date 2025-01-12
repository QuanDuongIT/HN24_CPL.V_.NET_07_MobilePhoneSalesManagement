import { Component, Input, SimpleChanges } from '@angular/core';
import { UserService } from '../services/user.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-edit',
  imports: [FormsModule, CommonModule],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.css'
})

export class UserEditComponent {
  @Input() user: any = {};
  
  isModalOpen = false;  // Biến để điều khiển việc hiển thị modal
  action: 'delete' = 'delete'

  constructor(private userService: UserService) {}

  ngOnChanges(changes: SimpleChanges) {
    
    if (changes['user'] && this.user.email != null) {
      this.user.dateOfBirth = new Date(this.user.dateOfBirth).toISOString().split('T')[0];
    }
  }

  // Mở modal xác nhận xóa người dùng
  openModal(action: 'delete') {
    if (action === 'delete') {
      this.isModalOpen = true;  // Mở modal xác nhận xóa
    }
  }

  // Đóng modal
  closeModal() {
    this.isModalOpen = false;
  }

  confirmAction() {
    if (this.action === 'delete') {
      this.deleteUserById();
    }
    this.closeModal(); // Đóng modal sau khi thực hiện hành động
  }

  // Xóa người dùng
  deleteUserById(): void {
    this.userService.deleteUserById(this.user.userId).subscribe(
      (res) => {
        alert('Xóa người dùng thành công.');
        window.location.reload();
        this.closeModal();  // Đóng modal sau khi xóa thành công
      },
      (error) => {
        console.error(error);
        alert('Đã có lỗi xảy ra khi xóa người dùng.');
        this.closeModal();  // Đóng modal nếu có lỗi
      }
    );
  }

  submitForm() {
    if (!this.user.email || !this.user.passwordHash || !this.user.fullName || !this.user.dateOfBirth || !this.user.phoneNumber) {
      alert("Vui lòng nhập đủ thông tin yêu cầu!");
      return;
    }

    this.userService.updateUser(this.user.userId, this.user).subscribe(
      (response) => {
        alert ("Cập nhật thành công!");
        window.location.reload();
      },
      (error) => {
        alert(error.error.message)
      }
    );
  }
}
