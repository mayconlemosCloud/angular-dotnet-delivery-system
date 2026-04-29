import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { OrderService } from '../core/services/order.service';

interface ViaCepResponse {
  logradouro: string;
  bairro: string;
  localidade: string;
  uf: string;
  erro?: boolean;
}

@Component({
  selector: 'app-new-order-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
  ],
  templateUrl: './new-order-dialog.component.html',
  styleUrl: './new-order-dialog.component.scss',
})
export class NewOrderDialogComponent {
  private fb        = inject(FormBuilder);
  private http      = inject(HttpClient);
  readonly orderSvc = inject(OrderService);
  private dialogRef = inject(MatDialogRef<NewOrderDialogComponent>);

  readonly cepLoading = signal(false);
  readonly cepError   = signal<string | null>(null);
  readonly address    = signal<ViaCepResponse | null>(null);

  form = this.fb.group({
    description: ['', [Validators.required, Validators.minLength(3)]],
    value:       [null as number | null, [Validators.required, Validators.min(0.01)]],
    zipCode:     ['', [Validators.required, Validators.pattern(/^\d{5}-?\d{3}$/)]],
    number:      ['', Validators.required],
  });

  constructor() {
    this.form.get('zipCode')!.valueChanges.pipe(
      debounceTime(600),
      distinctUntilChanged(),
    ).subscribe(cep => this.lookupCep(cep ?? ''));
  }

  private lookupCep(cep: string) {
    const clean = cep.replace(/\D/g, '');
    if (clean.length !== 8) { this.address.set(null); return; }

    this.cepLoading.set(true);
    this.cepError.set(null);

    this.http.get<ViaCepResponse>(`https://viacep.com.br/ws/${clean}/json/`).subscribe({
      next: res => {
        this.cepLoading.set(false);
        if (res.erro) { this.cepError.set('CEP não encontrado.'); this.address.set(null); return; }
        this.address.set(res);
      },
      error: () => {
        this.cepLoading.set(false);
        this.cepError.set('Erro ao buscar CEP.');
      },
    });
  }

  submit() {
    if (this.form.invalid || !this.address()) {
      this.form.markAllAsTouched();
      return;
    }

    const { description, value, zipCode, number } = this.form.getRawValue();

    this.orderSvc.create({
      description: description!,
      value: value!,
      deliveryAddress: { zipCode: zipCode!, number: number! },
    }).subscribe(() => this.dialogRef.close());
  }

  close() {
    this.dialogRef.close();
  }
}
