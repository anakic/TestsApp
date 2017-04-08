import { Component } from '@angular/core';
import { LoginService } from '../../login.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    constructor(public loginService: LoginService) {
    }
}
