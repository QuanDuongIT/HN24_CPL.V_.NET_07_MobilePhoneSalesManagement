import { Routes } from '@angular/router';
import { OrderManagementComponent } from './features/admin/order-management/order-management.component';
import { ProductManagementComponent } from './features/admin/product-management/product-management.component';
import { BrandManagementComponent } from './features/admin/brand-management/brand-management.component';
import { UserManagementComponent } from './features/admin/user-management/user-management.component';
import { AdminLayoutComponent } from './features/admin/admin-layout/admin-layout.component';
import { ClientLayoutComponent } from './features/client/client-layout/client-layout.component';
import { HomeComponent } from './features/client/home/home.component';
import { ProductListComponent } from './features/client/product/product-list/product-list.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { VerifyEmailComponent } from './features/auth/verify-email/verify-email.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
import { AccountComponent } from './features/client/user/account/account.component';
import { authGuard } from './features/auth/guards/auth.guard';

export const routes: Routes = [
  // { path: 'admin', redirectTo: '/admin/user-management', pathMatch: 'full' },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    // canActivate: [authGuard],
    data: { role: 'admin' },
    children: [
      { path: 'user-management', component: UserManagementComponent },
      { path: 'brand-management', component: BrandManagementComponent },
      { path: 'product-management', component: ProductManagementComponent },
      { path: 'order-management', component: OrderManagementComponent },
    ],
  },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: '',
    component: ClientLayoutComponent, // Giao diện layout cho client
    children: [
      { path: 'home', component: HomeComponent }, // Trang chủ của client
      { path: 'products', component: ProductListComponent }, // Danh sách sản phẩm
      // { path: 'product/:id', component: ProductDetailComponent }, // Chi tiết sản phẩm
      // { path: 'cart', component: CartComponent }, // Giỏ hàng
      // { path: 'checkout', component: CheckoutComponent }, // Thanh toán

      // auth
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent },
      { path: 'reset-password', component: ResetPasswordComponent },
      // user
      { path: 'account', component: AccountComponent },
      { path: 'verify-email', component: VerifyEmailComponent },
    ]
  }
];
