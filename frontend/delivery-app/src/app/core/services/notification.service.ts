import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, EMPTY, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface Notification {
  id: string;
  type: 'ORDER_CREATED' | 'DELIVERY_REGISTERED';
  message: string;
  isRead: boolean;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private http = inject(HttpClient);

  readonly notifications = signal<Notification[]>([]);
  readonly loading       = signal(false);

  readonly unreadCount = computed(
    () => this.notifications().filter(n => !n.isRead).length
  );

  load() {
    this.loading.set(true);
    return this.http.get<Notification[]>(`${environment.apiUrl}/notifications`).pipe(
      tap(list => {
        this.notifications.set(list);
        this.loading.set(false);
      }),
      catchError(() => {
        this.loading.set(false);
        return EMPTY;
      })
    );
  }

  addRealTime(notification: Notification) {
    this.notifications.update(list => [notification, ...list]);
  }

  markAsRead(id: string) {
    return this.http.patch<void>(`${environment.apiUrl}/notifications/${id}/read`, {}).pipe(
      tap(() =>
        this.notifications.update(list =>
          list.map(n => n.id === id ? { ...n, isRead: true } : n)
        )
      ),
      catchError(() => EMPTY)
    );
  }
}
