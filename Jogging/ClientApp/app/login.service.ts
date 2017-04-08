import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Rx'
import 'rxjs/add/operator/map';

@Injectable()
export class LoginService {

    public loginUrl: string;
    public redirectUrl: string;

    public user: LoginData;

    public initialize(): Observable<LoginData> {
        return this.http
            .get(`/api/login`, { withCredentials: true })
            .map(result => {
                return this.setUserAndRedirect(result.ok ? result.json() as LoginData : null);
            });
    }

    public signout(): Observable<boolean> {
        return this.http.delete(`/api/login`)
            .map(result => {
                if (result.ok) {
                    this.setUserAndRedirect(null);
                }
                return result.ok;
            });
    }

    public login(email: string, password: string): Observable<LoginResult> {
        return this.http
            .post(`/api/login`, { email, password })
            .map(result => {
                if (result.ok) {
                    var user = this.setUserAndRedirect(result.ok ? result.json() as LoginData : null);
                    return { success: true, message: null, user: user };
                }
                else {
                    return { success: true, message: result.statusText, user: null };
                }
            }, () => {
                this.setUserAndRedirect(null); return { sucess: false, message: `Invalid credentials` }
            });
    }

    private setUserAndRedirect(user: LoginData): LoginData {
        this.user = user;
        this.router.navigateByUrl(user ? this.redirectUrl : this.loginUrl);
        return this.user;
    }

    constructor(private http: Http, private router: Router) {
        this.loginUrl = '/login';
        this.redirectUrl = '/entries';
    }
}

interface LoginResult {
    success: boolean,
    message: string,
    user: LoginData
}

export interface LoginData {
    id: number,
    firstName: string,
    lastName: string,
    email: string,
    canCrudUsers: boolean,
    canCrudAllEntries: boolean
}