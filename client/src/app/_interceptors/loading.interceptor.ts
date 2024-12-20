import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http'
import { Observable, delay, finalize } from 'rxjs';
import { BusyService } from '../_services/busy.service';
import { request } from 'express';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private busyService: BusyService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.busyService.busy();
    
    return next.handle(request).pipe(
      delay(1000),
      finalize(() => {
        this.busyService.idle()
      })
    )
  }
}
