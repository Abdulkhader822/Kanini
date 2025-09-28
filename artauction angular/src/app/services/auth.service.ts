import { HttpClient } from '@angular/common/http';
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';

interface LoginResp {
  token: string;
  userId: number;
  fullName: string;
  role: string | { roleName?: string } | null;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = 'https://localhost:7222/api/Auth';

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  login(email: string, password: string): Observable<LoginResp> {
    return this.http.post<LoginResp>(`${this.base}/login`, { email, password }).pipe(
      tap(resp => {
        if (resp?.token && isPlatformBrowser(this.platformId)) {
          localStorage.setItem('jwt_token', resp.token);

          // Normalize role
          let role = resp.role;
          if (typeof role === 'object' && role?.roleName) {
            role = role.roleName;
          }

          localStorage.setItem('current_user', JSON.stringify({
            userId: resp.userId,
            fullName: resp.fullName,
            role: (role as string)?.toLowerCase()
          }));
        }
      })
    );
  }

  register(payload: any) {
    return this.http.post(`${this.base}/register`, payload);
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('jwt_token');
      localStorage.removeItem('current_user');
    }
  }

  getToken(): string | null {
    return isPlatformBrowser(this.platformId) ? localStorage.getItem('jwt_token') : null;
  }

  getCurrentUser(): { userId: number; fullName: string; role: string } | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    const v = localStorage.getItem('current_user');
    return v ? JSON.parse(v) : null;
  }

  getCurrentUserRole(): string | null {
    return this.getCurrentUser()?.role ?? null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  // Role helpers
  isAdmin(): boolean { return this.getCurrentUserRole() === 'admin'; }
  isArtist(): boolean { return this.getCurrentUserRole() === 'artist'; }
  isBuyer(): boolean { return this.getCurrentUserRole() === 'buyer'; }
}
