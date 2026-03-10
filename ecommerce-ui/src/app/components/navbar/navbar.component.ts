import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { AuthService } from '../../services/auth.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  private cartService = inject(CartService);
  private wishlistService = inject(WishlistService);
  public auth = inject(AuthService);

  searchText = '';

  // ✅ use cartCount$ directly (🔥 FIX)
  cartCount$ = this.cartService.cartCount$;

  // wishlist count
  wishlistCount$ = this.wishlistService.wishlistCount$;

  // user initial avatar
  userInitial$ = this.auth.userName$.pipe(
    map(name => name ? name.charAt(0).toUpperCase() : '')
  );

  onSearchChange() {
    localStorage.setItem('searchText', this.searchText);
    window.dispatchEvent(new Event('search-updated'));
  }

  logout() {
    this.auth.logout();
  }
}
