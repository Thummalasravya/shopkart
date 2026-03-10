export interface Product {
  productId: number;
  slug: string;  

  name: string;
  description: string;
  price: number;
  category: string;
  imageUrl: string;

  rating?: number;
  brand?: string;
  availability?: boolean;
  createdAt?: string;
}
