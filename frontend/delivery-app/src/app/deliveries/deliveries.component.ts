import { Component, inject, OnInit } from '@angular/core';
import { DatePipe, NgClass } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialog } from '@angular/material/dialog';
import { DeliveryService } from '../core/services/delivery.service';
import { TrackingService } from '../core/services/tracking.service';
import { NewDeliveryDialogComponent } from './new-delivery-dialog.component';

@Component({
  selector: 'app-deliveries',
  standalone: true,
  imports: [MatIconModule, MatButtonModule, MatProgressSpinnerModule, MatTooltipModule, DatePipe, NgClass],
  templateUrl: './deliveries.component.html',
  styleUrl: './deliveries.component.scss',
})
export class DeliveriesComponent implements OnInit {
  private dialog = inject(MatDialog);
  deliverySvc    = inject(DeliveryService);
  trackingSvc    = inject(TrackingService);

  readonly deliveries = this.deliverySvc.deliveries;
  readonly loading    = this.deliverySvc.loading;

  ngOnInit() {
    this.deliverySvc.load().subscribe();
  }

  openNew() {
    this.dialog.open(NewDeliveryDialogComponent, { width: '440px', disableClose: true });
  }
}
