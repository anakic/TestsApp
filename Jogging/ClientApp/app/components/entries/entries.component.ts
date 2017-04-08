import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'fetchdata',
    templateUrl: './entries.component.html'
})
export class EntriesComponent {
    //view data
    public entries: Entry[];
    public message: string;
    public fetchStatus: EntryFetchStatus;

    //filter params
    public from: Date;
    public to: Date;

    public filter() {
        this.entries = null;
        this.fetchStatus = EntryFetchStatus.Working;
        this.http.get(`/api/entries?from=${this.from.toISOString()}&to=${this.to.toISOString()}`).subscribe(result => {
            this.entries = result.json() as Entry[];
            this.fetchStatus = EntryFetchStatus.Completed;
        }, () => {
            this.message = "Error fetching data";
            this.fetchStatus = EntryFetchStatus.Completed;
            });
    }

    constructor(private http: Http) {
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
}
