import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { FormattingService } from '../../formatting.service';

@Component({
    templateUrl: './weekly-summary.component.html'
})
export class WeeklyReportComponent {

    private pleaseEnterFilterMsg: string = "Please enter filter range";
    private FilteringMsg: string = "Filtering, please wait...";

    //todo: change to Date
    public from: Date;
    public to: Date;
    public statusMessage;
    public entries: WeeklySummary[];

    public filter() {
        this.entries = null;
        this.statusMessage = "Filtering...";
        this.http.get(`/api/entries/weeklysummaries?from=${this.from.toISOString()}&to=${this.to.toISOString()}`).subscribe(result => {
            this.entries = result.json() as WeeklySummary[];
            this.statusMessage = "";
        });
    }

    constructor(private http: Http, private formattingService: FormattingService) {
        this.from = new Date();
        this.to = new Date();
        this.from.setDate(new Date().getDate() - 28);
        this.statusMessage = "Please enter filter range";
    }
}

interface WeeklySummary {
    year: Date;
    week: number;
    totalDistance: number;
    totalTime: number;
    averageSpeed: number;
}
