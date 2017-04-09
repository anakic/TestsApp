import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { UniversalModule } from 'angular2-universal';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './components/app/app.component'
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { EntriesComponent } from './components/entries/entries.component';
import { LoginService } from './login.service';
import { LoginComponent } from './components/login/login.component';
import { AdministrationComponent } from './components/administration/administration.component';
import { LoginSignoutComponent } from './components/login/login-signout.component';
import { WeeklyReportComponent } from './components/weekly-summary/weekly-summary.component';
import { IsLoggedInGuard, IsUserAdminGuard } from './auth.guard';
import { EntryEditGuard } from './components/entries/entry-edit.guard';
import { UserEditGuard } from './components/administration/user-edit.guard';
import { FormattingService } from './formatting.service';
import { DialogService } from './dialog.service';
import { UserService } from './users.service';



@NgModule({
    bootstrap: [AppComponent],
    declarations: [
        AppComponent,
        NavMenuComponent,
        EntriesComponent,
        LoginComponent,
        LoginSignoutComponent,
        AdministrationComponent,
        WeeklyReportComponent
    ],
    providers: [LoginService, IsLoggedInGuard, IsUserAdminGuard, FormattingService, UserService, DialogService, EntryEditGuard, UserEditGuard],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component: LoginComponent },
            { path: 'entries', component: EntriesComponent, canActivate: [IsLoggedInGuard], canDeactivate: [EntryEditGuard] },
            { path: 'weekly-report', component: WeeklyReportComponent, canActivate: [IsLoggedInGuard] },
            { path: 'administration', component: AdministrationComponent, canActivate: [IsUserAdminGuard], canDeactivate: [UserEditGuard] },
            { path: '**', redirectTo: 'entries' }
        ]),
        FormsModule
    ]
})
export class AppModule {
}
