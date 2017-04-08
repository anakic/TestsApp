import { Component, OnInit } from '@angular/core'
import { Router } from '@angular/router'
import { LoginService } from '../../login.service';

@Component({
    styleUrls: ['./login.component.css'],
    templateUrl: './login.component.html'
})
export class LoginComponent extends OnInit {

    public isLoaded: boolean;
    public isWorking: boolean;
    public errorMessage: string;

    public email: string;
    public password: string;

    constructor(public loginService: LoginService, private router: Router) {
        super();
        this.isWorking = true;
    }

    ngOnInit() {
        this.loginService.initUserFromServer().subscribe(u=> this.isWorking = false);
    }

    public login() {
        this.isWorking = true;
        this.loginService.login(this.email, this.password).subscribe(res => {
            if (!res.success)
                this.errorMessage = res.message;
            this.isWorking = false;
        }, res => {
            this.errorMessage = (res.json().credentials)[0];
            this.isWorking = false;
        });
    }
}
