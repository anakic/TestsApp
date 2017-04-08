import { Injectable } from '@angular/core';

@Injectable()
export class LoginService {
    public user: User;

    constructor() {
        //temp:
        this.user = { id: 1, firstName: 'Test', lastName: 'User', email: 'test@user.com' }
    }
}

export interface User {
    id: number,
    firstName: string,
    lastName: string,
    email: string,
}