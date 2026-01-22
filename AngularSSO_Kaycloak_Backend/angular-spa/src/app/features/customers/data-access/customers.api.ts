import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerDto } from './customers.models';

@Injectable({ providedIn: 'root' })
export class CustomersApi {
  private http = inject(HttpClient);

  getAll(): Observable<CustomerDto[]> {
    // ✅ relative + marker
    return this.http.get<CustomerDto[]>('/customer-service/api/Customers');
  }
}
