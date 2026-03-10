import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { WishlistService } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './wishlist.component.html',
  styleUrls: ['./wishlist.component.css']
})
export class WishlistComponent implements OnInit {

  wishlistItems: any[] = [];

  constructor(
    private wishlistService: WishlistService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    // 🔥 LIVE WISHLIST DATA
    this.wishlistService.wishlist$.subscribe(items => {
      this.wishlistItems = items;
    });
  }

  removeFromWishlist(product: any): void {
    // 🔥 USE SERVICE
    this.wishlistService.toggle(product.product);
  }

  addToCart(product: any): void {
    // 🔥 USE CART SERVICE
    this.cartService.addToCart({
      ...product.product,
      id: product.product.productId   // normalize id
    });
  }

  trackById(index: number, item: any) {
  return item.id;
}

}
