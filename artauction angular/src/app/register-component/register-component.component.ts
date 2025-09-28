import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register-component.component.html',   // ✅ matches your file
  styleUrls: ['./register-component.component.css']     // ✅ matches your file
})
export class RegisterComponent {
  form: FormGroup;
  error = '';

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
   this.form = this.fb.group({
  fullName: ['', Validators.required],   // ✅ not userName
  email: ['', [Validators.required, Validators.email]],
  password: ['', Validators.required],
  role: ['', Validators.required]
});

  }

submit() {
  if (this.form.invalid) {
    this.error = 'Please fill all fields correctly';
    return;
  }

  this.auth.register(this.form.value).subscribe({
    next: () => this.router.navigate(['/login']),
    error: (err) => {
      if (err?.status === 400 && err.error?.message?.includes('Email already exists')) {
        this.error = 'This email is already registered';
      } else {
        this.error = err?.error?.message || 'Registration failed';
      }
    }
  });
}

}
