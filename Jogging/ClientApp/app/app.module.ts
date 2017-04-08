import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { EntriesComponent } from './components/entries/entries.component';
import { LoginService } from './login.service';
import { LoginComponent } from './components/login/login.component';
import { LoginSignoutComponent } from './components/login/login-signout.component';

@NgModule({
    bootstrap: [ AppComponent ],
    declarations: [
        AppComponent,
        NavMenuComponent,
        EntriesComponent,
        LoginComponent,
        LoginSignoutComponent
    ],
    providers: [LoginService],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'entries', component: EntriesComponent },
            { path: 'login', component: LoginComponent },
            { path: '**', redirectTo: 'entries' }
        ]),
        FormsModule
    ]
})
export class AppModule {
}
