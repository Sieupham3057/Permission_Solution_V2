import { Component, inject, signal, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CustomersApi } from '../customers/data-access/customers.api';
import { CustomerDto } from '../customers/data-access/customers.models';

@Component({
  standalone: true,
  selector: 'app-customers-list-page',
  template: `
    <h2>Customers</h2>

    <button type="button" (click)="reload()">Reload</button>

    @if (loading()) {
      <div>Loading...</div>
    } @else {
      <ul>
        @for (c of customers(); track c.id) {
          <li>
            <b>{{ c.name }}</b> — {{ c.email ?? '-' }}
          </li>
        }
      </ul>
    }
  `,
})
export class CustomersListPage {
  private api = inject(CustomersApi);
  private destroyRef = inject(DestroyRef);

  customers = signal<CustomerDto[]>([]);
  loading = signal(false);

  ngOnInit() {
    this.reload();
  }

  reload() {
    this.loading.set(true);

    this.api.getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (data) => {
          this.customers.set(data ?? []);
          this.loading.set(false);
        },
        error: (err) => {
          console.error(err);
          this.loading.set(false);
        },
      });
  }
}
