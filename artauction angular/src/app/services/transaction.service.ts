import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private base = 'https://localhost:7222/api/transaction';
  constructor(private http: HttpClient) {}

  pay(artworkId: number) {
    return this.http.post<any>(this.base, { artworkId });
  }
}
