import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { LoginService } from "./login.service";

@Injectable()
export class IsLoggedInGuard implements CanActivate {

    constructor(private loginService: LoginService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

        var isLoggedIn = this.loginService.user != null;

        if (!isLoggedIn) {
            this.loginService.redirectUrl = state.url;
            this.router.navigate(['login']);
        }

        return isLoggedIn;
    };
}

@Injectable()
export class IsUserAdminGuard implements CanActivate {

    constructor(private loginService: LoginService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (!this.loginService.user || !this.loginService.user.canCrudUsers) {
            this.router.navigate(['entries']);
        }
        return this.loginService.user && this.loginService.user.canCrudUsers;
    };
}