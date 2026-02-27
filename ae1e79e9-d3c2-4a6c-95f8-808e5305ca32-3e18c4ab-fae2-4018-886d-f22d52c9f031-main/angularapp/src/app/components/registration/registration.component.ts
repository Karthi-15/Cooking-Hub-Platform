import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {
  registrationForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';
  submitting: boolean = false;
  roles: string[] = ['Admin', 'User'];
  private hardcodedAdminKey = 'SECRET123';  // Hardcoded admin key

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registrationForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3),Validators.pattern(/^[A-Za-z]+$/)]],
      email: ['', [Validators.required, Validators.email]],
      mobileNumber: ['', 
      [Validators.required,
      Validators.pattern(/^[6789]\d{9}$/),
      Validators.minLength(10),
      Validators.maxLength(10)]
],
      password: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/)
      ]],
      confirmPassword: ['', Validators.required],
      userRole: ['User', Validators.required],
      adminKey: [''] // Added adminKey form control (validation on submit)
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate([this.authService.isAdmin() ? '/admin/home' : '/user/home']);
    }
  }

  // Custom validator to check if password and confirmPassword match
  passwordMatchValidator: ValidatorFn = (form: AbstractControl): ValidationErrors | null => {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  };

  isAdminRole(): boolean {
    return this.registrationForm.get('userRole')?.value === 'Admin';
  }

  onSubmit(event: Event): void {
    event.preventDefault();
    this.errorMessage = '';
    this.successMessage = '';

    // Mark all controls touched to show validation errors if any
    if (!this.registrationForm.valid) {
      this.markFormGroupTouched(this.registrationForm);
      return;
    }

    if (this.isAdminRole()) {
      const enteredKey = this.registrationForm.get('adminKey')?.value;
      if (!enteredKey) {
        this.errorMessage = 'Admin Key is required for Admin role.';
        this.registrationForm.get('adminKey')?.setErrors({ required: true });
        return;
      } else if (enteredKey !== this.hardcodedAdminKey) {
        this.errorMessage = 'Invalid Admin Key.';
        this.registrationForm.get('adminKey')?.setErrors({ incorrect: true });
        return;
      }
    }

    // If here, form is valid and admin key is correct if role is Admin
    this.submitting = true;

    const userModel: User = {
      userId: undefined,
      email: this.registrationForm.get('email')?.value,
      password: this.registrationForm.get('password')?.value,
      username: this.registrationForm.get('username')?.value,
      mobileNumber: this.registrationForm.get('mobileNumber')?.value,
      userRole: this.registrationForm.get('userRole')?.value
    };

    this.authService.register(userModel).subscribe(
      response => {
        this.successMessage = 'Registration successful!';
        setTimeout(() => this.router.navigate(['/login']), 2000);
        this.submitting = false;
      },
      error => {
        this.submitting = false;
        if (error.status === 400) {
          this.errorMessage = 'Invalid registration data';
        } else if (error.error && typeof error.error === 'string' && error.error.includes('already exists')) {
          this.errorMessage = 'User already exists';
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
      }
    );
  }

  // Helper to mark all controls as touched
  markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}
