import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  name = '';
  email = '';
  password = '';
  confirmPassword = '';

  showPassword = false;
  showConfirmPassword = false;

  error = '';

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  register(): void {
    this.error = '';

    if (!this.name || !this.email || !this.password || !this.confirmPassword) {
      this.error = 'All fields are required';
      return;
    }

    if (!this.isStrongPassword(this.password)) {
      this.error =
        'Password must be at least 8 characters with uppercase, lowercase, number and special character';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match';
      return;
    }

    const success = this.auth.register({
      name: this.name,
      email: this.email,
      password: this.password
    });

    if (!success) {
      this.error = 'Email already exists';
      return;
    }

    alert('Account created successfully');
    this.router.navigate(['/login']);
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  private isStrongPassword(password: string): boolean {
    const regex =
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    return regex.test(password);
  }
}
