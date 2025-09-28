import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ReceiptService {
  private base = 'https://localhost:7222/api/receipt';

  constructor(private http: HttpClient) {}

  getReceiptByTransaction(transactionId: number): Observable<Blob> {
    return this.http.get(`${this.base}/download/${transactionId}`, { responseType: 'blob' });
  }
}
