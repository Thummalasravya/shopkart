import { Injectable, NgZone } from '@angular/core';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

export interface CartItem {
  id: number;
  cartId?: number;
  name: string;
  price: number;
  imageUrl?: string;
  quantity: number;
  selected: boolean;
}

@Injectable({ providedIn: 'root' })
export class CartService {

  private CART_API = 'http://localhost:5273/api/Cart';
  private PRODUCT_API = 'http://localhost:5273/api/Products';
  private ORDER_API = 'http://localhost:5273/api/Orders';

  private cartSubject = new BehaviorSubject<CartItem[]>([]);
  cart$ = this.cartSubject.asObservable().pipe(shareReplay(1));

  cartCount$ = this.cart$.pipe(
    map(items => items.reduce((s, i) => s + i.quantity, 0)),
    shareReplay(1)
  );

  private checkoutItems: CartItem[] = [];

  constructor(
    private zone: NgZone,
    private http: HttpClient,
    private router: Router,
    private auth: AuthService
  ) {
    this.auth.loggedIn$.subscribe(isLogged => {
      if (isLogged) this.loadCartFromBackend();
      else this.updateCart([]);
    });

    this.loadCartFromBackend();
  }

  /* ================= AUTH ================= */

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');

    if (!token) {
      this.router.navigate(['/login']);
      throw new Error('No token');
    }

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  private requireLogin(): boolean {
    if (localStorage.getItem('token')) return true;
    this.router.navigate(['/login']);
    return false;
  }

  /* ================= STREAM ================= */

  private updateCart(cart: CartItem[]) {
    const copy = [...cart];
    this.zone.run(() => this.cartSubject.next(copy));
  }

  /* ================= LOAD CART ================= */

  async loadCartFromBackend(): Promise<void> {

    const token = localStorage.getItem('token');
    if (!token) return;

    try {

      const carts = await firstValueFrom(
        this.http.get<any[]>(this.CART_API, { headers: this.getHeaders() })
      );

      const productResponse = await firstValueFrom(
        this.http.get<any>(this.PRODUCT_API)
      );

      const products = productResponse.data;

      const mapped: CartItem[] = carts
        .map((c: any) => {
          const product = products.find(
            (p: any) => p.productId === c.productId
          );

          if (!product) return null;

          return {
            cartId: c.cartId,
            id: c.productId,
            name: product.name,
            price: product.price,
            imageUrl: product.imageUrl,
            quantity: c.quantity,
            selected: c.isSelected
          };
        })
        .filter(Boolean) as CartItem[];

      this.updateCart(mapped);

    } catch (err) {
      console.error('Cart sync failed:', err);
    }
  }

  /* ================= ADD ================= */

  async addToCart(product: any, qty = 1): Promise<void> {

    if (!this.requireLogin()) return;

    const productId = product.productId ?? product.id;

    await firstValueFrom(
      this.http.post(this.CART_API, {
        productId,
        quantity: qty
      }, { headers: this.getHeaders() })
    );

    await this.loadCartFromBackend();
  }

  /* ================= UPDATE ================= */

  async updateQuantity(cart: CartItem, quantity: number): Promise<void> {
    if (!cart.cartId) return;

    await firstValueFrom(
      this.http.put(`${this.CART_API}/${cart.cartId}`, {
        quantity,
        isSelected: cart.selected,
        isWishlisted: false
      }, { headers: this.getHeaders() })
    );

    await this.loadCartFromBackend();
  }

  async toggleSelect(cart: CartItem, selected: boolean): Promise<void> {
    if (!cart.cartId) return;

    await firstValueFrom(
      this.http.put(`${this.CART_API}/${cart.cartId}`, {
        quantity: cart.quantity,
        isSelected: selected,
        isWishlisted: false
      }, { headers: this.getHeaders() })
    );

    await this.loadCartFromBackend();
  }

  async removeFromCart(cart: CartItem): Promise<void> {
    if (!cart.cartId) return;

    await firstValueFrom(
      this.http.delete(`${this.CART_API}/${cart.cartId}`, {
        headers: this.getHeaders()
      })
    );

    await this.loadCartFromBackend();
  }

  /* ================= TOTAL ================= */

  getTotal(items: CartItem[]): number {
    return items.reduce((s, i) => s + i.price * i.quantity, 0);
  }

  /* ================= CHECKOUT ================= */

  setCheckoutItems(items: CartItem[]) {
    this.checkoutItems = items;
  }

  getCheckoutItems(): CartItem[] {
    return this.checkoutItems;
  }

  clearCheckoutItems() {
    this.checkoutItems = [];
  }

  /* ================= ORDER ================= */

  placeOrder(addressId: number) {
    return this.http.post(
      `${this.ORDER_API}/place`,
      { addressId },
      { headers: this.getHeaders() }
    );
  }

  clearAfterOrder() {
    this.updateCart([]);
    this.clearCheckoutItems();
  }

  getOrdersFromBackend(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.ORDER_API}/my`,
      { headers: this.getHeaders() }
    );
  }

  clearOrdersFromBackend(): Observable<any> {
    return this.http.delete(
      `${this.ORDER_API}/clear`,
      { headers: this.getHeaders() }
    );
  }
}
