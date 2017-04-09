import { Component } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { LoginService } from '../../login.service';
import { FormattingService } from '../../formatting.service';
import { DialogService } from '../../dialog.service';

@Component({
    styleUrls: ['./entries.component.css'],
    templateUrl: './entries.component.html'
})
export class EntriesComponent {
    //view data
    public entries: Entry[];
    public editingEntry: Entry;
    public message: string;
    public fetchStatus: EntryFetchStatus;

    //filter params
    public from: Date;
    public to: Date;

    public newEntry() {
        this.editingEntry = { id: null, date: new Date(), timeInSeconds: 0, distanceInMeters: 0, userId: this.loginService.user.id, averageSpeed: null };
    }

    public edit(entry: Entry) {
        //copy object
        this.editingEntry = { id: entry.id, date: entry.date, distanceInMeters: entry.distanceInMeters, timeInSeconds: entry.timeInSeconds, averageSpeed: entry.averageSpeed, userId: entry.userId };
    }

    public cancelEdit() {
        this.editingEntry = null;
    }

    public saveChanges() {

        if (this.editingEntry.timeInSeconds == null)
            this.dialogService.alert('Invalid time');
        else if (this.editingEntry.distanceInMeters == null)
            this.dialogService.alert('Invalid distance');
        else {
            let observableResult: Observable<Response>;
            if (this.editingEntry.id != null) {
                observableResult = this.http.put(`/api/Entries/${this.editingEntry.id}`, this.editingEntry);
            }
            else {
                observableResult = this.http.post(`/api/Entries`, this.editingEntry);
            }

            observableResult.subscribe(result => {
                this.editingEntry = null;
                this.filter();
            }, error => {
                this.message = `Oops, something went wrong. ${error._body}`;
            });
        }
    }

    public delete(entry: Entry) {
        if (this.dialogService.confirm('Are you sure?')) {
            this.http.delete(`/api/entries/${entry.id}`).subscribe(result => {
                if (result.ok) {
                    //Removing the item from the array manually to reduce traffic and keep scroll position.
                    //The alternative would be to just call filter().
                    this.entries = this.entries.filter(e => e.id != entry.id);
                }
                else {
                    this.message = `Oops, something went wrong... Status: ${result.status}, text: ${result.statusText}`;
                }
            }, err => this.message = `Oops, something went wrong... ` + err);
        }
    }

    public filter() {
        this.entries = null;
        this.fetchStatus = EntryFetchStatus.Working;
        this.http.get(`/api/entries?from=${this.from.toISOString().substring(0, 11)}00:00:00&to=${this.to.toISOString().substring(0, 11)}23:59:59`).subscribe(result => {
            this.entries = result.json() as Entry[];
            this.fetchStatus = EntryFetchStatus.Completed;
        }, () => {
            this.message = "Error fetching data";
            this.fetchStatus = EntryFetchStatus.Completed;
        });
    }

    public parseDate(str: string): Date {
        return new Date(str);
    }

    constructor(private http: Http, private loginService: LoginService, private formattingService: FormattingService, private dialogService: DialogService) {
        this.from = new Date();
        this.to = new Date();
        this.from.setDate(new Date().getDate() - 28);
        this.fetchStatus = EntryFetchStatus.Initial;
    }
}

enum EntryFetchStatus {
    Initial,
    Working,
    Completed
}

interface Entry {
    id: number,
    date: Date,
    distanceInMeters: number,
    timeInSeconds: number,
    averageSpeed: number,
    userId: number
}
