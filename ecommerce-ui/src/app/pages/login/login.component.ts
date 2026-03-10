import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

import { AuthService } from '../../services/auth.service';

declare const google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(
    private auth: AuthService,
    private router: Router
  ) {}

  ////////////////////////////////
  // NORMAL LOGIN
  ////////////////////////////////

  login(form: NgForm): void {

    this.error = '';

    if (form.invalid) {
      this.error = 'Please fix validation errors';
      return;
    }

    this.loading = true;

    this.auth.login(this.email, this.password)
      .then(success => {

        this.loading = false;

        if (!success) {
          this.error = 'Invalid email or password';
          return;
        }

        const redirect = this.auth.getRedirectUrl();
        this.auth.clearRedirectUrl();

        this.router.navigate([redirect]);

      })
      .catch(() => {

        this.loading = false;
        this.error = 'Login failed. Try again.';

      });

  }

  ////////////////////////////////
  // GOOGLE LOGIN INIT
  ////////////////////////////////

  ngOnInit(): void {

    if (typeof google === 'undefined') return;

    google.accounts.id.initialize({

      client_id: '288205001215-jn8ie3arc1jeqgo248kbot9hathb12d5.apps.googleusercontent.com',

      callback: (response: any) => this.handleGoogleLogin(response)

    });

    google.accounts.id.renderButton(
      document.getElementById("googleButton"),
      {
        theme: "outline",
        size: "large",
        width: "280"
      }
    );

  }

  ////////////////////////////////
  // HANDLE GOOGLE LOGIN
  ////////////////////////////////

  handleGoogleLogin(response: any) {

    const idToken = response.credential;

    if (!idToken) {
      this.error = "Google login failed";
      return;
    }

    this.auth.googleLogin(idToken)
      .then(success => {

        if (!success) {
          this.error = "Google login failed";
          return;
        }

        this.router.navigate(['/']);

      });

  }

}