import { Injectable, signal } from '@angular/core';

export interface CourierLocation {
  orderNumber: string;
  latitude: number;
  longitude: number;
  progress: number;
  estimatedMinutes: number;
  isCompleted: boolean;
}

@Injectable({ providedIn: 'root' })
export class TrackingService {
  readonly locations = signal<Map<string, CourierLocation>>(new Map());

  update(location: CourierLocation) {
    this.locations.update(map => {
      const next = new Map(map);
      next.set(location.orderNumber, location);
      return next;
    });
  }

  get(orderNumber: string): CourierLocation | undefined {
    return this.locations().get(orderNumber);
  }
}
