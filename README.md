# 🛒 ShopKart – Full Stack E-Commerce Application

ShopKart is a full-stack e-commerce platform built using **Angular** for the frontend and **ASP.NET Core Web API** for the backend.
The application allows users to browse products, manage carts, place orders, and track order status in real-time.

This project demonstrates modern **full-stack development practices**, including REST APIs, JWT authentication, real-time communication using SignalR, and email notifications.

---

# 🚀 Features

### User Features

* User Registration & Login
* Secure JWT Authentication
* Browse Products
* Product Categories
* Shopping Cart Management
* Wishlist
* Order Placement
* Order History
* Real-time Order Status Tracking
* Email Notifications for Orders

### Admin Features

* Product Management
* Category Management
* Order Management
* User Management

---

# 🧰 Tech Stack

## Frontend

* Angular
* TypeScript
* HTML5
* CSS3

## Backend

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server

## Other Technologies

* JWT Authentication
* SignalR (Real-time updates)
* Mailtrap Email Service
* REST API Architecture

---

# 🏗️ System Architecture

Frontend (Angular)
⬇
ASP.NET Core Web API
⬇
Entity Framework Core
⬇
SQL Server Database

Additional services:

* Authentication → JWT
* Real-time updates → SignalR
* Email notifications → Mailtrap

---

# 📂 Project Structure

```
ShopKart
│
├── EcommerceAPI           # ASP.NET Core Web API
│   ├── Controllers
│   ├── Models
│   ├── Services
│   ├── Data
│   └── Program.cs
│
├── Angular Frontend       # Angular Application
│   ├── Components
│   ├── Services
│   ├── Models
│   └── Pages
│
└── Database               # SQL Server Database
```

---

# 🔐 Authentication

Authentication is implemented using **JWT (JSON Web Tokens)**.

Workflow:

1. User logs in
2. Server generates JWT token
3. Token stored in browser
4. Token sent with each API request
5. Backend validates the token

---

# 📡 Real-Time Features

Real-time order updates are implemented using **SignalR**.

Order Status Flow:

```
Placed → Processing → Shipped → Delivered
```

Users can see order status updates instantly without refreshing the page.

---

# 📧 Email Notifications

The system sends order confirmation emails using **Mailtrap SMTP service**.

Emails are sent when:

* Order is successfully placed
* Order confirmation is generated

---

# 💳 Payment Integration

The system is designed to support payment gateway integration such as:

* Razorpay
* Paypal

(Currently configured for development/testing)

---

# ⚙️ How to Run the Project

## 1️⃣ Backend – ASP.NET Core API

Navigate to the API folder:

```
cd EcommerceAPI
```

Run the API:

```
dotnet run
```

API will run at:

```
http://localhost:5273
```

---

## 2️⃣ Frontend – Angular

Navigate to Angular project:

```
cd AngularProject
```

Install dependencies:

```
npm install
```

Run the Angular app:

```
ng serve
```

Frontend will run at:

```
http://localhost:4200
```

---

# 🗄️ Database

Database used:

```
SQL Server
```

Entity Framework Core is used for:

* Migrations
* Database management
* ORM mapping

---

# 📸 Screenshots

You can add screenshots here later.

Example:

```
Home Page
Product Listing
Cart Page
Order History
Admin Dashboard
```

---

# 🎯 Learning Objectives

This project demonstrates knowledge of:

* Full Stack Development
* RESTful API Design
* Angular Frontend Development
* ASP.NET Core Backend Development
* Authentication & Authorization
* Real-time communication using SignalR
* Email service integration

---

# 👩‍💻 Author

**Sravya Thummala**

Full Stack Developer
Experienced in Angular, .NET Core, and SQL Server

---

# ⭐ If you like this project

Give it a ⭐ on GitHub!
