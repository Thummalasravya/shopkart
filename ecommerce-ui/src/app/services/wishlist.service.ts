import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, map } from 'rxjs';
import { Product } from '../models/product.model';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class WishlistService {

  private api = 'http://localhost:5273/api/Wishlist';

  private wishlistSubject = new BehaviorSubject<any[]>([]);
  wishlist$ = this.wishlistSubject.asObservable();

  wishlistCount$ = this.wishlist$.pipe(
    map(items => items.length)
  );

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private router: Router
  ) {
    this.refresh();
  }

  /* ================= REFRESH ================= */

  refresh() {
    if (!this.auth.isLoggedIn()) {
      this.wishlistSubject.next([]);
      return;
    }

    this.http.get<any[]>(this.api).subscribe(items => {
      this.wishlistSubject.next([...items]);
    });
  }

  /* ================= LOGIN CHECK ================= */

  private requireLogin(): boolean {
    if (this.auth.isLoggedIn()) return true;

    // save redirect
    this.auth.setRedirectUrl(this.router.url);
    this.router.navigate(['/login']);
    return false;
  }

  /* ================= TOGGLE ================= */

  toggle(product: Product) {

    // 🔥 BLOCK IF NOT LOGGED IN
    if (!this.requireLogin()) return;

    const items = [...this.wishlistSubject.value];
    const index = items.findIndex(
      i => i.product.productId === product.productId
    );

    if (index > -1) {
      const removed = items[index];
      items.splice(index, 1);

      this.wishlistSubject.next(items);

      this.http.delete(`${this.api}/${removed.id}`)
        .subscribe({ error: () => this.refresh() });

    } else {
      const temp = { id: -1, product };
      items.push(temp);

      this.wishlistSubject.next(items);

      this.http.post(this.api, { productId: product.productId })
        .subscribe(() => this.refresh());
    }
  }

  /* ================= CHECK ================= */

  isInWishlist(productId: number): boolean {
    return this.wishlistSubject.value.some(
      i => i.product.productId === productId
    );
  }
}
