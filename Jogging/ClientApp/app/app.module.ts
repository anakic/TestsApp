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
import { IsLoggedInGuard, IsUserAdminGuard } from './auth.guard';

@NgModule({
    bootstrap: [AppComponent],
    declarations: [
        AppComponent,
        NavMenuComponent,
        EntriesComponent,
        LoginComponent,
        LoginSignoutComponent,
        AdministrationComponent
    ],
    providers: [LoginService, IsLoggedInGuard, IsUserAdminGuard],
    imports: [
        UniversalModule, // Must be first import. This automatically imports BrowserModule, HttpModule, and JsonpModule too.
        RouterModule.forRoot([
            { path: '', redirectTo: 'entries', pathMatch: 'full' },
            { path: 'entries', component: EntriesComponent, canActivate: [IsLoggedInGuard] },
            { path: 'login', component: LoginComponent },
            { path: 'administration', component: AdministrationComponent, canActivate: [IsUserAdminGuard] },
            { path: '**', redirectTo: 'entries' }
        ]),
        FormsModule
    ]
})
export class AppModule {
}
