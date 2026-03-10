import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-success',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-success.component.html',
  styleUrls: ['./order-success.component.css']
})
export class OrderSuccessComponent {

  constructor(private router: Router) {}

  continueShopping(): void {
    this.router.navigate(['/']);
  }

  viewOrders(): void {
    this.router.navigate(['/orders-history']); // EXACT route
  }
}
