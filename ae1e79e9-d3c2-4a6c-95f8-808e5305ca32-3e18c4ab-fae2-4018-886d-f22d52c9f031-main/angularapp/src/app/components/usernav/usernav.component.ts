
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-usernav',
  templateUrl: './usernav.component.html',
  styleUrls: ['./usernav.component.css']
})
export class UsernavComponent implements OnInit {
  isDropdownHidden: boolean = true;
  showLogoutModal: boolean = false;
  showUserDetailsModal: boolean = false;

  username: string = '';
  userRole: string = '';
  userEmail: string = '';
  userMobile: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    const userInfo = this.authService.getUserInfo();
    this.username = userInfo.username;
    this.userRole = this.authService.getUserRole();
    this.userEmail = userInfo.email;
    this.userMobile = userInfo.mobileNumber;
  }

  toggleDropdown(): void {
    this.isDropdownHidden = !this.isDropdownHidden;
  }

  confirmLogout(): void {
    this.showLogoutModal = true;
  }

  closeLogoutModal(): void {
    this.showLogoutModal = false;
  }

  logout(): void {
    this.authService.logout();
    this.closeLogoutModal();
    this.router.navigate(['/home']);
  }
}
