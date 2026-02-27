import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Feedback } from '../models/feedback.model';
import { apiUrl } from 'src/globalUrl';
@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  public apiUrl =apiUrl;
  constructor(private http: HttpClient) { }

  // Get auth headers
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('jwtToken');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // Send feedback
  sendFeedback(feedback: Feedback): Observable<string> {
    return this.http.post(`${this.apiUrl}/feedback`, feedback, {
      headers: this.getAuthHeaders(),
      responseType : 'text'
    });
  }

  // Get all feedbacks by user ID
  getAllFeedbacksByUserId(userId: number): Observable<Feedback[]> {
    return this.http.get<Feedback[]>(`${this.apiUrl}/feedback/user/${userId}`, {
      headers: this.getAuthHeaders()
    });
  }

  // Delete feedback
  deleteFeedback(feedbackId: number): Observable<string> {
    return this.http.delete(`${this.apiUrl}/feedback/${feedbackId}`, {
      headers: this.getAuthHeaders(),
      responseType : 'text'
    });
  }

  // Get all feedbacks
  getFeedbacks(): Observable<Feedback[]> {
    return this.http.get<Feedback[]>(`${this.apiUrl}/feedback`, {
      headers: this.getAuthHeaders()
    });
  }
}