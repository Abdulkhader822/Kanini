import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArtworkService } from '../services/artwork.service';
import { BidService } from '../services/bid.service';
import { AuthService } from '../services/auth.service';
import { Subscription } from 'rxjs';
import { Bid } from '../Models/bid.model';

@Component({
  selector: 'app-artwork-detail',
  templateUrl: './artwork-detail.component.html',
  styleUrls: ['./artwork-detail.component.css']
})
export class ArtworkDetailComponent implements OnInit, OnDestroy {
  artwork: any;
  highest?: Bid | null;
  bids: Bid[] = [];
  bidAmount = 0;
  error = '';
  private artworkId!: number;
  private sub?: Subscription;
  now: Date = new Date();

  constructor(
    private route: ActivatedRoute,
    private artSvc: ArtworkService,
    private bidSvc: BidService,
    public auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.artworkId = Number(this.route.snapshot.paramMap.get('id'));
    this.artSvc.getById(this.artworkId).subscribe(a => this.artwork = a);

    this.bidSvc.getHighest(this.artworkId).subscribe(h => {
      this.highest = h;
      if (h) this.bids = [h];
    });

    this.bidSvc.connect(this.artworkId);
    this.sub = this.bidSvc.onBid().subscribe(b => {
      if (b.artworkId === this.artworkId) {
        this.highest = b;
        this.bids.unshift(b);
      }
    });
  }

  place() {
    this.error = '';
    if (!this.auth.isBuyer()) {
      this.error = 'Only buyers can place bids';
      return;
    }
    if (this.bidAmount <= 0) { 
      this.error = 'Enter a valid amount'; 
      return; 
    }
    const current = this.highest?.bidAmount ?? this.artwork.startingPrice;
    if (this.bidAmount <= current) { 
      this.error = 'Bid must be higher than current highest'; 
      return; 
    }

    this.bidSvc.placeBid(this.artworkId, this.bidAmount).subscribe({
      next: () => { this.bidAmount = 0; },
      error: (e) => this.error = e?.error || 'Bid failed'
    });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
    this.bidSvc.disconnect(this.artworkId);
  }

  goToPay() {
    this.router.navigate(['/pay', this.artworkId]);
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
