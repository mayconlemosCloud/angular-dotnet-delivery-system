import { inject, Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from './auth.service';
import { NotificationService, Notification } from './notification.service';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private authSvc  = inject(AuthService);
  private notifSvc = inject(NotificationService);

  readonly state = signal<'disconnected' | 'connecting' | 'connected'>('disconnected');

  private connection: signalR.HubConnection | null = null;

  async start() {
    if (this.connection) return;

    this.state.set('connecting');

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.signalrUrl, {
        accessTokenFactory: () => this.authSvc.getToken() ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.connection.on('ReceiveNotification', (msg: { type: string; message: string; createdAt: string }) => {
      const notification: Notification = {
        id: crypto.randomUUID(),
        type: msg.type as Notification['type'],
        message: msg.message,
        isRead: false,
        createdAt: msg.createdAt,
      };
      this.notifSvc.addRealTime(notification);
    });

    this.connection.onreconnecting(() => this.state.set('connecting'));
    this.connection.onreconnected(() => this.state.set('connected'));
    this.connection.onclose(() => this.state.set('disconnected'));

    try {
      await this.connection.start();
      this.state.set('connected');
    } catch {
      this.state.set('disconnected');
    }
  }

  async stop() {
    await this.connection?.stop();
    this.connection = null;
    this.state.set('disconnected');
  }
}
