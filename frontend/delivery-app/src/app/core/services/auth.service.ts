import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, EMPTY, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

interface LoginResponse { token: string; expiresAt: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http   = inject(HttpClient);
  private router = inject(Router);

  readonly token   = signal<string | null>(localStorage.getItem('token'));
  readonly loading = signal(false);
  readonly error   = signal<string | null>(null);

  readonly isAuthenticated = computed(() => !!this.token());

  login(email: string, password: string) {
    this.loading.set(true);
    this.error.set(null);

    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, { email, password }).pipe(
      tap(res => {
        localStorage.setItem('token', res.token);
        this.token.set(res.token);
        this.loading.set(false);
        this.router.navigate(['/dashboard']);
      }),
      catchError(err => {
        this.error.set(err.error?.message ?? 'Credenciais inválidas.');
        this.loading.set(false);
        return EMPTY;
      })
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.token.set(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return this.token();
  }
}
