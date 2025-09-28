import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';
import { AuthService } from './auth.service';
import { Bid } from '../Models/bid.model';

@Injectable({ providedIn: 'root' })
export class BidService {
  private base = 'https://localhost:7222/api/bid';
  private hub: signalR.HubConnection | null = null;
  private bidSubject = new Subject<Bid>();

  constructor(private http: HttpClient, private auth: AuthService) {}

  connect(artworkId: number) {
    if (this.hub) return;
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7222/auctionhub`, {
        accessTokenFactory: () => this.auth.getToken() || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hub.on('BidPlaced', (bid: Bid) => {
      this.bidSubject.next(bid);
    });

    this.hub.start().then(() => {
      this.hub!.invoke('JoinArtworkGroup', artworkId).catch(console.error);
    }).catch(console.error);
  }

  disconnect(artworkId: number) {
    if (!this.hub) return;
    this.hub.invoke('LeaveArtworkGroup', artworkId).finally(() => {
      this.hub!.stop().finally(() => this.hub = null);
    });
  }

  onBid(): Observable<Bid> {
    return this.bidSubject.asObservable();
  }

  placeBid(artworkId: number, bidAmount: number) {
    return this.http.post<Bid>(this.base, { artworkId, bidAmount });
  }

  getHighest(artworkId: number) {
    return this.http.get<Bid | null>(`${this.base}/highest/${artworkId}`);
  }

  getHistory(artworkId: number): Observable<Bid[]> {
  return this.http.get<Bid[]>(`${this.base}/history/${artworkId}`);
}


  
}
