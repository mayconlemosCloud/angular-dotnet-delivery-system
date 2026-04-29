import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, EMPTY, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Address {
  zipCode: string;
  street: string;
  number: string;
  neighborhood: string;
  city: string;
  state: string;
  latitude?: number;
  longitude?: number;
}

export interface Order {
  id: string;
  orderNumber: string;
  description: string;
  value: number;
  deliveryAddress: Address;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);

  readonly orders  = signal<Order[]>([]);
  readonly loading = signal(false);
  readonly error   = signal<string | null>(null);

  load() {
    this.loading.set(true);
    return this.http.get<Order[]>(`${environment.apiUrl}/orders`).pipe(
      tap(orders => {
        this.orders.set(orders);
        this.loading.set(false);
      }),
      catchError(err => {
        this.error.set(err.error?.message ?? 'Erro ao carregar pedidos.');
        this.loading.set(false);
        return EMPTY;
      })
    );
  }

  create(body: { description: string; value: number; deliveryAddress: { zipCode: string; number: string } }) {
    this.loading.set(true);
    this.error.set(null);
    return this.http.post<Order>(`${environment.apiUrl}/orders`, body).pipe(
      tap(order => {
        this.orders.update(list => [order, ...list]);
        this.loading.set(false);
      }),
      catchError(err => {
        this.error.set(err.error?.message ?? 'Erro ao criar pedido.');
        this.loading.set(false);
        return EMPTY;
      })
    );
  }
}
