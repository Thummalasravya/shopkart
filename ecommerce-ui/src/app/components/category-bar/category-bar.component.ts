import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-category-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category-bar.component.html',
  styleUrls: ['./category-bar.component.css']
})
export class CategoryBarComponent {

  categories: string[] = ['All', 'Electronics', 'Fashion', 'Home&Furniture', 'Appliances', 'Beauty'];

  constructor(private productService: ProductService) {}

  selectCategory(category: string) {
    this.productService.filterByCategory(category);
  }
}
