import { Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NotificationService } from '../core/services/notification.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [MatIconModule, MatButtonModule, MatProgressSpinnerModule, MatTooltipModule, DatePipe],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss',
})
export class NotificationsComponent implements OnInit {
  notifSvc = inject(NotificationService);

  readonly notifications = this.notifSvc.notifications;
  readonly loading       = this.notifSvc.loading;
  readonly unreadCount   = this.notifSvc.unreadCount;

  ngOnInit() {
    this.notifSvc.load().subscribe();
  }

  markAsRead(id: string) {
    this.notifSvc.markAsRead(id).subscribe();
  }

  markAllAsRead() {
    this.notifications()
      .filter(n => !n.isRead)
      .forEach(n => this.notifSvc.markAsRead(n.id).subscribe());
  }

  iconFor(type: string): string {
    return type === 'ORDER_CREATED' ? 'receipt_long' : 'local_shipping';
  }
}
