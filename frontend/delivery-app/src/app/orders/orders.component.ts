import { Component, inject, OnInit } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { OrderService } from '../core/services/order.service';
import { NewOrderDialogComponent } from './new-order-dialog.component';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [
    MatIconModule, MatButtonModule, MatProgressSpinnerModule,
    MatTooltipModule, CurrencyPipe, DatePipe,
  ],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss',
})
export class OrdersComponent implements OnInit {
  private dialog = inject(MatDialog);
  orderSvc       = inject(OrderService);

  readonly orders  = this.orderSvc.orders;
  readonly loading = this.orderSvc.loading;

  ngOnInit() {
    this.orderSvc.load().subscribe();
  }

  openNew() {
    this.dialog.open(NewOrderDialogComponent, { width: '480px', disableClose: true });
  }
}
