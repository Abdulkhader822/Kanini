export interface TransactionCreateDto {
  artworkId: number;
  buyerId?: number;
  paymentMethod: string;
}
export interface TransactionDto {
  transactionId: number;
  artworkId: number;
  buyerId: number;
   buyerName: string;
  finalPrice: number;
  transactionDate: string;
  paymentMethod: string;
  paymentStatus: string;
}
