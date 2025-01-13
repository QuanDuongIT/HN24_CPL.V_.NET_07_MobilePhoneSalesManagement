import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from "../../../core/components/admin/navbar/navbar.component";
import { HeaderComponent } from '../../../core/components/common/header/header.component';
import { Component, OnInit, Renderer2 } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-client-layout',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './client-layout.component.html',
  styleUrl: './client-layout.component.css',
  standalone: true,
})



export class ClientLayoutComponent implements OnInit {

  constructor(private renderer: Renderer2, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.loadClientAssets();
  }

  loadClientAssets(): void {
    // Nhúng CSS
    this.addCss('/src/assets/css/bootstrap.min.css');
    this.addCss('/src/assets/css/style.css');
    this.addCss('/src/assets/css/owl.carousel.css');
    this.addCss('/src/assets/css/owl.theme.default.css');
    this.addCss('/src/assets/css/font-awesome.min.css');

    // Nhúng JS
    this.addJs('/src/assets/js/jquery.min.js');
    this.addJs('/src/assets/js/bootstrap.min.js');
    this.addJs('/src/assets/js/menumaker.js');
    this.addJs('/src/assets/js/jquery.sticky.js');
    this.addJs('/src/assets/js/sticky-header.js');
    this.addJs('/src/assets/js/owl.carousel.min.js');
    this.addJs('/src/assets/js/multiple-carousel.js');
  }

  addCss(href: string): void {
    const link = this.renderer.createElement('link');
    link.rel = 'stylesheet';
    link.href = href;
    this.renderer.appendChild(document.head, link);
  }

  addJs(src: string): void {
    const script = this.renderer.createElement('script');
    script.type = 'text/javascript';
    script.src = src;
    this.renderer.appendChild(document.body, script);
  }
}
