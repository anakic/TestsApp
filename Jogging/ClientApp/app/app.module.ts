import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { EntriesComponent } from './components/entries/entries.component';
import { LoginService } from './login.service';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        EntriesComponent,
    ],
    providers: [LoginService],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'entries', pathMatch: 'full' },
            { path: 'entries', component: EntriesComponent },
            { path: '**', redirectTo: 'entries' }
        ]),
        FormsModule
    ]
})
export class AppModule {
}
