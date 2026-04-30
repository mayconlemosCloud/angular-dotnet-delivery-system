import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { AuthService } from '../core/services/auth.service';
import { NotificationService } from '../core/services/notification.service';
import { SignalRService } from '../core/services/signalr.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink, RouterLinkActive,
    MatIconModule, MatTooltipModule, MatBadgeModule,
  ],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
})
export class ShellComponent implements OnInit, OnDestroy {
  private auth        = inject(AuthService);
  private signalR     = inject(SignalRService);
  private notifSvc    = inject(NotificationService);
  private breakpoints = inject(BreakpointObserver);

  readonly navItems: NavItem[] = [
    { label: 'Dashboard',    icon: 'dashboard',      route: '/dashboard' },
    { label: 'Pedidos',      icon: 'receipt_long',   route: '/orders' },
    { label: 'Entregas',     icon: 'local_shipping', route: '/deliveries' },
    { label: 'Notificações', icon: 'notifications',  route: '/notifications' },
  ];

  readonly sidenavOpen  = signal(true);
  readonly unreadCount  = this.notifSvc.unreadCount;

  readonly userInitial = computed(() => {
    const token = this.auth.getToken();
    if (!token) return 'U';
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const name: string = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
        ?? payload['name']
        ?? payload['email']
        ?? 'U';
      return name.charAt(0).toUpperCase();
    } catch {
      return 'U';
    }
  });

  constructor() {
    this.breakpoints.observe([Breakpoints.Handset]).subscribe(result => {
      this.sidenavOpen.set(!result.matches);
    });
  }

  async ngOnInit() {
    await this.signalR.start();
    this.notifSvc.load().subscribe();
  }

  async ngOnDestroy() {
    await this.signalR.stop();
  }

  toggleSidenav() {
    this.sidenavOpen.update(v => !v);
  }

  logout() {
    this.signalR.stop();
    this.auth.logout();
  }
}
