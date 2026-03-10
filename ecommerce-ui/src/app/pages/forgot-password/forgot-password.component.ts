import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {

  email = '';
  password = '';
  confirmPassword = '';
  step = 1;
  error = '';

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  verifyEmail(): void {
    this.error = '';

    if (!this.email) {
      this.error = 'Email is required';
      return;
    }

    if (!this.auth.isEmailRegistered(this.email)) {
      this.error = 'Email not found';
      return;
    }

    this.step = 2;
  }

  resetPassword(): void {
    this.error = '';

    if (!this.password || !this.confirmPassword) {
      this.error = 'All fields are required';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    this.auth.resetPassword(this.email, this.password);
    alert('Password updated successfully');

    this.router.navigate(['/login']);
  }
}
