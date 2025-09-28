import { Component, OnInit } from '@angular/core';
import { Artwork } from '../Models/artwork.model';
import { ArtworkService } from '../services/artwork.service';
import { AuthService } from '../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-artwork-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  styleUrls: ['./artwork-list.component.css'],
  templateUrl: './artwork-list.component.html'
})
export class ArtworkListComponent implements OnInit {
  artworks: Artwork[] = [];
  loading = false;
  error = '';

  constructor(
    private svc: ArtworkService,
    public auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;
    this.svc.getAll().subscribe({
      next: (d) => {
        this.artworks = d;
        this.loading = false;
      },
      error: () => {
        this.error = 'Could not load artworks';
        this.loading = false;
      }
    });
  }

  add() {
    this.router.navigate(['/artworks/new']);
  }

  edit(id: number) {
    this.router.navigate(['/artworks/edit', id]);
  }

  delete(id?: number) {
    if (!id) return;
    if (confirm("Are you sure you want to delete this artwork?")) {
      this.svc.delete(id).subscribe({
        next: () => this.artworks = this.artworks.filter(a => a.artworkId !== id),
        error: () => alert("Delete failed")
      });
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
