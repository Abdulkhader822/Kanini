export interface Bid {
  bidId?: number;
  bidAmount: number;
  bidTime: string|Date;
  artworkId: number;
  buyerId: number;
}
export interface BidCreateDto {
  bidAmount: number;
  artworkId: number;
  buyerId?: number; // will be filled from token in UI
}
