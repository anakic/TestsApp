import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class UserService {

    public search(searchTerm: string) : Observable<UserData[]> {
        return this.http.get(`/api/users?searchTerm=${searchTerm}`).map(res => {
            if (res.ok)
                return res.json() as UserData[];
            else
                return null;
        });
    }

    public delete(userId: number): Observable<boolean> {
        return this.http.delete(`/api/users/${userId}`)
            .map(res =>{
                return res.ok;
            });
    }

    public update(user: UserData): Observable<boolean> {
        return this.http.put(`/api/users/${user.id}`, user)
            .map(res => {
                return res.ok;
            });
    }

    public create(user: UserData): Observable<boolean> {
        return this.http.post(`/api/users/`, user)
            .map(res => {
                return res.ok;
            });
    }

    constructor(private http: Http) { }
}

export enum UserRole {
    User,
    Manager,
    Admin
}

export interface UserData {
    id: number,
    email: string,
    firstName: string,
    lastName: string,
    roleName: string,
    role: number,
}