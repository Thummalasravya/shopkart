import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  cartCount$!: Observable<number>;

  constructor(
    public auth: AuthService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.cartCount$ = this.cartService.cart$.pipe(
      map(cart => cart.reduce((s, i) => s + i.quantity, 0))
    );
  }

  logout() {
    this.auth.logout();
  }
}
