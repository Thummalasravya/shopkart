import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AddressService } from '../../services/address.service';

@Component({
  selector: 'app-address',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.css']
})
export class AddressComponent implements OnInit {

  addresses: any[] = [];

  showForm = false;
  editingId: number | null = null;

  newAddress: any = {
    addressTitle: '',
    fullName: '',
    phone: '',
    addressLine: '',
    city: '',
    pincode: '',
    isDefault: false
  };

  constructor(private addressService: AddressService) {}

  ngOnInit(): void {
    this.loadAddresses();
  }

  // 🔥 Load addresses
  loadAddresses(): void {
    this.addressService.getAddresses().subscribe({
      next: data => this.addresses = data,
      error: err => console.error(err)
    });
  }

  // 🔥 Show / hide form
  toggleForm(): void {
    this.showForm = !this.showForm;
    if (!this.showForm) this.resetForm();
  }

  // 🔥 Save or update
  saveAddress(): void {
    if (this.editingId) {
      this.addressService.updateAddress(this.editingId, this.newAddress)
        .subscribe(() => {
          this.loadAddresses();
          this.resetForm();
        });
    } else {
      this.addressService.addAddress(this.newAddress)
        .subscribe(() => {
          this.loadAddresses();
          this.resetForm();
        });
    }
  }

  // 🔥 Edit
  editAddress(addr: any): void {
    this.showForm = true;
    this.editingId = addr.id;
    this.newAddress = { ...addr };
  }

  // 🔥 Delete
  deleteAddress(id: number): void {
    this.addressService.deleteAddress(id)
      .subscribe(() => this.loadAddresses());
  }

  // 🔥 Set default
  setDefault(id: number): void {
    this.addressService.setDefault(id)
      .subscribe(() => this.loadAddresses());
  }

  // 🔥 Reset
  resetForm(): void {
    this.showForm = false;
    this.editingId = null;
    this.newAddress = {
      addressTitle: '',
      fullName: '',
      phone: '',
      addressLine: '',
      city: '',
      pincode: '',
      isDefault: false
    };
  }
}
