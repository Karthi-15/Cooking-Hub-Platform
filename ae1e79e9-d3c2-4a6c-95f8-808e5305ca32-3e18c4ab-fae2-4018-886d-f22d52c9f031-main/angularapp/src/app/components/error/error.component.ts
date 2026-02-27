import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-error',
  templateUrl: './error.component.html',
  styleUrls: ['./error.component.css']
})

export class ErrorComponent implements OnInit {
  
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;

  constructor(
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.isLoggedIn = this.authService.isLoggedIn();
    this.isAdmin = this.authService.isAdmin();
  }

  navigateToHome(): void {

    if (this.isLoggedIn) {
      if (this.isAdmin) {
        this.router.navigate(['/admin/home']);
      } 
      else {
        this.router.navigate(['/user/home']);
      }
    } 
    else {
      this.router.navigate(['/login']);
    }
  }
}







































































































































































































































