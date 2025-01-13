import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { FormBuilder, FormGroup, FormsModule, NgForm, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ToastService } from '../../../core/services/toast-service/toast.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [FormsModule , CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  email: string = '';
  code: string = '';

  user = {
    email: '',
    password: '',
    re_password: '',
    fullName: '',
    phoneNumber: '',
  };
  userModelSubmit: any
  

  constructor(private toastService: ToastService, private authService: AuthService, private route: ActivatedRoute) {
  }
  ngOnInit(): void {
    // Lấy tham số từ URL (email và code)
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      this.code = params['code'];
      
      this.verifyEmail();
    });
  }

  matchPasswords(form: NgForm) {
    const password = form.controls['password']?.value;
    const confirmPassword = form.controls['re_password']?.value;
    const email = form.controls['email']?.value;
  
    // Kiểm tra email hợp lệ
    const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
    if (email && !emailPattern.test(email) || email === '') {
      form.controls['email']?.setErrors({ invalidEmail: true });
    } else {
      form.controls['email']?.setErrors(null);
    }
  
    // Kiểm tra mật khẩu (ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường và chữ số)
    const passwordPattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$/;
    if (password && !passwordPattern.test(password) || password === '') {
      form.controls['password']?.setErrors({ weakPassword: true });
    } else {
      form.controls['password']?.setErrors(null);
    }
  
    // Kiểm tra xác nhận mật khẩu
    if (password && confirmPassword && password !== confirmPassword || confirmPassword === '') {
      form.controls['re_password']?.setErrors({ noMatch: true });
    } else {
      form.controls['re_password']?.setErrors(null);
    }
  }

  onSubmit(form: NgForm) {
    
    this.matchPasswords(form);

    if (!form.invalid) {
      this.userModelSubmit = {
        email: this.user.email,
        passwordHash: this.user.password,
        status: true,
        role: 'client',
        fullName: this.user.fullName,
        dateOfBirth: new Date().toISOString().split('T')[0],
        gender: 'Nam',
        address: '',
        phoneNumber: this.user.phoneNumber,
        lastOnlineAt: new Date()
      };
      
      this.authService.register(this.userModelSubmit).subscribe(
        (res)=> {
          if (res.success) {
            this.toastService.showSuccess(res.message);
          } else {
            this.toastService.showError(res.message);
          }
        },
        (error) => {
          this.toastService.showError(error.Message)
        }
      )
    }
  }
  
  // xác thực email
  verifyEmail(): void {
    if (this.email !== '' && this.code !== '')
    {
      console.log('heheh');
      
      this.authService.verifyEmail(this.email, this.code).subscribe(
        (response: any) => {
          if (response.success) {
            alert('Email xác nhận thành công!');
            window.location.href = '/login';  // Chuyển hướng đến trang login
          } else {
            alert('Xác nhận email thất bại!');
          }
        },
        error => {
          console.error('Lỗi khi xác nhận email:', error);
          alert('Đã xảy ra lỗi khi xác nhận email!');
        }
      );
    }
  }
}