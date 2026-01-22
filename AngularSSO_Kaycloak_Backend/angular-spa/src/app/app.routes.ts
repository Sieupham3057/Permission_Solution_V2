import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'customers',
    loadComponent: () =>
      import('./features/pages/customers-list.page')
        .then(m => m.CustomersListPage),
  },
  { path: '', pathMatch: 'full', redirectTo: 'customers' },
];
