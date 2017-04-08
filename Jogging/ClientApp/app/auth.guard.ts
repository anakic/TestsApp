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
            this.router.navigateByUrl(this.loginService.loginUrl);
        }

        return isLoggedIn;
    };
}

@Injectable()
export class IsUserAdminGuard implements CanActivate {

    constructor(private loginService: LoginService) { }

    canActivate(): boolean {

        if (!this.loginService.user.canCrudUsers)
            alert('User does not have permission to view this resource');

        return  this.loginService.user.canCrudUsers;
    };
}