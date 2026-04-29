import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, EMPTY, tap } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { environment } from '../../../environments/environment';

function strongPassword(control: AbstractControl): ValidationErrors | null {
  const v: string = control.value ?? '';
  if (!/[A-Z]/.test(v)) return { noUppercase: true };
  if (!/[a-z]/.test(v)) return { noLowercase: true };
  if (!/[0-9]/.test(v)) return { noNumber: true };
  return null;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule, RouterLink,
    MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatProgressSpinnerModule,
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private http   = inject(HttpClient);
  private router = inject(Router);
  private fb     = inject(FormBuilder);

  hidePassword = true;
  loading      = signal(false);
  error        = signal<string | null>(null);

  form = this.fb.group({
    name:     ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    email:    ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8), strongPassword]],
  });

  passwordError(): string {
    const ctrl = this.form.get('password');
    if (!ctrl?.touched) return '';
    if (ctrl.hasError('required'))    return 'Senha obrigatória';
    if (ctrl.hasError('minlength'))   return 'Mínimo 8 caracteres';
    if (ctrl.hasError('noUppercase')) return 'Precisa de ao menos 1 letra maiúscula';
    if (ctrl.hasError('noLowercase')) return 'Precisa de ao menos 1 letra minúscula';
    if (ctrl.hasError('noNumber'))    return 'Precisa de ao menos 1 número';
    return '';
  }

  submit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const { name, email, password } = this.form.getRawValue();

    this.http.post(`${environment.apiUrl}/users/register`, { name, email, password }).pipe(
      tap(() => {
        this.loading.set(false);
        this.router.navigate(['/auth/login']);
      }),
      catchError(err => {
        this.error.set(err.error?.message ?? 'Erro ao registrar.');
        this.loading.set(false);
        return EMPTY;
      })
    ).subscribe();
  }
}
