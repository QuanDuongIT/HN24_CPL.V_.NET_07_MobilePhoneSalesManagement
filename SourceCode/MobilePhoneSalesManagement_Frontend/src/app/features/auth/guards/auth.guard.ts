import { CanActivateFn, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

export const authGuard: CanActivateFn = (route, state) => {
  const cookieService = inject(CookieService);
  const authService = inject(AuthService);
  const router = inject(Router);
  const user = authService.getUser();

  let token = cookieService.get('Authentication');

  if (user && token) {
    token = token.replace('Bearer ', '');

    const decodeToken: any = jwtDecode(token);

    const expirationDate = decodeToken.exp * 1000;
    const currentTime = new Date().getTime();

    if (expirationDate < currentTime) {
      authService.logout();
      return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } })
    }
    
    // Kiểm tra role của người dùng (nếu route yêu cầu role admin)
    const requiredRole = route.data?.['role']; // Lấy role yêu cầu từ route data
    const userRole = decodeToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    console.log(requiredRole);
    console.log(userRole);
    
    
    if (requiredRole && userRole !== requiredRole) {
      window.location.href = `/login`;
      return false;
    }

    return true; 
  }

  authService.logout();
  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } })
};