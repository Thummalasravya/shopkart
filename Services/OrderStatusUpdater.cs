using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Hubs;

namespace ECommerceAPI.Services
{
    public class OrderStatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderStatusUpdater> _logger;
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderStatusUpdater(
            IServiceScopeFactory scopeFactory,
            ILogger<OrderStatusUpdater> logger,
            IHubContext<OrderHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Order Status Background Service Started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateOrderStatuses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating order statuses");
                }

                // check every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task UpdateOrderStatuses()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ECommerceDbContext>();

            var orders = await context.Orders.ToListAsync();

            foreach (var order in orders)
            {
                var hours = (DateTime.UtcNow - order.CreatedAt).TotalHours;

                if (order.Status == "Placed" && hours >= 1)
                {
                    order.Status = "Processing";

                    await _hubContext.Clients.All.SendAsync(
                        "OrderStatusUpdated",
                        order.OrderId,
                        order.Status);

                    _logger.LogInformation($"Order {order.OrderId} → Processing");
                }
                else if (order.Status == "Processing" && hours >= 5)
                {
                    order.Status = "Shipped";

                    await _hubContext.Clients.All.SendAsync(
                        "OrderStatusUpdated",
                        order.OrderId,
                        order.Status);

                    _logger.LogInformation($"Order {order.OrderId} → Shipped");
                }
                else if (order.Status == "Shipped" && hours >= 24)
                {
                    order.Status = "Delivered";

                    await _hubContext.Clients.All.SendAsync(
                        "OrderStatusUpdated",
                        order.OrderId,
                        order.Status);

                    _logger.LogInformation($"Order {order.OrderId} → Delivered");
                }
            }

            await context.SaveChangesAsync();
        }
    }
}