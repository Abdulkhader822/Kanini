export interface Artwork {
  artworkId: number;          // ✅ always required
  title: string;
  description: string;
  category: string;
  startingPrice: number;
  auctionStartTime: Date;   // or Date
  auctionEndTime: Date;     // or Date
  artistId: number;

  imageBase64?: string;
}

export interface ArtworkCreateDto {
  title: string;
  description: string;
  category: string;
  startingPrice: number;
  auctionStartTime: string;
  auctionEndTime: string;
  artistId: number;
    imageFile?: File;

}
