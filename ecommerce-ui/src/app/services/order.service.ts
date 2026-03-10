import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  private apiUrl = 'http://localhost:5273/api/orders';

  constructor(private http: HttpClient) {}

  /* ================= AUTH HEADERS ================= */

  private authHeaders(): HttpHeaders {

    const token = localStorage.getItem('token');

    if (!token) {
      console.warn('No token found');
      return new HttpHeaders();
    }

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  /* ================= GET ORDERS ================= */

  getMyOrders(): Observable<any[]> {

    console.log('Fetching orders...');

    return this.http.get<any[]>(
      `${this.apiUrl}/my`,
      { headers: this.authHeaders() }
    );
  }

  /* ================= DELETE SINGLE ITEM ================= */

  deleteOrderItem(id: number): Observable<any> {

    return this.http.delete(
      `${this.apiUrl}/item/${id}`,
      { headers: this.authHeaders() }
    );
  }

  /* ================= CLEAR ALL ================= */

  clearOrders(): Observable<any> {

    return this.http.delete(
      `${this.apiUrl}/clear`,
      { headers: this.authHeaders() }
    );
  }
deleteOrder(orderId: number) {
  return this.http.delete(
    `${this.apiUrl}/${orderId}`,
    { headers: this.authHeaders() }
  );
}


}
