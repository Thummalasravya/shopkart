import { Injectable } from '@angular/core';
import { Product } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private products: Product[] = [

    {
      productId: 1,
      slug: 'dell-01',
      name: 'Dell Laptop',
      description: 'High-performance laptop suitable for office work, programming, and multimedia usage.',
      price: 58000,
      category: 'Electronics',
      rating: 4,
      imageUrl: 'assets/dell-laptop.png'
    },

    {
      productId: 2,
      slug: 'samsung-02',
      name: 'Samsung Mobile',
      description: 'Smartphone with powerful processor, excellent camera quality, and long battery life.',
      price: 42000,
      category: 'Electronics',
      rating: 5,
      imageUrl: 'assets/samsung-s21.png'
    },

    {
      productId: 3,
      slug: 'headphones-03',
      name: 'Headphones',
      description: 'Noise-isolating headphones delivering clear sound and deep bass for music and calls.',
      price: 9000,
      category: 'Electronics',
      rating: 4,
      imageUrl: 'assets/sony-headphones.png'
    },

    {
      productId: 4,
      slug: 'shirt-04',
      name: 'Men Shirt',
      description: 'Comfortable cotton shirt suitable for office wear and casual outings.',
      price: 1499,
      category: 'Fashion',
      rating: 4,
      imageUrl: 'assets/shirt.png'
    },

    {
      productId: 5,
      slug: 'dress-05',
      name: 'Women Dress',
      description: 'Stylish and elegant dress perfect for parties, events, and festive occasions.',
      price: 2199,
      category: 'Fashion',
      rating: 5,
      imageUrl: 'assets/women-dress.png'
    },

    {
      productId: 6,
      slug: 'sofa-06',
      name: 'Sofa',
      description: 'Comfortable and spacious sofa designed for relaxing and enhancing living room decor.',
      price: 32000,
      category: 'Home&Furniture',
      rating: 4,
      imageUrl: 'assets/sofa.png'
    },

    {
      productId: 7,
      slug: 'table-07',
      name: 'Study Table',
      description: 'Strong wooden study table ideal for students and work-from-home setups.',
      price: 12000,
      category: 'Home&Furniture',
      rating: 4,
      imageUrl: 'assets/study-table.png'
    },

    {
      productId: 8,
      slug: 'washing-08',
      name: 'Washing Machine',
      description: 'Fully automatic washing machine with multiple wash programs and energy efficiency.',
      price: 28000,
      category: 'Appliances',
      rating: 5,
      imageUrl: 'assets/washing-machine.png'
    },

    {
      productId: 9,
      slug: 'fridge-09',
      name: 'Refrigerator',
      description: 'Spacious refrigerator with advanced cooling technology to keep food fresh longer.',
      price: 40000,
      category: 'Appliances',
      rating: 5,
      imageUrl: 'assets/fridge.png'
    },

    {
      productId: 10,
      slug: 'cream-10',
      name: 'Face Cream',
      description: 'Moisturizing face cream that nourishes skin and provides a healthy glow.',
      price: 799,
      category: 'Beauty',
      rating: 4,
      imageUrl: 'assets/face-cream.png'
    },

    {
      productId: 11,
      slug: 'lipstick-11',
      name: 'Lipstick',
      description: 'Long-lasting lipstick with rich color and smooth matte finish.',
      price: 499,
      category: 'Beauty',
      rating: 4,
      imageUrl: 'assets/lipstick.png'
    }
  ];

  filteredProducts: Product[] = [];

  constructor() {
    this.filteredProducts = [...this.products];
  }

  getProducts(): Product[] {
    return this.products;
  }

  getFilteredProducts(): Product[] {
    return this.filteredProducts;
  }

  getProductById(id: number): Product {
    return this.products.find(p => p.productId === id)!;
  }

  // ✅ NEW
  getProductBySlug(slug: string): Product {
    return this.products.find(p => p.slug === slug)!;
  }

  filterByCategory(category: string): void {
    this.filteredProducts =
      category === 'All'
        ? [...this.products]
        : this.products.filter(p => p.category === category);
  }
}
