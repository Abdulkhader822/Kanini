import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ArtworkService } from '../services/artwork.service';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-artwork-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule, CommonModule],
  templateUrl: './artwork-form.component.html',
  styleUrls: ['./artwork-form.component.css'] // ✅ for styling
})
export class ArtworkFormComponent implements OnInit {
  form: FormGroup;
  id?: number;
  isEdit = false;
  artworks: any[] = []; // ✅ list of artworks for grid

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private svc: ArtworkService,
    public auth: AuthService
  ) {
   this.form = this.fb.group({
  title: ['', Validators.required],
  description: [''],
  category: ['', Validators.required],
  startingPrice: [0, [Validators.required, Validators.min(1)]],
  auctionStartTime: ['', Validators.required],
  auctionEndTime: ['', Validators.required],
  artistId: [this.auth.getCurrentUser()?.userId],
  imageFile: [null]  
});


  }

  ngOnInit() {
    this.id = Number(this.route.snapshot.paramMap.get('id')) || undefined;

    if (this.id) {
      this.isEdit = true;
      this.svc.getById(this.id).subscribe((a) => this.form.patchValue(a));
    }

    this.loadArtworks();
  }

   onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input?.files && input.files.length > 0) {
      const file = input.files[0];
      this.form.patchValue({ imageFile: file });
      this.form.get('imageFile')?.updateValueAndValidity();
    }
  }

 success = '';
error = '';

save() {
  if (this.form.invalid) {
    this.error = 'Please fill all required fields';
    return;
  }

  const val = this.form.value;

  if (this.isEdit && this.id) {
    this.svc.update(this.id, val).subscribe({
      next: () => {
        this.success = 'Artwork updated successfully!';
        this.router.navigate(['/artworks']);
      },
      error: () => this.error = 'Failed to update artwork'
    });
  } else {
    this.svc.create(val).subscribe({
      next: () => {
        this.success = 'Artwork added successfully!';
        setTimeout(() => this.router.navigate(['/artworks']), 1500); // redirect after 1.5s
      },
      error: () => this.error = 'Failed to create artwork'
    });
  }
}


  loadArtworks() {
    this.svc.getAll().subscribe((list) => (this.artworks = list));
  }

  edit(id: number) {
    this.router.navigate(['/artworks/edit', id]);
  }

  delete(id: number) {
    if (confirm('Are you sure you want to delete this artwork?')) {
      this.svc.delete(id).subscribe(() => this.loadArtworks());
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
