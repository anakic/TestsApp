import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { LoginData } from '../../login.service';
import { DialogService } from '../../dialog.service';
import { UserData, UserRole, UserService } from '../../users.service';

@Component({
    templateUrl: './administration.component.html'
})
export class AdministrationComponent {

    public message: string;
    public isFetching: boolean;

    public updatePassword: boolean;

    public searchTerm: string;

    public editingUser: UserData;
    public users: UserData[];

    constructor(private http: Http, private userService: UserService, private dialogService: DialogService) {
        this.isFetching = false;
        this.searchTerm = "";
    }

    public roleIdToName(roleId: number): string {
        return UserRole[roleId];
    }

    public filter() {
        this.users = null;
        this.message = null;
        this.userService.search(this.searchTerm).subscribe(res => {
            this.users = res;
        }, res => {
            this.isFetching = false;
            this.message = res._body;
        });
    }

    public deleteUser(user: UserData) {
        if (this.dialogService.confirm('Are you sure?')) {
            this.message = null;
            this.userService.delete(user.id).subscribe(res => {
                if (res)
                    this.filter();
                else
                    this.message = 'Oops, error deleting user.';
            }, res => {
                this.message = res._body;
            });
        }
    }

    public saveChanges() {

        if (!this.validateEmail(this.editingUser.email)) {
            this.message = "Invalid email address";
        } else {
            var observable;
            this.message = null;
            if (this.editingUser.id == null) {
                observable = this.userService.create(this.editingUser);
            } else {
                observable = this.userService.update(this.editingUser, this.updatePassword);
            }

            observable.subscribe(res => {
                if (res) {
                    this.filter();
                    this.editingUser = null;
                } else {
                    this.message = 'Failed to save user data!';
                }
            }, res => {
                this.message = res._body;
            });
        }
    }

    public editUser(user: UserData) {
        this.message = null;
        this.updatePassword = false;
        this.editingUser = { id: user.id, email: user.email, firstName: user.firstName, lastName: user.lastName, role: user.role, roleName: '', newPassword: "" };
    }

    public newUser() {
        this.message = null;
        this.updatePassword = false;
        this.editingUser = { id: null, email: "", firstName: "", lastName: "", role: 0, roleName: "", newPassword: "" };
    }

    public cancelEdit() {
        this.message = null;
        this.editingUser = null;
    }

    private validateEmail(email): boolean {
        var expression = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return expression.test(email);
    }
}
