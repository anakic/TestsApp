import { CanDeactivate } from "@angular/router";
import { Injectable } from "@angular/core";
import { DialogService } from "../../dialog.service";
import { EntriesComponent } from "./entries.component";

@Injectable()
export class EntryEditGuard implements CanDeactivate<EntriesComponent> {

    constructor(private dialogService: DialogService) { }

    public canDeactivate(component: EntriesComponent): boolean {
        return component.editingEntry == null || this.dialogService.confirm("Cancel editing entry?");
    };
}