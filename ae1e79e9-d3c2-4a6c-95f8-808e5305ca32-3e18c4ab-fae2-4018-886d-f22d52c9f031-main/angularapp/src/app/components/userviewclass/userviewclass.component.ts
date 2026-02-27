import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClass } from '../../models/cooking-class.model';

@Component({
  selector: 'app-userviewclass',
  templateUrl: './userviewclass.component.html',
  styleUrls: ['./userviewclass.component.css']
})

export class UserviewclassComponent implements OnInit {

  cookingClasses: CookingClass[] = [];
  filteredClasses: CookingClass[] = [];
  searchTerm: string = '';
  selectedClass: CookingClass | null = null;
  loading: boolean = true;
  error: string = '';
  showDetailsModal: boolean = false;
  
currentPage: number = 1;
itemsPerPage: number = 6;


  
  constructor(

    private cookingClassService: CookingClassService,
    private router: Router
  ) { }

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
      classItem => 
        classItem.className.toLowerCase().includes(term) ||
        classItem.cuisineType.toLowerCase().includes(term) ||
        classItem.chefName.toLowerCase().includes(term) ||
        classItem.location.toLowerCase().includes(term) ||
        classItem.skillLevel.toLowerCase().includes(term)
    );
  }


  viewDetails(cookingClass: CookingClass): void {
    this.selectedClass = cookingClass;
    this.showDetailsModal = true;
  }


  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedClass = null;
  }


  applyForClass(): void {

    console.log(this.selectedClass);
    if (this.selectedClass && this.selectedClass.cookingClassId) {
      localStorage.setItem('selectedClassId', this.selectedClass.cookingClassId.toString());
      this.router.navigate(['/user/add-request']);
    }
    
  }

  get totalPages(): number {
    return Math.ceil(this.filteredClasses.length / this.itemsPerPage);
  }
  
  paginatedClasses() {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;
    return this.filteredClasses.slice(start, end);
  }
  
  prevPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }
  
  nextPage() {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }
  
  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }
  
  getPageArray(): number[] {
    return Array(this.totalPages).fill(0).map((x, i) => i + 1);
  }
}