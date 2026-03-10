import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class OrderSignalRService {

  private hubConnection!: signalR.HubConnection;

  startConnection() {

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5273/orderHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(err => console.log('SignalR error:', err));
  }

  onOrderStatusUpdate(callback: (orderId: number, status: string) => void) {

    this.hubConnection.on(
      'OrderStatusUpdated',
      (orderId: number, status: string) => {

        console.log("Order updated:", orderId, status);

        callback(orderId, status);

      }
    );
  }

}