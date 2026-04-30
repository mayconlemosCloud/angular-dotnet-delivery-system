import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, EMPTY, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export type DeliveryStatus = 'IN_ROUTE' | 'DELIVERED';

export interface Delivery {
  id: string;
  orderNumber: string;
  deliveryDateTime: string;
  status: DeliveryStatus;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class DeliveryService {
  private http = inject(HttpClient);

  readonly deliveries = signal<Delivery[]>([]);
  readonly loading    = signal(false);
  readonly error      = signal<string | null>(null);

  load() {
    this.loading.set(true);
    return this.http.get<Delivery[]>(`${environment.apiUrl}/deliveries`).pipe(
      tap(deliveries => {
        this.deliveries.set(deliveries);
        this.loading.set(false);
      }),
      catchError(err => {
        this.error.set(err.error?.message ?? 'Erro ao carregar entregas.');
        this.loading.set(false);
        return EMPTY;
      })
    );
  }

  create(body: { orderNumber: string; deliveryDateTime: string }) {
    this.loading.set(true);
    this.error.set(null);
    return this.http.post<Delivery>(`${environment.apiUrl}/deliveries`, body).pipe(
      tap(delivery => {
        this.deliveries.update(list => [delivery, ...list]);
        this.loading.set(false);
      }),
      catchError(err => {
        this.error.set(err.error?.message ?? 'Erro ao registrar entrega.');
        this.loading.set(false);
        return EMPTY;
      })
    );
  }
}
