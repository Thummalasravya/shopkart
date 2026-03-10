import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { OrderService } from '../../services/order.service';
import { ProductService } from '../../services/product.service';
import { OrderSignalRService } from '../../services/order-signalr.service';

import { Product } from '../../models/product.model';

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.css']
})
export class OrderHistoryComponent implements OnInit {

  orders: any[] = [];
  loading = true;
  error = '';

  constructor(
    private orderService: OrderService,
    private productService: ProductService,
    private router: Router,
    private cd: ChangeDetectorRef,
    private signalR: OrderSignalRService
  ) {}

  ////////////////////////////////////////////////////////
  // INIT
  ////////////////////////////////////////////////////////

  ngOnInit(): void {

    this.loadOrders();

    // Start SignalR
    this.signalR.startConnection();

    // Listen for real-time order updates
    this.signalR.onOrderStatusUpdate((orderId: number, status: string) => {

      const order = this.orders.find(o => o.orderId === orderId);

      if (order) {

        order.status = status;

        console.log("Order updated:", orderId, status);

        this.cd.detectChanges();
      }

    });

  }

  ////////////////////////////////////////////////////////
  // LOAD ORDERS
  ////////////////////////////////////////////////////////

  loadOrders(): void {

    this.loading = true;

    this.orderService.getMyOrders().subscribe({

      next: (data: any[]) => {

        console.log("Orders loaded:", data);

        this.orders = data ?? [];
        this.loading = false;

        this.cd.detectChanges();

      },

      error: (err) => {

        console.error(err);

        this.error = 'Failed to load orders';
        this.loading = false;

        this.cd.detectChanges();

      }

    });

  }

  ////////////////////////////////////////////////////////
  // BUY AGAIN
  ////////////////////////////////////////////////////////

  buyAgain(productId: number): void {

    const product: Product = this.productService.getProductById(productId);

    if (!product) {
      alert('Product not found');
      return;
    }

    this.router.navigate(['/product', product.slug]);

  }

  ////////////////////////////////////////////////////////
  // CLEAR HISTORY
  ////////////////////////////////////////////////////////

  clearHistory(): void {

    if (!confirm('Clear all order history?')) return;

    this.orderService.clearOrders().subscribe(() => {

      this.orders = [];

    });

  }

  ////////////////////////////////////////////////////////
  // DELETE ORDER
  ////////////////////////////////////////////////////////

  deleteOrder(orderId: number): void {

    if (!confirm('Delete this order?')) return;

    this.orderService.deleteOrder(orderId).subscribe(() => {

      this.orders = this.orders.filter(o => o.orderId !== orderId);

    });

  }

}