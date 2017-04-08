import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Rx'
import 'rxjs/add/operator/map';

@Injectable()
export class LoginService {

    public loginUrl: string;
    public redirectUrl: string;

    public user: User;

    public initUserFromServer(): Observable<User> {
        return this.http
            .get(`/api/Account`, { withCredentials: true })
            .map(result => {
                return this.setUserAndRedirect(result.ok ? result.json() as User : null);
            });
    }

    public signout(): Observable<boolean> {
        return this.http.post(`/api/Account/Signout`, {})
            .map(result => {
                if (result.ok) {
                    this.setUserAndRedirect(null);
                }
                return result.ok;
            });
    }

    public login(userName: string, password: string): Observable<LoginResult> {
        return this.http
            .post(`/api/Account`, { userName, password })
            .map(result => {
                if (result.ok) {
                    var user = this.setUserAndRedirect(result.ok ? result.json() as User : null);
                    return { success: true, message: null, user: user };
                }
                else {
                    return { success: true, message: result.statusText, user: null };
                }
            }, () => {
                this.setUserAndRedirect(null); return { sucess: false, message: `Invalid credentials` }
            });
    }

    private setUserAndRedirect(user: User): User {
        this.user = user;
        this.router.navigateByUrl(user ? this.redirectUrl : this.loginUrl);
        return this.user;
    }

    constructor(private http: Http, private router: Router) {
        this.loginUrl = '/login';
        this.redirectUrl = '/entries';
        this.initUserFromServer();
    }
}

interface LoginResult {
    success: boolean,
    message: string,
    user: User
}

export interface User {
    id: number,
    firstName: string,
    lastName: string,
    email: string,
    canCrudUsers: boolean,
    canCrudAllEntries: boolean
}