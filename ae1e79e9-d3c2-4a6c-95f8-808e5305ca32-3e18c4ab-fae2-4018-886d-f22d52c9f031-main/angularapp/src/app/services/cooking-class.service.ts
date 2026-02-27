import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookingClass } from '../models/cooking-class.model';
import { CookingClassRequest } from '../models/cooking-class-request.model';

import { apiUrl } from 'src/globalUrl';
@Injectable({
  providedIn: 'root'
})
export class CookingClassService {
  //public apiUrl = 'https://8080-abeecbbbdbedfffbcadcffcbecabfaedfdcf.premiumproject.examly.io/api';
  public apiUrl =apiUrl;
  constructor(private http: HttpClient) { }

  // Get auth headers
  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('jwtToken');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // Get all cooking classes
  getAllCookingClasses(): Observable<CookingClass[]> {
    return this.http.get<CookingClass[]>(`${this.apiUrl}/cookingClass`, {
      headers: this.getAuthHeaders()
    });
  }

  // Get cooking class by ID
  getCookingClassById(classId: number): Observable<CookingClass> {
    return this.http.get<CookingClass>(`${this.apiUrl}/cookingClass/${classId}`, {
      headers: this.getAuthHeaders()
    });
  }

  // Add new cooking class
  addCookingClass(cooking: CookingClass): Observable<string> {
    return this.http.post(`${this.apiUrl}/cookingClass`, cooking, {
      headers: this.getAuthHeaders(),
      responseType: 'text' // Ensures response is treated as plain text
    });
  }


  // Update cooking class
  updateCookingClass(classId: number, cooking: CookingClass): Observable<string> {
    return this.http.put(`${this.apiUrl}/cookingClass/${classId}`, cooking, {
      headers: this.getAuthHeaders(),
      responseType: 'text'
    });
  }

  // Delete cooking class
  deleteCookingClass(classId: number): Observable<string> {
    return this.http.delete(`${this.apiUrl}/cookingClass/${classId}`, {
      headers: this.getAuthHeaders(),
      responseType: 'text'
    });
  }


  // Get all cooking class requests
  getAllCookingClassRequests(): Observable<CookingClassRequest[]> {
    return this.http.get<CookingClassRequest[]>(`${this.apiUrl}/cooking-class-request`, {
      headers: this.getAuthHeaders()
    });
  }

  // Get cooking class requests by user ID
  getCookingClassRequestsByUserId(userId: number): Observable<CookingClassRequest[]> {
    return this.http.get<CookingClassRequest[]>(`${this.apiUrl}/cooking-class-request/user/${userId}`, {
      headers: this.getAuthHeaders()
    });
  }

  // Add cooking class request
  addCookingClassRequest(request: CookingClassRequest): Observable<string> {
    return this.http.post(`${this.apiUrl}/cooking-class-request`, request, {
      headers: this.getAuthHeaders(),
      responseType: 'text'
    });
  }

  // Update cooking class request
  updateCookingClassRequest(requestId: string, request: CookingClassRequest): Observable<string> {
    return this.http.put(`${this.apiUrl}/cooking-class-request/${requestId}`, request, {
      headers: this.getAuthHeaders(),
      responseType: 'text'
    });
  }

  // Delete cooking class request
  deleteCookingClassRequest(requestId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/cooking-class-request/${requestId}`, {
      headers: this.getAuthHeaders()
    });
  }
}