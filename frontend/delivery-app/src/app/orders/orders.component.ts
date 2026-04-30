import { Component, computed, inject, OnInit } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { forkJoin } from 'rxjs';
import { OrderService } from '../core/services/order.service';
import { DeliveryService } from '../core/services/delivery.service';
import { NewOrderDialogComponent } from './new-order-dialog.component';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [MatIconModule, MatButtonModule, MatProgressSpinnerModule, MatTooltipModule, CurrencyPipe, DatePipe],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss',
})
export class OrdersComponent implements OnInit {
  private dialog      = inject(MatDialog);
  orderSvc            = inject(OrderService);
  deliverySvc         = inject(DeliveryService);

  readonly orders     = this.orderSvc.orders;
  readonly loading    = this.orderSvc.loading;

  // Números de pedidos que já têm entrega registrada
  private deliveredOrderNumbers = computed(() =>
    new Set(this.deliverySvc.deliveries().map(d => d.orderNumber))
  );

  readonly pendingOrders = computed(() =>
    this.orders().filter(o => !this.deliveredOrderNumbers().has(o.orderNumber))
  );

  readonly completedOrders = computed(() =>
    this.orders().filter(o => this.deliveredOrderNumbers().has(o.orderNumber))
  );

  ngOnInit() {
    forkJoin([
      this.orderSvc.load(),
      this.deliverySvc.load(),
    ]).subscribe();
  }

  openNew() {
    this.dialog.open(NewOrderDialogComponent, { width: '480px', disableClose: true });
  }

  hasDelivery(orderNumber: string): boolean {
    return this.deliveredOrderNumbers().has(orderNumber);
  }
}
