import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from "../../../core/components/admin/navbar/navbar.component";
import { HeaderComponent } from '../../../core/components/common/header/header.component';

@Component({
  selector: 'app-client-layout',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './client-layout.component.html',
  styleUrl: './client-layout.component.css'
})
export class ClientLayoutComponent {

}
