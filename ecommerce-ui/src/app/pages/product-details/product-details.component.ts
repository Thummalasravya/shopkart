import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit {

  product: Product | null = null;
  loading = true;
  error = '';

  quantity = 0; // ⭐ track quantity like home page

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {

    this.route.paramMap.subscribe(params => {

      const slug = params.get('slug');

      if (!slug) {
        this.error = 'Invalid product link';
        this.loading = false;
        return;
      }

      try {
        this.product = this.productService.getProductBySlug(slug);
        this.loading = false;
      } catch {
        this.error = 'Product not found';
        this.loading = false;
      }
    });
  }

  // ✅ ADD
  async addToCart(): Promise<void> {
    if (!this.product) return;

    this.quantity++;

    await this.cartService.addToCart({
      id: this.product.productId,
      name: this.product.name,
      price: this.product.price,
      imageUrl: this.product.imageUrl,
      quantity: 1,
      selected: true
    });
  }

  // ✅ INCREASE
  async increase(): Promise<void> {
    if (!this.product) return;

    this.quantity++;

    await this.cartService.addToCart({
      id: this.product.productId,
      name: this.product.name,
      price: this.product.price,
      imageUrl: this.product.imageUrl,
      quantity: 1,
      selected: true
    });
  }

  // ✅ DECREASE
  async decrease(): Promise<void> {
    if (!this.product || this.quantity <= 0) return;

    this.quantity--;

    if (this.quantity === 0) {
      await this.cartService.removeFromCart({
        id: this.product.productId,
        name: this.product.name,
        price: this.product.price,
        imageUrl: this.product.imageUrl,
        quantity: 1,
        selected: true
      });
    } else {
      await this.cartService.addToCart({
        id: this.product.productId,
        name: this.product.name,
        price: this.product.price,
        imageUrl: this.product.imageUrl,
        quantity: -1,
        selected: true
      });
    }
  }

  // ✅ BUY NOW
  buyNow(): void {
    if (!this.product) return;

    const item = {
      id: this.product.productId,
      name: this.product.name,
      price: this.product.price,
      imageUrl: this.product.imageUrl,
      quantity: 1,
      selected: true
    };

    this.cartService.setCheckoutItems([item]);
    this.router.navigate(['/checkout']);
  }
}
