import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClass } from '../../models/cooking-class.model';
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
  selector: 'app-admineditclass',
  templateUrl: './admineditclass.component.html',
  styleUrls: ['./admineditclass.component.css']
})
export class AdmineditclassComponent implements OnInit {
  classForm: FormGroup;
  classId: number;
  errorMessage: string = '';
  showSuccessModal: boolean = false;
  loading: boolean = true;
  submitting: boolean = false;
  skillLevels: string[] = ['Beginner', 'Intermediate', 'Advanced'];



  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private cookingClassService: CookingClassService
  ) 
  
  {
    this.classForm = this.fb.group({
      className: ['', [Validators.required, noWhitespaceValidator, Validators.maxLength(80)]],
      cuisineType: ['', [Validators.required, noWhitespaceValidator, Validators.pattern(/^[A-Za-z ]+$/)]],
      chefName: ['', [Validators.required, noWhitespaceValidator, Validators.maxLength(50)]],
      location: ['', [Validators.required, noWhitespaceValidator, Validators.maxLength(100)]],
      durationInHours: ['', [Validators.required, Validators.min(1), Validators.max(12)]],
      fee: ['', [Validators.required, Validators.min(1), Validators.max(10000)]],
      ingredientsProvided: ['', [Validators.required, noWhitespaceValidator, Validators.maxLength(200)]],
      skillLevel: ['', Validators.required],
      specialRequirements: ['', [Validators.required, noWhitespaceValidator, Validators.maxLength(300)]]
    });
    
    this.classId = +this.route.snapshot.paramMap.get('id');
  }


  ngOnInit(): void {
    this.loadCookingClass();
  }

  loadCookingClass(): void {
    if (!this.classId) {
      this.router.navigate(['/admin/view-class']);
      return;
    }


    this.cookingClassService.getCookingClassById(this.classId).subscribe(
    (cookingClass: CookingClass) => {
      console.log('Fetched Cooking Class:', cookingClass); // Debugging

      // Ensure response contains all required fields before patching
      if (cookingClass) {
        this.classForm.patchValue({
          className: cookingClass.className || '',
          cuisineType: cookingClass.cuisineType || '',
          chefName: cookingClass.chefName || '',
          location: cookingClass.location || '',
          durationInHours: cookingClass.durationInHours || '',
          fee: cookingClass.fee || '',
          ingredientsProvided: cookingClass.ingredientsProvided || '',
          skillLevel: cookingClass.skillLevel || '',
          specialRequirements: cookingClass.specialRequirements || ''
        });
      }

      this.loading = false;
    },
    error => {
      console.error('Error fetching cooking class:', error);
      this.loading = false;
    }
  );
}
  

  onSubmit(event:Event): void {
    event.preventDefault();
    if (this.classForm.valid) {
      this.submitting = true;
      this.errorMessage = '';
      
      const cookingClass: CookingClass = this.classForm.value;
      
      this.cookingClassService.updateCookingClass(this.classId, cookingClass).subscribe(
        () => {
          this.submitting = false;
          this.showSuccessModal = true;
        },

        error => {
          this.submitting = false;
          
          if (error.error && typeof error.error === 'string' && error.error.includes('already exists')) {
            this.errorMessage = 'Cooking class with the same name already exists';
          }
          else {
            this.errorMessage = 'Failed to update cooking class. Please try again.';
          }
        }
      );
    } else {
      this.markFormGroupTouched(this.classForm);
      this.errorMessage = 'All fields are required';
    }
  }

  closeSuccessModal(): void {
    this.showSuccessModal = false;
    this.router.navigate(['/admin/view-class']);
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











































































