import { BrowserModule } from '@angular/platform-browser';
import localeDe from '@angular/common/locales/de';
import { NgModule,LOCALE_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HeaderComponent } from './header/header.component';
import { MainSectionComponent } from './main-section/main-section.component';
import { PersonalLoanComponent } from './personal-loan/personal-loan.component';
import { TermDepositComponent } from './term-deposit/term-deposit.component';
import { MatSelectModule } from '@angular/material/select';
import { AboutComponent } from './about/about.component';
import { registerLocaleData } from '@angular/common';


registerLocaleData(localeDe);

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HeaderComponent,
    MainSectionComponent,
    PersonalLoanComponent,
    TermDepositComponent,
    AboutComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    MatSelectModule
  ],
  providers: [{provide:LOCALE_ID,useValue:'de-DE'}],
  bootstrap: [AppComponent]
})
export class AppModule { }
