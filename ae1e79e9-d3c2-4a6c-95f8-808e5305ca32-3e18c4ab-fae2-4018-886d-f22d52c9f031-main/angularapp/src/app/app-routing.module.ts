import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Components
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { CookingImpComponent } from './components/cooking-imp/cooking-imp.component';
import { ErrorComponent } from './components/error/error.component';

// Admin Components
import { AdminviewclassComponent } from './components/adminviewclass/adminviewclass.component';
import { AdminaddclassComponent } from './components/adminaddclass/adminaddclass.component';
import { AdmineditclassComponent } from './components/admineditclass/admineditclass.component';
import { AdminviewappliedrequestComponent } from './components/adminviewappliedrequest/adminviewappliedrequest.component';
import { AdminviewfeedbackComponent } from './components/adminviewfeedback/adminviewfeedback.component';

// User Components
import { UserviewclassComponent } from './components/userviewclass/userviewclass.component';
import { UseraddrequestComponent } from './components/useraddrequest/useraddrequest.component';
import { UserviewappliedrequestComponent } from './components/userviewappliedrequest/userviewappliedrequest.component';
import { UseraddfeedbackComponent } from './components/useraddfeedback/useraddfeedback.component';
import { UserviewfeedbackComponent } from './components/userviewfeedback/userviewfeedback.component';

// Guards
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';
import { UserGuard } from './guards/user.guard';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'cooking-imp', component: CookingImpComponent, canActivate: [AuthGuard] },

  // Admin routes
  {
    path: 'admin',
    canActivate: [AdminGuard],
    children: [
      { path: 'home', component: HomeComponent },
      { path: 'add-class', component: AdminaddclassComponent },
      { path: 'view-class', component: AdminviewclassComponent },
      { path: 'edit-class/:id', component: AdmineditclassComponent },
      { path: 'view-requests', component: AdminviewappliedrequestComponent },
      { path: 'view-feedback', component: AdminviewfeedbackComponent }
    ]
  },

  // User routes
  {
    path: 'user',
    canActivate: [UserGuard],
    children: [
      { path: 'home', component: HomeComponent },
      { path: 'view-classes', component: UserviewclassComponent },
      { path: 'add-request', component: UseraddrequestComponent },
      { path: 'applied-requests', component: UserviewappliedrequestComponent },
      { path: 'add-feedback', component: UseraddfeedbackComponent },
      { path: 'view-feedback', component: UserviewfeedbackComponent }
    ]
  },

  // Error route
  { path: 'error', component: ErrorComponent },
  { path: '**', redirectTo: '/error' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }