import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent {

  email = '';
  userName = '';
  editMode = false;
  tempName = '';

  constructor(
    public auth: AuthService, // 👈 IMPORTANT (public)
    private router: Router
  ) {
    this.email = this.auth.getUserEmail() || '';

    const savedName = this.auth.getUserName();
    if (savedName) {
      this.userName = savedName;
    } else if (this.email) {
      let name = this.email.split('@')[0].split('.')[0].replace(/[0-9]/g, '');
      this.userName = name.charAt(0).toUpperCase() + name.slice(1);
      this.auth.setUserName(this.userName);
    }
  }

  getInitial(): string {
    return this.userName.charAt(0);
  }

  enableEdit(): void {
    this.tempName = this.userName;
    this.editMode = true;
  }

  saveName(): void {
    if (!this.tempName.trim()) return;
    this.userName = this.tempName.trim();
    this.auth.setUserName(this.userName);
    this.editMode = false;
  }

  cancelEdit(): void {
    this.editMode = false;
  }

  // 📷 UPLOAD PHOTO
  onPhotoSelected(event: any): void {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      this.auth.setProfilePhoto(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  // 🗑️ DELETE PHOTO
  removePhoto(): void {
    this.auth.removeProfilePhoto();
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
