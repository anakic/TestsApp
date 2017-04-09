import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { LoginData } from '../../login.service';

@Component({
    templateUrl: './administration.component.html'
})
export class AdministrationComponent {

    public message: string;
    public isFetching: boolean;

    public searchTerm: string;

    public editingUser: UserData;
    public users: UserData[];

    constructor(private http: Http) {
        this.isFetching = false;
        this.searchTerm = "";
    }

    public roleIdToName(roleId: number): string {
        return UserRole[roleId];
    }

    public filter() {
        this.users = null;
        this.message = null;
        this.http.get(`/api/users?searchTerm=${this.searchTerm}`).subscribe(res => {
            if (res.ok)
                this.users = res.json() as UserData[];
            else
                this.message = 'Error fetching users: ' + res;

            this.isFetching = false;
        }, res => {
            this.isFetching = false;
            this.message = res._body;
        });
    }

    public deleteUser(user: UserData) {
        this.message = null;
        this.http.delete(`/api/users`, user).subscribe(res => {
            if (res.ok)
                this.filter();
            else
                this.message = 'Oops, error deleting users. Message: ' + res;
        }, res => {
            this.message = 'Oops, error deleting users. Message: ' + res._body;
        });
    }

    public saveChanges() {

        if (!this.validateEmail(this.editingUser.email)) {
            this.message = "Invalid email address";
        }
        else {
            var observable;
            this.message = null;
            if (this.editingUser.id == null)
                observable = this.http.post(`/api/users`, this.editingUser);
            else
                observable = this.http.put(`/api/users/${this.editingUser.id}`, this.editingUser);

            observable.subscribe(res => {
                if (res.ok) {
                    this.filter();
                    this.editingUser = null;
                } else
                    this.message = res;
            }, res => {
                this.message = res._body;
            });
        }
    }

    public editUser(user: UserData) {
        this.message = null;
        this.editingUser = { id: user.id, email: user.email, firstName: user.firstName, lastName: user.lastName, role: user.role, roleName: "" };
    }

    public newUser() {
        this.message = null;
        this.editingUser = { id: null, email: "", firstName: "", lastName: "", role: 0, roleName: "" };
    }

    public cancelEdit() {
        this.message = null;
        this.editingUser = null;
    }

    private validateEmail(email) : boolean {
        var expression = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return expression.test(email);
    }
}

enum UserRole {
    User,
    Manager,
    Admin
}

interface UserData {
    id: number,
    email: string,
    firstName: string,
    lastName: string,
    roleName: string,
    role: number,
}