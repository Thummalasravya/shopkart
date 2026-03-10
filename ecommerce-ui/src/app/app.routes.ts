import { Routes } from '@angular/router';

// 🏠 Home / Products
import { ProductListComponent } from './pages/product-list/product-list.component';
import { ProductDetailsComponent } from './pages/product-details/product-details.component';

// 🔐 Auth
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';

// 👤 User
import { ProfileComponent } from './pages/profile/profile.component';
import { OrderHistoryComponent } from './pages/order-history/order-history.component';

// 🛒 Shopping
import { CartComponent } from './pages/cart/cart.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { WishlistComponent } from './pages/wishlist/wishlist.component';
import { OrderSuccessComponent } from './pages/order-success/order-success.component';

// 📍 Address
import { AddressComponent } from './pages/address/address.component';

// 🔒 Guard
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [

  // 🏠 Home
  { path: '', component: ProductListComponent },

  // 📦 Product details (🔥 FORCE RELOAD)
  {
    path: 'product/:slug',
    component: ProductDetailsComponent,
    runGuardsAndResolvers: 'always'
  },

  // 🔐 Authentication
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },

  // 👤 Profile
  { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },

  // 📍 Address management
  { path: 'addresses', component: AddressComponent, canActivate: [AuthGuard] },

  // 🛒 Protected routes
  { path: 'cart', component: CartComponent, canActivate: [AuthGuard] },
  { path: 'checkout', component: CheckoutComponent, canActivate: [AuthGuard] },
  { path: 'wishlist', component: WishlistComponent, canActivate: [AuthGuard] },
  { path: 'order-history', component: OrderHistoryComponent, canActivate: [AuthGuard] },

  // ✅ Order success
  { path: 'order-success', component: OrderSuccessComponent, canActivate: [AuthGuard] },

  // ❌ Fallback
  { path: '**', redirectTo: '' }
];
