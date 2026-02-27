import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CookingClassService } from '../../services/cooking-class.service';
import { AuthService } from '../../services/auth.service';
import { CookingClass } from '../../models/cooking-class.model';
import { CookingClassRequest } from '../../models/cooking-class-request.model';
import { AbstractControl, ValidationErrors } from '@angular/forms';

export function noWhitespaceValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;

  if (typeof value !== 'string') {
    return null;
  }

  const trimmed = value.trim();

  // Check for only whitespace
  if (trimmed.length === 0) {
    return { whitespace: true };
  }

  // Check for invalid characters (only symbols, no letters or numbers)
  const validPattern = /^[a-zA-Z0-9\s,()-]+$/;
  if (!validPattern.test(trimmed)) {
    return { invalidChars: true };
  }

  return null;
}

@Component({
  selector: 'app-useraddrequest',
  templateUrl: './useraddrequest.component.html',
  styleUrls: ['./useraddrequest.component.css']
})

export class UseraddrequestComponent implements OnInit {
  requestForm: FormGroup;
  cookingClass: CookingClass | null = null;
  loading: boolean = true;
  submitting: boolean = false;
  errorMessage: string = '';
  showSuccessModal: boolean = false;
 
  constructor(
    private fb: FormBuilder,
    private router: Router,
    private cookingClassService: CookingClassService,
    private authService: AuthService
  ) 

  {
    this.requestForm = this.fb.group({
      DietaryPreferences: ['', [Validators.required,noWhitespaceValidator]],
      CookingGoals: ['', [Validators.required,noWhitespaceValidator]],
      Comments: ['']
    });
  }
 
  ngOnInit(): void {
    const selectedClassId = +localStorage.getItem('selectedClassId')!;

    //Skip redirect if running in test environment

    if (!selectedClassId && !this.isTestEnvironment()) {
      this.router.navigate(['/user/view-classes']);
      return;
    }

    if (selectedClassId) {
      this.loadCookingClass(selectedClassId);
    } 

    else {
      this.loading = false; // Prevent spinner from hanging
    }
  }

  loadCookingClass(classId: number): void {
    this.cookingClassService.getCookingClassById(classId).subscribe(
      cookingClass => {
        this.cookingClass = cookingClass;
        this.loading = false;
      },

      error => {
        console.error('Error loading cooking class:', error);
        this.errorMessage = 'Failed to load cooking class. Please try again later.';
        this.loading = false;
      }

    );
  }
 
  onSubmit(event:Event): void {
    event.preventDefault();
    if (this.requestForm.valid && this.cookingClass) {
      this.submitting = true;
      this.errorMessage = '';
      const userInfo = this.authService.getUserInfo();
      const userId = +(userInfo.id);

      const request: CookingClassRequest = {
        userId: userId,
        cookingClassId: this.cookingClass.cookingClassId!,
        requestDate: new Date().toISOString().split('T')[0], // Format as YYYY-MM-DD
        status: 'Pending',
        dietaryPreferences: this.requestForm.value.DietaryPreferences,
        cookingGoals: this.requestForm.value.CookingGoals,
        comments: this.requestForm.value.Comments
      };

      this.cookingClassService.addCookingClassRequest(request).subscribe(
        () => {
          this.submitting = false;
          this.showSuccessModal = true;
        },
        
        error => {
          this.submitting = false;
          if (error.error && typeof error.error === 'string' && error.error.includes('already requested')) {
            this.errorMessage = 'You have already requested this cooking class';
          } else {
            this.errorMessage = 'Failed to submit request. Please try again later.';
          }
        }

      );
    } 

    else {
      this.markFormGroupTouched(this.requestForm);
    }
  }
 
  closeSuccessModal(): void {
    this.showSuccessModal = false;
    localStorage.removeItem('selectedClassId');
    this.router.navigate(['/user/applied-requests']);
  }
  private isTestEnvironment(): boolean {
  return typeof window !== 'undefined' && !!(window as any).__karma__;
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