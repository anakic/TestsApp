import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http } from '@angular/http';
import { Observable } from 'rxjs'

@Injectable()
export class LoginService {

    public redirectUrl: string;

    public user: User;

    public initUserFromServer() {
        this.setUserAndRedirect({ id: 1, firstName: 'Test', lastName: 'User', email: 'test@user.com', canCrudAllEntries: false, canCrudUsers: false });
    }

    private setUserAndRedirect(user: User) {
        this.user = user;
        if (user)
            this.router.navigateByUrl(this.redirectUrl);
    }

    constructor(private http: Http, private router: Router) {
        this.redirectUrl = '/entries';
        this.initUserFromServer();
    }
}

export interface User {
    id: number,
    firstName: string,
    lastName: string,
    email: string,
    canCrudUsers: boolean,
    canCrudAllEntries: boolean
}