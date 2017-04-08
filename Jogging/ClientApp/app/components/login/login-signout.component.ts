import { Component, OnInit } from '@angular/core';
import { LoginService, LoginData } from '../../login.service';

@Component({
    selector: 'login-signout',
    templateUrl: './login-signout.component.html',
    styleUrls: ['./login-signout.component.css']
})
export class LoginSignoutComponent {

    constructor(public loginService: LoginService) { }

    public signout() {
        this.loginService.signout().subscribe(() => { });
    }
}