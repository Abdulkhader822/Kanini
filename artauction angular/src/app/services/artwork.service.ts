import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Artwork, ArtworkCreateDto } from '../Models/artwork.model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ArtworkService {
  private base = 'https://localhost:7222/api/artwork';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Artwork[]> {
    return this.http.get<Artwork[]>(this.base);
  }

  getById(id: number): Observable<Artwork> {
    return this.http.get<Artwork>(`${this.base}/${id}`);
  }

  create(dto: ArtworkCreateDto): Observable<Artwork> {
    const formData = new FormData();
    formData.append('title', dto.title);
    formData.append('description', dto.description);
    formData.append('category', dto.category);
    formData.append('startingPrice', dto.startingPrice.toString());
    formData.append('auctionStartTime', dto.auctionStartTime);
    formData.append('auctionEndTime', dto.auctionEndTime);
    formData.append('artistId', dto.artistId.toString());

    if (dto.imageFile) {
      formData.append('imageFile', dto.imageFile);
    }

    return this.http.post<Artwork>(this.base, formData);
  }

  update(id: number, dto: Partial<ArtworkCreateDto>): Observable<Artwork> {
    const formData = new FormData();
    if (dto.title) formData.append('title', dto.title);
    if (dto.description) formData.append('description', dto.description);
    if (dto.category) formData.append('category', dto.category);
    if (dto.startingPrice !== undefined)
      formData.append('startingPrice', dto.startingPrice.toString());
    if (dto.auctionStartTime) formData.append('auctionStartTime', dto.auctionStartTime);
    if (dto.auctionEndTime) formData.append('auctionEndTime', dto.auctionEndTime);
    if (dto.artistId !== undefined) formData.append('artistId', dto.artistId.toString());

    if (dto.imageFile) {
      formData.append('imageFile', dto.imageFile);
    }

    return this.http.put<Artwork>(`${this.base}/${id}`, formData);
  }

  delete(id: number) {
    return this.http.delete(`${this.base}/${id}`);
  }
}
