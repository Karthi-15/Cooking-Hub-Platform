import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { Login } from '../../models/login.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  showPassword: boolean = false;
  loginForm: FormGroup;
  errorMessage: string = '';
  submitting: boolean = false;

  captchaQuestion: string = '';
  captchaAnswer: number = 0;
  captchaInvalid: boolean = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) 
  {
    this.loginForm = this.fb.group({
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  generateCaptcha() {
    const a = Math.floor(Math.random() * 10);
    const b = Math.floor(Math.random() * 10);
    this.captchaAnswer = a + b;
    this.captchaQuestion = `${a} + ${b}`;
  }

  ngOnInit(): void {

    if (this.authService.isLoggedIn()) {
      if (this.authService.isAdmin()) {
        this.router.navigate(['/admin/home']);
      } else {
        this.router.navigate(['/user/home']);
      }
    }
    this.loginForm.addControl('captchaAnswer', this.fb.control('', Validators.required));
 
    // Generate captcha question
    this.generateCaptcha();
  }



  onSubmit(event:Event): void {
    event.preventDefault();
    const userAnswer = parseInt(this.loginForm.get('captchaAnswer')?.value, 10);
 
    if (userAnswer !== this.captchaAnswer) {
      this.captchaInvalid = true;
      return;
    }
    
    this.captchaInvalid = false;

    if (this.loginForm.valid) {

      this.submitting = true;
      this.errorMessage = '';
      const loginModel: Login = this.loginForm.value;
      this.authService.login(loginModel).subscribe(
        response => {
          this.submitting = false;

          Swal.fire({
            icon: 'success',
            title: 'Login Successful',
            text: 'You have been logged in successfully!',
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true,
            toast: false,
            position: 'center',
            showClass: {
              popup: 'animate__animated animate__fadeInDown'
            },

            hideClass: {
              popup: 'animate__animated animate__fadeOutUp'
            }

          });

          if (this.authService.isAdmin()) {
            this.router.navigate(['/admin/home']);
          } 
          else {
            this.router.navigate(['/user/home']);
          }
        },

        error => {
          this.submitting = false;
          if (error.status === 401) {
            this.errorMessage = error.error;
          } 
          else {
            this.errorMessage = 'An error occurred. Please try again later.';
          }
        }
      );
    } 
    else {
      this.markFormGroupTouched(this.loginForm);
    }
  }
  markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}
