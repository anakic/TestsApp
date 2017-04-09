import { Injectable } from '@angular/core';

@Injectable()
export class DialogService {
    public alert(message: string) {
        alert(message);
    }

    public confirm(message: string) : boolean {
        return confirm(message);
    }
}