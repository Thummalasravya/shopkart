import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { map, firstValueFrom } from 'rxjs';

import { ProductService } from '../../services/product.service';
import { CartService, CartItem } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit, OnDestroy {

  allProducts: Product[] = [];
  products: Product[] = [];
  cartItems: CartItem[] = [];

  categories = ['All', 'Electronics', 'Fashion', 'Home&Furniture', 'Appliances', 'Beauty'];
  selectedCategory = 'All';

  quantities: { [key: number]: number } = {};

  private searchListener = () => this.applyFilters();

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private wishlistService: WishlistService,
    private router: Router
  ) {}

  async ngOnInit(): Promise<void> {

    this.allProducts = this.productService.getProducts();
    this.products = [...this.allProducts];

    await this.cartService.loadCartFromBackend();

    this.cartService.cart$.subscribe(cart => {
      this.cartItems = cart;
      this.syncQuantities();
    });

    window.addEventListener('search-updated', this.searchListener);
  }

  ngOnDestroy(): void {
    window.removeEventListener('search-updated', this.searchListener);
  }

  /* ================= SYNC QTY ================= */

  syncQuantities() {
    this.quantities = {};
    this.cartItems.forEach(item => {
      this.quantities[item.id] = item.quantity;
    });
  }

  /* ================= FILTER ================= */

  filterByCategory(category: string): void {
    this.selectedCategory = category;
    this.applyFilters();
  }

  applyFilters(): void {
    const searchText = (localStorage.getItem('searchText') || '').toLowerCase();

    this.products = this.allProducts.filter(p => {
      const matchCategory =
        this.selectedCategory === 'All' ||
        p.category === this.selectedCategory;

      const matchSearch =
        p.name.toLowerCase().includes(searchText);

      return matchCategory && matchSearch;
    });
  }

  /* ================= WISHLIST ================= */

  toggleWishlist(product: Product, event: MouseEvent): void {
    event.stopPropagation();
    this.wishlistService.toggle(product);
  }

  isWishlisted$(productId: number) {
    return this.wishlistService.wishlist$.pipe(
      map(items => items.some(i => i.product.productId === productId))
    );
  }

  /* ================= ADD ================= */

  async addToCart(product: Product): Promise<void> {
    await this.cartService.addToCart(product, 1);
  }

  /* ================= INCREASE ================= */

  async increase(product: Product): Promise<void> {

    const cart = this.cartItems.find(c => c.id === product.productId);

    if (cart) {
      await this.cartService.updateQuantity(cart, cart.quantity + 1);
    } else {
      await this.cartService.addToCart(product, 1);
    }
  }

  /* ================= DECREASE ================= */

  async decrease(product: Product): Promise<void> {

    const cart = this.cartItems.find(c => c.id === product.productId);
    if (!cart) return;

    if (cart.quantity > 1) {
      await this.cartService.updateQuantity(cart, cart.quantity - 1);
    } else {
      await this.cartService.removeFromCart(cart);
    }
  }

  /* ================= BUY NOW ================= */

  async buyNow(product: Product): Promise<void> {

    await this.cartService.addToCart(product, 1);

    this.cartService.setCheckoutItems([{
      id: product.productId,
      name: product.name,
      price: product.price,
      imageUrl: product.imageUrl,
      quantity: 1,
      selected: true
    }]);

    this.router.navigate(['/checkout']);
  }
}
