import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClass } from '../../models/cooking-class.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-adminviewclass',
  templateUrl: './adminviewclass.component.html',
  styleUrls: ['./adminviewclass.component.css']
})
export class AdminviewclassComponent implements OnInit {
  cookingClasses: CookingClass[] = [];
  filteredClasses: CookingClass[] = [];
  searchTerm: string = '';
  selectedClass: CookingClass | null = null;
  loading: boolean = true;
  error: string = '';
  currentPage: number = 1;
  itemsPerPage: number = 10;

  constructor(
    private cookingClassService: CookingClassService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCookingClasses();
  }

  loadCookingClasses(): void {
    this.loading = true;
    this.cookingClassService.getAllCookingClasses().subscribe(
      classes => {
        this.cookingClasses = classes;
        this.filteredClasses = classes;
        this.loading = false;
      },
      error => {
        console.error('Error loading cooking classes:', error);
        this.error = 'Failed to load cooking classes. Please try again later.';
        this.loading = false;
      }
    );
  }

  search(): void {
    if (!this.searchTerm) {
      this.filteredClasses = this.cookingClasses;
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredClasses = this.cookingClasses.filter(
      classItem => classItem.className.toLowerCase().includes(term)
    );
  }

  editClass(classId: number): void {
    this.router.navigate([`/admin/edit-class/${classId}`]);
  }

  confirmDelete(cookingClass: CookingClass): void {
    Swal.fire({
      title: 'Are you sure?',
      text: `Do you really want to delete "${cookingClass.className}"? This action cannot be undone.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (result.isConfirmed) {
        this.selectedClass = cookingClass;
        this.deleteClass();
      }
    });
  }

  deleteClass(): void {
    if (!this.selectedClass || !this.selectedClass.cookingClassId) {
      return;
    }

    const classId = this.selectedClass.cookingClassId;

    this.cookingClassService.deleteCookingClass(classId).subscribe(
      () => {
        this.loadCookingClasses();
        this.selectedClass = null;

        Swal.fire({
          icon: 'success',
          title: 'Deleted!',
          text: 'Cooking class has been successfully deleted.',
          timer: 2000,
          showConfirmButton: false
        });
      },
      error => {
        console.error('Error deleting cooking class:', error);
        this.selectedClass = null;

        if (error.error && typeof error.error === 'string' && error.error.includes('referenced in a request')) {
          Swal.fire({
            icon: 'error',
            title: 'Cannot Delete',
            text: 'This class is referenced in a request and cannot be deleted.',
          });
        } else {
          Swal.fire({
            icon: 'error',
            title: 'Failed',
            text: 'Failed to delete cooking class. Please try again.',
          });
        }
      }
    );
  }

  get totalPages(): number {
    return Math.ceil(this.filteredClasses.length / this.itemsPerPage);
  }

  paginatedClasses(): CookingClass[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredClasses.slice(start, start + this.itemsPerPage);
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  getPageArray(): number[] {
    return Array(this.totalPages).fill(0).map((_, i) => i + 1);
  }
  selectedProfile: any = null;
showProfileModal: boolean = false;

viewProfile(cookingClass: any): void {
  this.selectedProfile = cookingClass;
  this.showProfileModal = true;
}

closeProfileModal(): void {
  this.showProfileModal = false;
  this.selectedProfile = null;
}

}
