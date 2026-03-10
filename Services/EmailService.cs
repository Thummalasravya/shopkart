using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using ECommerceAPI.Models;

namespace ECommerceAPI.Services
{
    public class EmailService
    {
        private readonly IWebHostEnvironment _env;

        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _fromEmail;
        private readonly string _displayName;

        public EmailService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;

            _host = configuration["MailSettings:Host"] ?? throw new Exception("Mail Host missing");
            _port = int.Parse(configuration["MailSettings:Port"] ?? "587");
            _username = configuration["MailSettings:UserName"] ?? throw new Exception("Mail username missing");
            _password = configuration["MailSettings:Password"] ?? throw new Exception("Mail password missing");
            _fromEmail = configuration["MailSettings:Email"] ?? throw new Exception("Mail email missing");
            _displayName = configuration["MailSettings:DisplayName"] ?? "ShopKart";
        }

        ////////////////////////////////////////////////////////
        // SEND EMAIL
        ////////////////////////////////////////////////////////

        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtp = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_username, _password),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            smtp.Send(message);
        }

        ////////////////////////////////////////////////////////
        // BUILD ORDER EMAIL TEMPLATE
        ////////////////////////////////////////////////////////

        public string BuildOrderEmailTemplate(Order order)
        {
            StringBuilder itemsHtml = new StringBuilder();

            foreach (var item in order.Items)
            {
                decimal subtotal = item.Price * item.Quantity;

                string imageTag = "https://via.placeholder.com/70";

                if (!string.IsNullOrEmpty(item.Product?.ImageUrl))
                {
                    try
                    {
                        string fullPath = Path.Combine(_env.WebRootPath, item.Product.ImageUrl);

                        if (File.Exists(fullPath))
                        {
                            byte[] bytes = File.ReadAllBytes(fullPath);
                            string base64 = Convert.ToBase64String(bytes);
                            string extension = Path.GetExtension(fullPath).Replace(".", "");

                            imageTag = $"data:image/{extension};base64,{base64}";
                        }
                    }
                    catch { }
                }

                itemsHtml.Append($@"
<tr style='border-bottom:1px solid #eee'>
<td style='padding:12px'>
<img src='{imageTag}' width='70' style='border-radius:6px'/>
</td>

<td style='padding:12px;font-weight:500'>
{item.Product?.Name}
</td>

<td style='padding:12px;text-align:center'>
{item.Quantity}
</td>

<td style='padding:12px'>
₹{item.Price}
</td>

<td style='padding:12px;font-weight:bold'>
₹{subtotal}
</td>
</tr>");
            }

            return $@"

<div style='background:#f4f6f8;padding:40px;font-family:Segoe UI,Arial'>

<div style='max-width:700px;margin:auto;background:white;border-radius:10px;
box-shadow:0 3px 12px rgba(0,0,0,0.08);overflow:hidden'>

<!-- HEADER -->

<div style='background:#2c3e50;color:white;padding:20px'>

<h2 style='margin:0'>🛒 ShopKart</h2>

</div>

<!-- CONTENT -->

<div style='padding:30px'>

<h2 style='margin-top:0;color:#2c3e50'>Order Confirmation</h2>

<p style='font-size:15px'>
Thank you for shopping with <b>ShopKart</b>!  
Your order has been successfully placed.
</p>

<!-- ORDER DETAILS -->

<div style='background:#f7f7f7;padding:15px;border-radius:6px;margin-top:20px'>

<p><b>Order ID:</b> #{order.OrderId}</p>
<p><b>Date:</b> {order.CreatedAt:dd MMM yyyy}</p>
<p><b>Status:</b> {order.Status}</p>

</div>

<br>

<!-- ORDER ITEMS -->

<table width='100%' style='border-collapse:collapse;font-size:14px'>

<tr style='background:#fafafa;border-bottom:2px solid #eee'>

<th align='left' style='padding:12px'>Image</th>
<th align='left'>Product</th>
<th align='center'>Qty</th>
<th align='left'>Price</th>
<th align='left'>Subtotal</th>

</tr>

{itemsHtml}

</table>

<!-- TOTAL -->

<div style='text-align:right;margin-top:25px'>

<h3>Total: ₹{order.TotalAmount}</h3>

</div>

<br>

<!-- TRACK BUTTON -->

<div style='text-align:center;margin-top:25px'>

<a href='http://localhost:4200/orders/{order.OrderId}'
style='background:#27ae60;color:white;
padding:14px 30px;
border-radius:6px;
font-size:16px;
text-decoration:none;
display:inline-block'>

Track Your Order

</a>

</div>

<br><br>

<hr>

<p style='font-size:15px'>
Thank you for choosing <b>ShopKart</b>.  
We hope to serve you again!
</p>

<!-- SUPPORT -->

<div style='background:#f7f7f7;padding:15px;border-radius:6px'>

<p style='margin:0;font-weight:bold'>Customer Support</p>

<p style='margin:5px 0'>
📧 support@shopkart.com
</p>

<p style='margin:5px 0'>
📞 +91 9876543210
</p>

</div>

<br>

<!-- FOOTER -->

<p style='text-align:center;font-size:12px;color:#888'>

© 2026 ShopKart. All rights reserved.

</p>

</div>

</div>

</div>";
        }
    }
}