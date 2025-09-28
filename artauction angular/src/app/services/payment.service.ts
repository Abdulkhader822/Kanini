import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private base = 'https://localhost:7222/api/Transaction';

  constructor(private http: HttpClient, private auth: AuthService) {}

  pay(artworkId: number, method: string) {
    const token = this.auth.getToken();
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`
    });

    return this.http.post<any>(
      `${this.base}/pay`,
      { artworkId, paymentMethod: method },
      { headers }
    );
  }
}
