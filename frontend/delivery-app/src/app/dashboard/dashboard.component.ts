import { Component } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../core/services/auth.service';
import { inject } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatCardModule, MatIconModule, MatButtonModule],
  template: `
    <div style="display:flex; justify-content:center; align-items:center; height:100vh; background:#f0f2f5;">
      <mat-card style="padding:32px; text-align:center; min-width:320px;">
        <mat-icon style="font-size:48px; width:48px; height:48px; color:#1565c0;">check_circle</mat-icon>
        <h2 style="margin:16px 0 8px;">Login realizado com sucesso!</h2>
        <p style="color:#666; margin-bottom:24px;">Dashboard em construção...</p>
        <button mat-stroked-button (click)="auth.logout()">Sair</button>
      </mat-card>
    </div>
  `,
})
export class DashboardComponent {
  auth = inject(AuthService);
}
