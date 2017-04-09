import { UserService, UserData } from '../../users.service';
import { LoginService } from '../../login.service';
import { DialogService } from '../../dialog.service';
import { Router } from '@angular/router';
import { Component } from '@angular/core';

@Component({
    styleUrls: ['./create-user.component.css'],
    templateUrl: './create-user.component.html'
})
export class CreateUserComponent {

    public message: string;
    public user: NewUserData;

    public create() {
        this.message = '';
        if (this.user.password != this.user.password2) {
            this.message = 'Password 1&2 do not match!';
        }
        else {
            this.userService.create(this.user).subscribe(
                u => this.router.navigate(['login']),
                res => this.message = res._body);
        }
    }
    public cancel() {
        this.router.navigate(['login']);
    }

    constructor(private router: Router, private loginService: LoginService, private userService: UserService) {
        this.user = { id: 0, password: '', password2: '', email: '', firstName: '', lastName: '', role: 0, roleName: '' };
    }
}

interface NewUserData extends UserData{
    password2: string;
}