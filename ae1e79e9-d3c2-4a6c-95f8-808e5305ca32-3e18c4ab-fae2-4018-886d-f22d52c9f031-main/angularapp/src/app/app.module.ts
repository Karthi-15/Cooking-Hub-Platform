import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Components
import { NavbarComponent } from './components/navbar/navbar.component';
import { FooterComponent } from './components/footer/footer.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { CookingImpComponent } from './components/cooking-imp/cooking-imp.component';
import { ErrorComponent } from './components/error/error.component';

// Admin Components
import { AdminnavComponent } from './components/adminnav/adminnav.component';
import { AdminviewclassComponent } from './components/adminviewclass/adminviewclass.component';
import { AdminaddclassComponent } from './components/adminaddclass/adminaddclass.component';
import { AdmineditclassComponent } from './components/admineditclass/admineditclass.component';
import { AdminviewappliedrequestComponent } from './components/adminviewappliedrequest/adminviewappliedrequest.component';
import { AdminviewfeedbackComponent } from './components/adminviewfeedback/adminviewfeedback.component';

// User Components
import { UsernavComponent } from './components/usernav/usernav.component';
import { UserviewclassComponent } from './components/userviewclass/userviewclass.component';
import { UseraddrequestComponent } from './components/useraddrequest/useraddrequest.component';
import { UserviewappliedrequestComponent } from './components/userviewappliedrequest/userviewappliedrequest.component';
import { UseraddfeedbackComponent } from './components/useraddfeedback/useraddfeedback.component';
import { UserviewfeedbackComponent } from './components/userviewfeedback/userviewfeedback.component';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    NavbarComponent,
    AppComponent,
    HomeComponent,
    LoginComponent,
    RegistrationComponent,
    CookingImpComponent,
    ErrorComponent,
    AdminnavComponent,
    AdminviewclassComponent,
    AdminaddclassComponent,
    AdmineditclassComponent,
    AdminviewappliedrequestComponent,
    AdminviewfeedbackComponent,
    UsernavComponent,
    UserviewclassComponent,
    UseraddrequestComponent,
    UserviewappliedrequestComponent,
    UseraddfeedbackComponent,
    UserviewfeedbackComponent,
    FooterComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }