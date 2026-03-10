using Microsoft.AspNetCore.SignalR;

namespace ECommerceAPI.Hubs
{
    public class OrderHub : Hub
    {
        // Called when order status changes
        public async Task SendOrderStatus(int orderId, string status)
        {
            await Clients.All.SendAsync("OrderStatusUpdated", orderId, status);
        }
    }
}