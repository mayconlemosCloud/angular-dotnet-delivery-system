import { Component, computed, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { OrderService } from '../core/services/order.service';
import { DeliveryService } from '../core/services/delivery.service';
import { NotificationService } from '../core/services/notification.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatIconModule, MatButtonModule, RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private orderSvc    = inject(OrderService);
  private deliverySvc = inject(DeliveryService);
  private notifSvc    = inject(NotificationService);

  readonly orders        = this.orderSvc.orders;
  readonly deliveries    = this.deliverySvc.deliveries;
  readonly unreadCount   = this.notifSvc.unreadCount;
  readonly loading       = this.orderSvc.loading;

  readonly todayDeliveries = computed(() => {
    const today = new Date().toDateString();
    return this.deliveries().filter(
      d => new Date(d.deliveryDateTime).toDateString() === today
    ).length;
  });

  readonly recentOrders = computed(() => this.orders().slice(0, 5));

  ngOnInit() {
    forkJoin([
      this.orderSvc.load(),
      this.deliverySvc.load(),
      this.notifSvc.load(),
    ]).subscribe();
  }
}
