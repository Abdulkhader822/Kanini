import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ArtworkListComponent } from './artwork-list/artwork-list.component';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';
import { ArtworkFormComponent } from './artwork-form/artwork-form.component';
import { BidComponent } from './bid/bid.component';
import { RegisterComponent } from './register-component/register-component.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent},

  { path: 'artworks', component: ArtworkListComponent, canActivate: [AuthGuard] },
  {
    path: 'artworks/new',
    component: ArtworkFormComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Artist','admin'] }
  },
  { path: 'artworks/edit/:id',
    component: ArtworkFormComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Artist','admin'] }
  },
  {
    path: 'bid/:id', 
   component: BidComponent, 
   canActivate: [AuthGuard, RoleGuard], 
   data: { roles: ['buyer', 'admin', 'artist'] }
  }

];
