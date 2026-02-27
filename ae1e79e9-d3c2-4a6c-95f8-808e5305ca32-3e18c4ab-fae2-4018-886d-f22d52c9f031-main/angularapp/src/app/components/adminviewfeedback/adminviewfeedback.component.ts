import { Component, OnInit } from '@angular/core';
import { FeedbackService } from '../../services/feedback.service';
import { Feedback } from '../../models/feedback.model';
import { HttpClient } from '@angular/common/http';
import { apiUrl } from 'src/globalUrl';
import Swal from 'sweetalert2';
 
@Component({
  selector: 'app-adminviewfeedback',
  templateUrl: './adminviewfeedback.component.html',
  styleUrls: ['./adminviewfeedback.component.css']
})
export class AdminviewfeedbackComponent implements OnInit {
 
  public apiUrl = apiUrl;
  feedbacks: Feedback[] = [];
  filteredFeedbacks: Feedback[] = [];
  loading: boolean = true;
  error: string = '';
 
  startDate: string = '';
  endDate: string = '';
  searchKeyword: string = '';
 
  showProfileModal: boolean = false;
  selectedProfile: Feedback | null = null;
 
  constructor(private feedbackService: FeedbackService, private http: HttpClient) { }
 
  ngOnInit(): void {
    this.loadFeedbacks();
  }
 
  loadFeedbacks(): void {
    this.loading = true;
    this.feedbackService.getFeedbacks().subscribe(
      feedbacks => {
        this.feedbacks = feedbacks;
        this.filteredFeedbacks = feedbacks;
        this.loading = false;
      },
      error => {
        console.error('Error loading feedbacks:', error);
        this.error = 'Failed to load feedbacks. Please try again later.';
        this.loading = false;
      }
    );
  }
 
  filterFeedbacksByDate(): void {
    // Validation: From Date is selected but To Date is missing
    if (this.startDate && !this.endDate) {
      Swal.fire({
        icon: 'warning',
        title: 'Missing To Date',
        text: 'Sorry, you didn’t mention the To Date.',
        confirmButtonColor: '#d33'
      });
      return;
    }
 
    // If both dates are not selected, reset filter
    if (!this.startDate || !this.endDate) {
      this.filteredFeedbacks = this.feedbacks;
      this.searchComments(); // Apply keyword filter if any
      return;
    }
 
    const start = new Date(this.startDate);
    const end = new Date(this.endDate);
 
    if (start > end) {
      Swal.fire({
        icon: 'warning',
        title: 'Invalid Date Range',
        text: 'From Date must be less than To Date.',
        confirmButtonColor: '#d33'
      });
      return;
    }
 
    start.setHours(0, 0, 0, 0);
    end.setHours(23, 59, 59, 999);
 
    const dateFiltered = this.feedbacks.filter(feedback => {
      const feedbackDate = new Date(feedback.date);
      return feedbackDate >= start && feedbackDate <= end;
    });
 
    this.filteredFeedbacks = dateFiltered;
    this.searchComments(); // Apply keyword filter on top of date filter
  }
 
  searchComments(): void {
    const keyword = this.searchKeyword.trim().toLowerCase();
 
    // Start from the full feedback list
    let baseList = this.feedbacks;
 
    // Apply date filter if both dates are selected
    if (this.startDate && this.endDate) {
      const start = new Date(this.startDate);
      const end = new Date(this.endDate);
      start.setHours(0, 0, 0, 0);
      end.setHours(23, 59, 59, 999);
 
      baseList = baseList.filter(feedback => {
        const feedbackDate = new Date(feedback.date);
        return feedbackDate >= start && feedbackDate <= end;
      });
    }
 
    // Apply keyword filter
    if (keyword) {
      baseList = baseList.filter(feedback =>
        feedback.feedbackText.toLowerCase().includes(keyword)
      );
    }
 
    // Update the filtered list
    this.filteredFeedbacks = baseList;
  }
 
  contactUser(feedback: Feedback): void {
    const payload = {
      toEmail: feedback.email,
      username: feedback.username,
      feedbackText: feedback.feedbackText
    };
 
    this.http.post(`${this.apiUrl}/feedback/contact-user`, payload).subscribe({
      next: () => {
        Swal.fire({
          icon: 'success',
          title: 'Email Sent',
          text: 'Email sent successfully!',
          confirmButtonColor: '#3085d6'
        });
      },
      error: err => {
        console.error(err);
        Swal.fire({
          icon: 'error',
          title: 'Email Failed',
          text: 'Failed to send email. Please try again later.',
          confirmButtonColor: '#d33'
        });
      }
    });
  }
 
  openProfileModal(profile: Feedback): void {
    this.selectedProfile = profile;
    this.showProfileModal = true;
  }
 
  closeProfileModal(): void {
    this.showProfileModal = false;
    this.selectedProfile = null;
  }
}