import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AddressService {

  private apiUrl = 'http://localhost:5273/api/address';

  constructor(private http: HttpClient) {}

  private headers(): HttpHeaders {
    const token = localStorage.getItem('token') || '';
    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }

  getAddresses() {
    return this.http.get<any[]>(this.apiUrl, { headers: this.headers() });
  }

  addAddress(address: any) {
    return this.http.post(this.apiUrl, address, { headers: this.headers() });
  }

  updateAddress(id: number, address: any) {
    return this.http.put(`${this.apiUrl}/${id}`, address, { headers: this.headers() });
  }

  deleteAddress(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.headers() });
  }

  setDefault(id: number) {
    return this.http.post(`${this.apiUrl}/set-default/${id}`, {}, { headers: this.headers() });
  }
}
