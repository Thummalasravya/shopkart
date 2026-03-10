import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

import { AddressService } from '../../services/address.service';
import { CartService, CartItem } from '../../services/cart.service';

declare var paypal: any;

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {

  addresses: any[] = [];
  selectedAddressId!: number;

  checkoutItems: CartItem[] = [];
  total = 0;

  paymentMethod = 'COD';

  showAddressForm = false;
  editingAddressId: number | null = null;

  addressError = '';

  paypalScriptLoaded = false;

  newAddress: any = {
    addressTitle: '',
    fullName: '',
    phone: '',
    addressLine: '',
    city: '',
    pincode: ''
  };

  constructor(
    private addressService: AddressService,
    private cartService: CartService,
    private router: Router,
    private http: HttpClient
  ) {}

  ngOnInit(): void {

    this.checkoutItems = this.cartService.getCheckoutItems();

    if (this.checkoutItems.length > 0) {
      this.total = this.cartService.getTotal(this.checkoutItems);
    }

    this.loadAddresses();
    this.loadPaypalScript();
  }

  /* ================= PAYMENT CHANGE ================= */

  onPaymentChange() {

    if (this.paymentMethod === 'PAYPAL') {

      setTimeout(() => {

        const container = document.getElementById('paypal-button-container');

        if (container) container.innerHTML = '';

        this.renderPaypalButton();

      }, 300);

    }

  }

  /* ================= PAYPAL ================= */

  loadPaypalScript() {

    if (this.paypalScriptLoaded) return;

    const script = document.createElement('script');

    script.src =
      "https://www.paypal.com/sdk/js?client-id=AesXfO3cZv1VIpMENL7_Xh30Eg01yJnd-K8OhXDtbacEOjWzlDTCbAl-vboYvRXgrSgNsc_B7Q2X3Ce_&currency=USD";

    script.onload = () => {
      this.paypalScriptLoaded = true;
    };

    document.body.appendChild(script);
  }

  renderPaypalButton() {

    /* Convert INR → USD */

    const usdAmount = (this.total / 83).toFixed(2);

    paypal.Buttons({

      createOrder: (data: any, actions: any) => {

        return actions.order.create({
          purchase_units: [{
            amount: {
              value: usdAmount
            }
          }]
        });

      },

      onApprove: (data: any, actions: any) => {

        return actions.order.capture().then(() => {

          alert("Payment Successful");

          this.placeOrder();

        });

      },

      onError: (err: any) => {

        console.log(err);
        alert("Payment Failed");

      }

    }).render('#paypal-button-container');

  }

  /* ================= PINCODE ================= */

  lookupPincode() {

    const pin = this.newAddress.pincode;

    if (!pin || pin.length !== 6) return;

    this.http
      .get<any[]>(`http://localhost:5273/api/pincode/${pin}`)
      .subscribe(res => {

        if (res[0].Status === 'Success') {

          const postOffice = res[0].PostOffice[0];
          this.newAddress.city = postOffice.District;

        } else {

          this.addressError = 'Invalid pincode';

        }

      });

  }

  /* ================= ADDRESS ================= */

  loadAddresses() {

    this.addressService.getAddresses()
      .subscribe((data: any[]) => {

        this.addresses = data;

        if (!this.addresses.length) return;

        const defaultAddr = this.addresses.find(a => a.isDefault);

        this.selectedAddressId =
          defaultAddr?.id ?? this.addresses[0].id;

      });

  }

  editAddress(addr: any) {

    this.showAddressForm = true;
    this.editingAddressId = addr.id;
    this.newAddress = { ...addr };

  }

  saveAddress() {

    if (!this.newAddress.addressTitle ||
        !this.newAddress.fullName ||
        !this.newAddress.phone ||
        !this.newAddress.addressLine ||
        !this.newAddress.city ||
        !this.newAddress.pincode) {

      this.addressError = 'All fields are required';
      return;
    }

    const request =
      this.editingAddressId
        ? this.addressService.updateAddress(this.editingAddressId, this.newAddress)
        : this.addressService.addAddress(this.newAddress);

    request.subscribe(() => {

      this.editingAddressId = null;
      this.showAddressForm = false;
      this.resetForm();
      this.loadAddresses();

    });

  }

  resetForm() {

    this.newAddress = {
      addressTitle: '',
      fullName: '',
      phone: '',
      addressLine: '',
      city: '',
      pincode: ''
    };

  }

  setDefault(id: number) {
    this.addressService.setDefault(id)
      .subscribe(() => this.loadAddresses());
  }

  deleteAddress(id: number) {
    this.addressService.deleteAddress(id)
      .subscribe(() => this.loadAddresses());
  }

  /* ================= ORDER ================= */

  placeOrder() {

    if (!this.selectedAddressId) {
      alert('Select address first');
      return;
    }

    this.cartService.placeOrder(this.selectedAddressId)
      .subscribe(() => {

        this.cartService.clearAfterOrder();
        this.router.navigate(['/order-success']);

      });

  }

}