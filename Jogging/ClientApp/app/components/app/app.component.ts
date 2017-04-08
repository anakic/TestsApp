import { Component } from '@angular/core';
import { LoginService } from '../../login.service';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(public loginService: LoginService) {
    }
}
