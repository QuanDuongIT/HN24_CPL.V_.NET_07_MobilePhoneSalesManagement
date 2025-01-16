import { Component, Renderer2 } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { ToastService } from './core/services/toast-service/toast.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [RouterOutlet],
  standalone: true,
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'MobilePhoneSalesManagement_Frontend'; // Thuộc tính mẫu cho component
  isClientLayout: boolean = false;
  isAdminLayout: boolean = false;

  constructor(private renderer: Renderer2) {}

  ngOnInit() {
    // Kiểm tra route hiện tại để xác định layout
    if (window.location.pathname.startsWith('/admin')) {
      this.isAdminLayout = true;
      this.isClientLayout = false;
      this.loadAdminScripts();
    } else {
      this.isClientLayout = true;
      this.isAdminLayout = false;
      this.loadClientScripts();
    }
  }
  loadClientScripts() {
    this.addStylesheet('/assets/css/bootstrap.min.css');
    this.addStylesheet('/assets/css/style.css');
    this.addStylesheet('/assets/css/owl.carousel.css');
    this.addStylesheet('/assets/css/owl.theme.default.css');
    this.addStylesheet('/assets/css/font-awesome.min.css');
    this.setFavicon('logo_icon.png');

    this.addScript('/assets/js/jquery.min.js', () => {
      // Sau khi jQuery đã sẵn sàng, load Bootstrap và các tệp khác
      this.addScript('/assets/js/bootstrap.min.js', () => {
        this.addScript('/assets/js/menumaker.js');
        this.addScript('/assets/js/jquery.sticky.js');
        this.addScript('/assets/js/owl.carousel.min.js');
        this.addScript('/assets/js/multiple-carousel.js');
      });
    });
  }

  loadAdminScripts() {
    // Thêm các file CSS
    this.addStylesheet('/assets/vendor/bootstrap/css/bootstrap.min.css');
    this.addStylesheet('/assets/css/all.min.css');
    this.addStylesheet(
      '/assets/fonts/fontawesome-free-5.15.4-web/css/all.min.css'
    );
    this.addStylesheet('/assets/vendor/bootstrap/css/bootstrap.min.css');
    this.addStylesheet('/assets/css/css_admin/admin.css');

    // Thêm các file JS
    this.addScript('/assets/vendor/jquery/jquery.min.js');
    this.addScript('/assets/vendor/bootstrap/js/bootstrap.bundle.min.js');
    this.addScript('node_modules/jquery/dist/jquery.min.js');
    this.addScript('node_modules/bootstrap/dist/js/bootstrap.bundle.min.js');
    this.addScript('/assets/js/menumaker.js');
    this.addScript('/assets/js/jquery.sticky.js');
    this.addScript('/assets/js/sticky-header.js');
    this.addScript('/assets/js/owl.carousel.min.js');
    this.addScript('/assets/js/multiple-carousel.js');
  }

  addStylesheet(href: string): void {
    const link = this.renderer.createElement('link');
    this.renderer.setAttribute(link, 'rel', 'stylesheet');
    this.renderer.setAttribute(link, 'href', href);
    this.renderer.appendChild(document.head, link);
  }

  addScript(src: string, callback?: () => void) {
    const script = document.createElement('script');
    script.src = src;
    script.type = 'text/javascript';
    script.async = false;

    if (callback) {
      script.onload = callback;
    }

    document.body.appendChild(script);
  }
  setFavicon(image: string): void {
    // Tạo thẻ <link>
    const link: HTMLLinkElement = this.renderer.createElement('link');
    link.rel = 'icon';
    link.type = 'image/png';
    link.href = `assets/images/${image}`; // Đường dẫn tới file favicon

    // Thêm thẻ <link> vào <head>
    this.renderer.appendChild(document.head, link);
  }
}
