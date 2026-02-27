import { Component, OnInit } from '@angular/core';
import { CookingClassService } from '../../services/cooking-class.service';
import { AuthService } from '../../services/auth.service';
import { CookingClassRequest } from '../../models/cooking-class-request.model';
import Swal from 'sweetalert2';

@Component({

  selector: 'app-userviewappliedrequest',
  templateUrl: './userviewappliedrequest.component.html',
  styleUrls: ['./userviewappliedrequest.component.css']
  
})


export class UserviewappliedrequestComponent implements OnInit {

  requests: CookingClassRequest[] = [];
  filteredRequests: CookingClassRequest[] = [];
  statusFilter: string = '';
  loading: boolean = true;
  error: string = '';
  selectedRequest: CookingClassRequest | null = null;
  showDetailsModal: boolean = false;

  constructor(

    private cookingClassService: CookingClassService,
    private authService: AuthService
  ) { }


  ngOnInit(): void {
    this.loadRequests();
  }


  loadRequests(): void {

  this.loading = true;
  const userInfo = this.authService.getUserInfo();

  this.cookingClassService.getCookingClassRequestsByUserId(userInfo.id).subscribe(

    requests => {
      this.requests = requests;

      this.requests.forEach(request => {
        this.cookingClassService.getCookingClassById(request.cookingClassId).subscribe(
          cookingClass => {
            request.cookingClass = cookingClass; // Attach details to request
          },

          error => {
            console.error(`Error fetching class details for ID ${request.cookingClassId}:`, error);
          }
        );
      });

      this.applyFilters();
      this.loading = false;
    },

    error => {

      console.error('Error loading cooking class requests:', error);
      this.error = 'Failed to load requests. Please try again later.';
      this.loading = false;
    }
  );
}

  applyFilters(): void {

    let filtered = this.requests;

    // Apply status filter

    if (this.statusFilter) {
      filtered = filtered.filter(request => request.status === this.statusFilter);
    }
    this.filteredRequests = filtered;
  }


  viewDetails(request: CookingClassRequest): void {
    this.selectedRequest = request;
    this.showDetailsModal = true;
  }
  
  deleteRequest(requestId: number | undefined): void {
    if (!requestId) return;
  
    Swal.fire({
      title: 'Are you sure?',
      text: 'Do you really want to delete this request?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
      if (result.isConfirmed) {
        this.showDetailsModal = false;
        this.selectedRequest = null;
  
        this.cookingClassService.deleteCookingClassRequest(requestId).subscribe({
          next: () => {
            this.requests = this.requests.filter(r => r.cookingClassRequestId !== requestId);
            this.filteredRequests = this.filteredRequests.filter(r => r.cookingClassRequestId !== requestId);
  
            Swal.fire(
              'Deleted!',
              'The request has been deleted.',
              'success'
            );
          },
          error: (err) => {        
              Swal.fire(
                'Error!',
                'Something went wrong. Reloading the page...',
                'error'
              )
          }
        });
      }
    });
  }
  
  
  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedRequest = null;
  }
}
