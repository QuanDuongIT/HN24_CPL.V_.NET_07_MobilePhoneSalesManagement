import { Component, model } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { UserService } from '../../../admin/user-management/services/user.service';
import { ToastService } from '../../../../core/services/toast-service/toast.service';
import dayjs from 'dayjs';
import { CommonModule } from '@angular/common';
import { ValidatorsService } from '../../../../core/services/validators-service/validators.service';
import { AuthService } from '../../../auth/services/auth.service';
import { log } from '@angular-devkit/build-angular/src/builders/ssr-dev-server';
import { Router } from '@angular/router';
import { UserClientService } from '../service/user-client.service';

@Component({
  selector: 'app-account',
  imports: [FormsModule, CommonModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css',
})
export class AccountComponent {
  user = {
    fullName: '',
    email: '',
    phoneNumber: '',
    address: '',
    dateOfBirth: '',
    gender: 'male',
  };
  passwordModel = {
    currentPassword: '',
    password: '',
    re_password: '',
  };
  private modelChangePassword: any;

  currentTab: string = 'account-info';

  constructor(
    private router: Router,
    private userService: UserClientService,
    private toastService: ToastService,
    private validateService: ValidatorsService
  ) {}

  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe(
      (res) => {
        this.user.fullName = res.data.fullName;
        this.user.email = res.data.email;
        this.user.phoneNumber = res.data.phoneNumber;
        this.user.address = res.data.address;
        this.user.dateOfBirth = dayjs(res.data.dateOfBirth).format(
          'YYYY-MM-DD'
        );
        this.user.gender = res.data.gender;
      },
      (error) => {
        this.router.navigateByUrl('/login');
      }
    );
  }
  onSubmit(): void {
    this.userService.updateCurrentUser(this.user).subscribe(
      (res) => {
        if (res.success) {
          this.toastService.showSuccess(res.message);
          setTimeout(() => {
            window.location.reload();
          }, 600);
        } else {
          this.toastService.showError(res.message);
        }
      },
      (error) => {
        this.toastService.showError('Có lỗi bên server');
        console.error('Failed to get user info:', error);
      }
    );
  }

  switchTab(tab: string) {
    this.currentTab = tab;
  }

  onChangePassword(form: NgForm) {
    this.validateService.matchPasswords(form);
    // Gửi dữ liệu tới server hoặc xử lý logic khác
    if (!form.invalid) {
      this.modelChangePassword = {
        currentPassword: this.passwordModel.currentPassword,
        newPassword: this.passwordModel.password,
        confirmPassword: this.passwordModel.re_password
      }
      this.userService.changePassword(this.modelChangePassword).subscribe(
        (res) => {
          if (res.success) {
            this.toastService.showSuccess(res.message);
            setTimeout(() => {
              window.location.reload();
            }, 600);
          } else {
            this.toastService.showError(res.message);
          }
        },
        (err) => {
          console.error('Đổi mật khẩu thất bại:', err);
          if (!err.error.success) {
            this.toastService.showError(err.error.message);
          }
          else {
            this.toastService.showError('Lỗi gửi yêu cầu');
          }
        }
      );
    }
  }
}
