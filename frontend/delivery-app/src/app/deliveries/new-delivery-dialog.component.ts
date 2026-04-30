import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DeliveryService } from '../core/services/delivery.service';

@Component({
  selector: 'app-new-delivery-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule,
  ],
  templateUrl: './new-delivery-dialog.component.html',
  styleUrl: './new-delivery-dialog.component.scss',
})
export class NewDeliveryDialogComponent {
  private fb        = inject(FormBuilder);
  readonly deliverySvc = inject(DeliveryService);
  private dialogRef = inject(MatDialogRef<NewDeliveryDialogComponent>);

  form = this.fb.group({
    orderNumber:      ['', [Validators.required, Validators.pattern(/^PED-\d{8}-[A-Z0-9]{5}$/)]],
    deliveryDateTime: [this.localDateTimeNow(), Validators.required],
  });

  private localDateTimeNow(): string {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    return now.toISOString().slice(0, 16);
  }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    const { orderNumber, deliveryDateTime } = this.form.getRawValue();

    this.deliverySvc.create({
      orderNumber: orderNumber!,
      deliveryDateTime: new Date(deliveryDateTime!).toISOString(),
    }).subscribe(() => this.dialogRef.close());
  }

  close() { this.dialogRef.close(); }
}
