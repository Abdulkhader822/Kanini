import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TransactionService } from '../services/transaction.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-payment',
  template: `
    <div class="container mt-3">
      <h3>Mock Payment</h3>
      <p *ngIf="message" class="alert alert-info">{{ message }}</p>
      <button class="btn btn-success" (click)="pay()">Pay Now (Mock)</button>
      <div *ngIf="receiptUrl" class="mt-3">
        <p>Payment successful. <a [href]="receiptUrl" target="_blank">Download receipt</a></p>
        <p><strong>Delivered successfully</strong></p>
      </div>
    </div>
  `
})
export class PaymentComponent implements OnInit {
  artworkId!: number;
  message = '';
  receiptUrl?: string;

  constructor(private route: ActivatedRoute, private txnSvc: TransactionService, private auth: AuthService) {}

  ngOnInit() {
    this.artworkId = Number(this.route.snapshot.paramMap.get('id'));
  }

  pay() {
    this.txnSvc.pay(this.artworkId).subscribe({
      next: (res) => {
        this.receiptUrl = res.receiptUrl;
        this.message = res.message;
      },
      error: (err) => this.message = err?.error || 'Payment failed'
    });
  }
}
