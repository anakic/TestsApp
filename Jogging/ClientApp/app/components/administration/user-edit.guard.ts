import { CanDeactivate } from "@angular/router";
import { Injectable } from "@angular/core";
import { DialogService } from "../../dialog.service";
import { AdministrationComponent } from "./administration.component";

@Injectable()
export class UserEditGuard implements CanDeactivate<AdministrationComponent> {

    constructor(private dialogService: DialogService) { }

    public canDeactivate(component: AdministrationComponent): boolean {
        return component.editingUser == null || this.dialogService.confirm("Cancel editing user?");
    };
}