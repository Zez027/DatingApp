import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const userString = localStorage.getItem('user');
    if (!userString) return next.handle(request);
    
    const user = JSON.parse(userString);

    // Clone the request to add the new headers
    const authRequest = request.clone({
      setHeaders: {
        Authorization: `Bearer ${user.token}` // Adiciona o token JWT
      }
    });

    // Pass the request to the next handler in the chain
    return next.handle(authRequest);
  }
}
