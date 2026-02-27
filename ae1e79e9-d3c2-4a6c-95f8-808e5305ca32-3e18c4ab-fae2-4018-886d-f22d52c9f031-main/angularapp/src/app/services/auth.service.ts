import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { User } from '../models/user.model';
import { Login } from '../models/login.model';
import { EncryptPasswordService } from './encrypt-password.service';
import { apiUrl } from 'src/globalUrl';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  public apiUrl = apiUrl;
  private userRole = new BehaviorSubject<string>('');
  private userId = new BehaviorSubject<number>(0);
  private username = new BehaviorSubject<string>('');
  private email = new BehaviorSubject<string>('');
  private mobileNumber = new BehaviorSubject<string>('');

  userRole$ = this.userRole.asObservable();
  userId$ = this.userId.asObservable();
  username$ = this.username.asObservable();
  email$ = this.email.asObservable();
  mobileNumber$ = this.mobileNumber.asObservable();

  constructor(private http: HttpClient, private encryptService: EncryptPasswordService) {
    this.checkToken();
  }

  private checkToken(): void {
    const token = localStorage.getItem('jwtToken');
    const role = localStorage.getItem('userRole');
    const userId = localStorage.getItem('userId');
    const username = localStorage.getItem('username');
    const email = localStorage.getItem('email');
    const mobile = localStorage.getItem('mobileNumber');

    if (token && role && userId && username && email && mobile) {
      this.userRole.next(role);
      this.userId.next(Number(userId));
      this.username.next(username);
      this.email.next(email);
      this.mobileNumber.next(mobile);
    }
  }

  register(user: User): Observable<any> {
    const encryptedUser = { ...user, password: this.encryptService.encrypt(user.password) };
    return this.http.post(`${this.apiUrl}/register`, encryptedUser, { responseType: 'text' });
  }

  login(login: Login): Observable<any> {
    const encryptedPassword = this.encryptService.encrypt(login.Password);
    const encryptedLogin = { ...login, Password: encryptedPassword };
    return this.http.post(`${this.apiUrl}/login`, encryptedLogin, { responseType: 'text' })
      .pipe(
        tap((response: any) => {
          const parsedResponse = typeof response === 'string' ? JSON.parse(response) : response;
          localStorage.setItem('jwtToken', parsedResponse.token);
          localStorage.setItem('userId', parsedResponse.user.id);
          localStorage.setItem('username', parsedResponse.user.username);
          localStorage.setItem('userRole', parsedResponse.role);
          localStorage.setItem('email', parsedResponse.user.email);
          localStorage.setItem('mobileNumber', parsedResponse.user.mobileNumber);

          this.userRole.next(parsedResponse.role);
          this.userId.next(parsedResponse.user.id);
          this.username.next(parsedResponse.user.username);
          this.email.next(parsedResponse.user.email);
          this.mobileNumber.next(parsedResponse.user.mobileNumber);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('userId');
    localStorage.removeItem('username');
    localStorage.removeItem('userRole');
    localStorage.removeItem('email');
    localStorage.removeItem('mobileNumber');

    this.userRole.next('');
    this.userId.next(0);
    this.username.next('');
    this.email.next('');
    this.mobileNumber.next('');
  }

  getUserRole(): string {
    return localStorage.getItem('userRole') || '';
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('jwtToken');
  }

  isAdmin(): boolean {
    return this.getUserRole() === 'Admin';
  }

  isUser(): boolean {
    return this.getUserRole() === 'User';
  }

  getUserInfo(): { id: number, username: string, email: string, mobileNumber: string } {
    return {
      id: +(localStorage.getItem('userId') || '0'),
      username: localStorage.getItem('username') || '',
      email: localStorage.getItem('email') || '',
      mobileNumber: localStorage.getItem('mobileNumber') || ''
    };
  }
}
