import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

import { CartService, CartItem } from '../../services/cart.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit, OnDestroy {

  cart: CartItem[] = [];
  total: number = 0;
  loading: boolean = true;

  private sub?: Subscription;

  constructor(
    private cartService: CartService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {

    console.log('🔥 Cart page loaded');

    // 🔥 Always sync backend first
    await this.cartService.loadCartFromBackend();

    // 🔥 Reactive subscription
    this.sub = this.cartService.cart$.subscribe(cart => {
      console.log('🛒 Cart received:', cart);
      this.cart = cart;
      this.calculateTotal();
      this.loading = false;
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  async increase(item: CartItem): Promise<void> {
    await this.cartService.updateQuantity(item, item.quantity + 1);
  }

  async decrease(item: CartItem): Promise<void> {
    if (item.quantity > 1) {
      await this.cartService.updateQuantity(item, item.quantity - 1);
    }
  }

  async toggle(item: CartItem, event: Event): Promise<void> {
    const checked = (event.target as HTMLInputElement).checked;
    await this.cartService.toggleSelect(item, checked);
  }

  async remove(item: CartItem): Promise<void> {
    await this.cartService.removeFromCart(item);
  }

  calculateTotal(): void {
    this.total = this.cart
      .filter(i => i.selected)
      .reduce((sum, i) => sum + i.price * i.quantity, 0);
  }

  /* ================= CHECKOUT ================= */

  checkout(): void {

    const selected = this.cart.filter(i => i.selected);

    if (!selected.length) {
      alert('Please select items');
      return;
    }

    // 🔥 store selected items
    this.cartService.setCheckoutItems(selected);

    // 🔥 navigate to checkout
    this.router.navigate(['/checkout']);
  }

  trackById(index: number, item: CartItem) {
    return item.cartId ?? item.id;
  }
}
