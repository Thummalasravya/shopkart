import { CartItem } from './cart-item.model';

export interface Order {
  id: number;
  items: CartItem[];
  total: number;
  paymentMethod: string;
  address: any;
  orderDate: Date;
}
