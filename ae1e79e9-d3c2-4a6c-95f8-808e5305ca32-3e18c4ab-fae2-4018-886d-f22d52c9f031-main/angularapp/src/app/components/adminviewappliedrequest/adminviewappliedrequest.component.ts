import { Component, OnInit } from '@angular/core';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClassRequest } from '../../models/cooking-class-request.model';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-adminviewappliedrequest',
  templateUrl: './adminviewappliedrequest.component.html',
  styleUrls: ['./adminviewappliedrequest.component.css']
})

export class AdminviewappliedrequestComponent implements OnInit {

  requests: CookingClassRequest[] = [];
  filteredRequests: CookingClassRequest[] = [];
  searchTerm: string = '';
  statusFilter: string = '';
  loading: boolean = true;
  error: string = '';
  selectedRequest: CookingClassRequest | null = null;
  showProfileModal: boolean = false;

  constructor(private cookingClassService: CookingClassService) {}

  ngOnInit(): void {
    this.loadRequests();
  }

  loadRequests(): void {
    this.loading = true;
    this.cookingClassService.getAllCookingClassRequests().subscribe(
      requests => {
        this.requests = requests;
        this.filteredRequests = requests;
        this.loading = false;
      },

      error => {
        console.error('Error loading cooking class requests:', error);
        this.error = 'Failed to load requests. Please try again later.';
        this.loading = false;
      }
    );
  }

  search(): void {
    this.applyFilters();
  }

  applyFilters(): void {

    let filtered = this.requests;

    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(
        request => request.cookingClass?.className.toLowerCase().includes(term)
      );
    }

    if (this.statusFilter) {
      filtered = filtered.filter(request => request.status === this.statusFilter);
    }

    this.filteredRequests = filtered;
  }

  updateStatus(requestId: number, status: string): void {

    const request = this.requests.find(r => r.cookingClassRequestId === requestId);
    if (!request) return;

    const updatedRequest = { ...request, status: status };

    this.cookingClassService.updateCookingClassRequest(requestId.toString(), updatedRequest).subscribe(
      () => {
        const index = this.requests.findIndex(r => r.cookingClassRequestId === requestId);
        if (index !== -1) {
          this.requests[index].status = status;
          this.applyFilters();

          Swal.fire({
            icon: 'success',
            title: 'Status Updated',
            text: `Request status updated to ${status}`,
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true,
            position: 'center',
            showClass: {
              popup: 'animate__animated animate__fadeInDown'
            },
            hideClass: {
              popup: 'animate__animated animate__fadeOutUp'
            }
          });
        }
      },

      error => {
        console.error('Error updating request status:', error);
        Swal.fire({
          icon: 'error',
          title: 'Update Failed',
          text: 'Failed to update request status',
          confirmButtonColor: '#d33',
          showClass: {
            popup: 'animate__animated animate__shakeX'
          }
        });
      }
    );
  }

  viewProfile(request: CookingClassRequest): void {
    this.selectedRequest = request;
    this.showProfileModal = true;
  }

  closeProfileModal(): void {
    this.showProfileModal = false;
    this.selectedRequest = null;
  }

  confirmAndUpdateStatus(requestId: number, status: string): void {
    Swal.fire({
      title: `Mark this request as ${status}?`,
      text: 'This action cannot be undone.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#aaa',
      confirmButtonText: 'Yes, update it!',

      showClass: {
        popup: 'animate__animated animate__fadeInDown'
      },

      hideClass: {
        popup: 'animate__animated animate__fadeOutUp'
      }
      
    }).then(result => {
      if (result.isConfirmed) {
        this.updateStatus(requestId, status);
      }
    });
  }
  currentPage: number = 1;
  itemsPerPage: number = 5; // or preferred items per page
  
  get totalPages(): number {
    return Math.ceil(this.filteredRequests.length / this.itemsPerPage);
  }
  
  paginatedClasses() {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredRequests.slice(start, start + this.itemsPerPage);
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
    return Array(this.totalPages).fill(0).map((_, i) => i + 1);
  }
}
