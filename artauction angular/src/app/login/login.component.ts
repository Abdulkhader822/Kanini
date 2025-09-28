import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']   // add your CSS file for centering layout
})
export class LoginComponent {
  form: FormGroup;
  error = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],   // ✅ email validation
      password: ['', Validators.required]
    });
  }

  submit() {
    if (this.form.invalid) {
      this.error = 'Please enter a valid email and password';
      return;
    }

    const { email, password } = this.form.value;

    this.auth.login(email, password).subscribe({
      next: () => {
        this.error = '';
        this.router.navigate(['/artworks']); // ✅ redirect after login
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === 401) {
          this.error = 'Invalid email or password';  // ✅ wrong password
        } else {
          this.error = err.error?.message || 'Login failed. Please try again.';
        }
      }
    });
  }
}
