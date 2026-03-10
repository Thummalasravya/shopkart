import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export interface RegisterDto {
  name: string;
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {

  private API = 'http://localhost:5273/api/auth';
  private redirectUrl = '/';

  /* LOGIN STATE */

  private loggedInSubject = new BehaviorSubject<boolean>(
    !!localStorage.getItem('token')
  );

  loggedIn$ = this.loggedInSubject.asObservable();

  /* PROFILE PHOTO */

  private profilePhotoSubject = new BehaviorSubject<string | null>(
    localStorage.getItem('profilePhoto')
  );

  profilePhoto$ = this.profilePhotoSubject.asObservable();

  /* USERNAME */

  private userNameSubject = new BehaviorSubject<string>(
    localStorage.getItem('userName') || ''
  );

  userName$ = this.userNameSubject.asObservable();

  constructor(
    private router: Router,
    private http: HttpClient
  ) {}

  /* REGISTER */

  register(user: RegisterDto): Promise<boolean> {

    return new Promise(resolve => {

      this.http.post(`${this.API}/register`, user)
        .subscribe({
          next: () => {
            resolve(true);
          },
          error: () => {
            resolve(false);
          }
        });

    });

  }

  /* LOGIN */

  login(email: string, password: string): Promise<boolean> {

    return new Promise(resolve => {

      this.http.post<any>(`${this.API}/login`, { email, password })
        .subscribe({

          next: res => {

            const token = res.token;

            localStorage.setItem('token', token);
            localStorage.setItem('userEmail', email);

            const payload = JSON.parse(atob(token.split('.')[1]));

            const userId =
              payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

            localStorage.setItem('userId', userId);

            const name = res.name || email.split('@')[0];
            this.setUserName(name);

            this.loggedInSubject.next(true);

            resolve(true);
          },

          error: () => {
            resolve(false);
          }

        });

    });

  }

  /* GOOGLE LOGIN */

  googleLogin(idToken: string): Promise<boolean> {

    return new Promise(resolve => {

      this.http.post<any>(`${this.API}/google-login`, { idToken })
        .subscribe({

          next: res => {

            const token = res.token;

            localStorage.setItem('token', token);
            localStorage.setItem('userEmail', res.email || '');

            const payload = JSON.parse(atob(token.split('.')[1]));

            const userId =
              payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

            localStorage.setItem('userId', userId);

            this.setUserName(res.name);

            this.loggedInSubject.next(true);

            resolve(true);
          },

          error: () => {
            resolve(false);
          }

        });

    });

  }

  /* LOGOUT */

  logout(): void {

    localStorage.clear();

    this.userNameSubject.next('');
    this.profilePhotoSubject.next(null);
    this.loggedInSubject.next(false);

    this.router.navigate(['/login']);
  }

  /* AUTH CHECK */

  isLoggedIn(): boolean {
    return this.loggedInSubject.value;
  }

  /* PROFILE */

  getUserEmail(): string | null {
    return localStorage.getItem('userEmail');
  }

  getUserId(): string | null {
    return localStorage.getItem('userId');
  }

  getUserName(): string {
    return this.userNameSubject.value;
  }

  setUserName(name: string): void {

    localStorage.setItem('userName', name);
    this.userNameSubject.next(name);

  }

  getProfilePhoto(): string | null {
    return localStorage.getItem('profilePhoto');
  }

  setProfilePhoto(photo: string): void {

    localStorage.setItem('profilePhoto', photo);
    this.profilePhotoSubject.next(photo);

  }

  removeProfilePhoto(): void {

    localStorage.removeItem('profilePhoto');
    this.profilePhotoSubject.next(null);

  }

  /* REDIRECT */

  setRedirectUrl(url: string): void {
    this.redirectUrl = url;
  }

  getRedirectUrl(): string {
    return this.redirectUrl;
  }

  clearRedirectUrl(): void {
    this.redirectUrl = '/';
  }

  /* FORGOT PASSWORD (UI SAFE) */

  isEmailRegistered(email: string): boolean {
    return true;
  }

  resetPassword(email: string, newPassword: string): boolean {
    alert('Password reset not implemented yet');
    return true;
  }

}