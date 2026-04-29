import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../core/services/auth.service';

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
    MatIconModule, MatTooltipModule,
  ],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
})
export class ShellComponent {
  private auth        = inject(AuthService);
  private breakpoints = inject(BreakpointObserver);

  readonly navItems: NavItem[] = [
    { label: 'Dashboard',    icon: 'dashboard',      route: '/dashboard' },
    { label: 'Pedidos',      icon: 'receipt_long',   route: '/orders' },
    { label: 'Entregas',     icon: 'local_shipping', route: '/deliveries' },
    { label: 'Notificações', icon: 'notifications',  route: '/notifications' },
  ];

  readonly sidenavOpen = signal(true);

  // Pega a inicial do e-mail armazenado no token (fallback: 'U')
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

  toggleSidenav() {
    this.sidenavOpen.update(v => !v);
  }

  logout() {
    this.auth.logout();
  }
}
