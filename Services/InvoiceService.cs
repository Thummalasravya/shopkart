using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using ECommerceAPI.Models;

public class InvoiceService
{
    public byte[] GenerateInvoice(Order order)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text($"ShopKart Invoice - Order #{order.OrderId}")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    col.Item().Text($"Date: {order.CreatedAt}");
                    col.Item().Text($"Status: {order.Status}");

                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Product").Bold();
                            header.Cell().Text("Qty").Bold();
                            header.Cell().Text("Price").Bold();
                            header.Cell().Text("Subtotal").Bold();
                        });

                        foreach (var item in order.Items)
                        {
                            table.Cell().Text(item.Product?.Name);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text($"₹{item.Price}");
                            table.Cell().Text($"₹{item.Price * item.Quantity}");
                        }
                    });

                    col.Item().PaddingTop(20)
                        .Text($"Total: ₹{order.TotalAmount}")
                        .FontSize(16)
                        .Bold();
                });

                page.Footer()
                    .AlignCenter()
                    .Text("Thank you for shopping with ShopKart!");
            });
        });

        return doc.GeneratePdf();
    }
}