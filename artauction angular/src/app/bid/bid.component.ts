import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';

import { BidService } from '../services/bid.service';
import { PaymentService } from '../services/payment.service';
import { AuthService } from '../services/auth.service';
import { ArtworkService } from '../services/artwork.service';
import { ReceiptService } from '../services/receipt.service';

import { Artwork } from '../Models/artwork.model';
import { Bid } from '../Models/bid.model';

@Component({
  selector: 'app-bid',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bid.component.html',
  styleUrls: ['./bid.component.css']
})
export class BidComponent implements OnInit, OnDestroy {
  artwork?: Artwork;
  bids: Bid[] = [];
  highest?: Bid | null;
  bidAmount = 0;
  error = '';
  success = '';
  artworkId!: number;
  private bidSub?: Subscription;

  lastTransactionId: number | null = null;

  constructor(
    private route: ActivatedRoute,
    private bidSvc: BidService,
    private paySvc: PaymentService,
    private artSvc: ArtworkService,
    private receiptSvc: ReceiptService,
    public auth: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.artworkId = Number(this.route.snapshot.paramMap.get('id'));

    this.artSvc.getById(this.artworkId).subscribe({
      next: (a) => this.artwork = a,
      error: () => this.error = 'Could not load artwork'
    });

    this.loadHighest();
    this.loadHistory();

    this.bidSvc.connect(this.artworkId);
    this.bidSub = this.bidSvc.onBid().subscribe((bid) => {
      if (bid.artworkId === this.artworkId) {
        this.highest = bid;
        this.bids.unshift(bid);
      }
    });
  }

  ngOnDestroy(): void {
    this.bidSvc.disconnect(this.artworkId);
    this.bidSub?.unsubscribe();
  }

  loadHighest() {
   this.bidSvc.getHighest(this.artworkId).subscribe({
  next: (b) => this.highest = b,
  error: () => this.error = 'Could not load highest bid'
});

  }

  loadHistory() {
    this.bidSvc.getHistory(this.artworkId).subscribe({
  next: (list) => this.bids = list,
  error: () => this.error = 'Could not load bid history'
});

  }

  placeBid() {
    if (!this.bidAmount) return;

  this.bidSvc.placeBid(this.artworkId, this.bidAmount).subscribe({
  next: () => {
    this.success = 'Bid placed successfully!';
    this.error = '';
    this.bidAmount = 0;
  },
  error: (err) => {
    this.error = err.error?.message || 'Bid failed';
    this.success = '';
  }
});

  }

  pay() {
    this.paySvc.pay(this.artworkId, 'Mock').subscribe({
      next: (resp) => {
        if (resp.success) {
          this.success = resp.message;
          this.error = '';
          this.lastTransactionId = resp.transaction.transactionId;
        } else {
          this.error = resp.message;
        }
      },
      error: (err) => {
        this.error = err.error?.message || 'Payment failed';
        this.success = '';
      }
    });
  }

  downloadReceipt(transactionId: number) {
    this.receiptSvc.getReceiptByTransaction(transactionId).subscribe((blob: Blob) => {
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = `receipt-${transactionId}.pdf`;
      link.click();
    });
  }

  isAuctionEnded(): boolean {
    if (!this.artwork?.auctionEndTime) return false;
    return new Date(this.artwork.auctionEndTime) < new Date();
  }

  isWinner(): boolean {
    if (!this.highest) return false;
    const currentUser = this.auth.getCurrentUser();
    return !!currentUser && this.highest.buyerId === currentUser.userId;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
