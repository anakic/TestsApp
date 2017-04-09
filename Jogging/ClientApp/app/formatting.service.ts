import { Injectable } from '@angular/core';

@Injectable()
export class FormattingService {
        
    //note: i18n (would likely need to be user configurable)

    public formatDistanceForDisplay(distanceInMeters: number): string {
        return `${(distanceInMeters / 1000).toFixed(2)}km`;
    }
    public formatDistanceForEditing(distanceInMeters: number): string {
        return `${(distanceInMeters/1000).toFixed(2)}`;
    }

    public parseDistanceFromEditing(input: string) {
        return (parseFloat(input) * 1000);
    }

    public formatDateForDisplay(date: Date): string {
        date = new Date(date);//coerce type
        return `${date.toDateString()}`;
    }
    public formatDateForEditing(date: Date): string {
        date = new Date(date);//coerce type
        return `${date.getFullYear()}-${("0" + (date.getMonth()+1)).slice(-2)}-${("0" + date.getDate()).slice(-2)}`;
    }
    public parseDateFromEditing(input: string) {
        return new Date(input);
    }

    public formatTimeForDisplay(timeInSeconds: number): string {
        var seconds = timeInSeconds % 60;
        var minutes = Math.floor(timeInSeconds / 60 % 60);
        var hours = Math.floor(timeInSeconds / 3600);
        return `${("0" + hours).slice(-2)}:${("0" + minutes).slice(-2)}:${("0" + seconds).slice(-2)}`
    }

    public formatTimeForEditing(timeInSeconds: number): string {
        return this.formatTimeForDisplay(timeInSeconds);
    }
    public parseTimeFromEditing(input: string) {
        var segments = input.split(":");
        if (segments.length != 3)
            throw new Error("Invalid time string!");

        var hours = parseInt(segments[0]);
        var minutes = parseInt(segments[1]);
        var seconds = parseInt(segments[2]);

        return (hours * 60 + minutes) * 60 + seconds;
    }

    public formatSpeedForDisplay(speedInMetersPerSecond: number): string {
        return `${(speedInMetersPerSecond * 3.6).toFixed(2)}km/h`;
    }
}