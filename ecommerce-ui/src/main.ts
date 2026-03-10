import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter, withRouterConfig } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';
import { authInterceptor } from './app/interceptors/auth.interceptor';

bootstrapApplication(AppComponent, {
  providers: [

    provideRouter(
      routes,
      withRouterConfig({ onSameUrlNavigation: 'reload' })
    ),

    provideHttpClient(
      withInterceptors([authInterceptor])
    )

  ]
}).catch(err => console.error(err));