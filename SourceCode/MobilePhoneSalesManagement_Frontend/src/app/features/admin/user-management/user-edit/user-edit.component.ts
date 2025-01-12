import { Component, Input, SimpleChanges } from '@angular/core';
import { UserService } from '../services/user.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user-edit',
  imports: [FormsModule],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.css'
})

export class UserEditComponent {
  @Input() user: any = {};
  constructor(private userService: UserService) {}

  ngOnChanges(changes: SimpleChanges) {
    
    if (changes['user'] && this.user.email != null) {
      this.user.dateOfBirth = new Date(this.user.dateOfBirth).toISOString().split('T')[0];
    }
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
