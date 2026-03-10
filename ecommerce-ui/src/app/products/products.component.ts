import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './products.component.html'
})
export class ProductsComponent implements OnInit {

  products: any[] = [];
  loading = true;

  constructor(
    private http: HttpClient,
    private cartService: CartService
  ) {}
ngOnInit() {
  this.http.get<any[]>('http://localhost:5273/api/products')
    .subscribe(data => {

      // 🔥 NORMALIZE PRODUCT ID
      this.products = data.map(p => ({
        ...p,
        id: p.productId   // ✅ CRITICAL FIX
      }));

      this.loading = false;
    });
}

 
  addToCart(product: any) {
    this.cartService.addToCart(product);
  }
}
