import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent),
        //canActivate: [AuthGuard],
        title: 'Home',
        children: [
            {
                path: 'customers',
                loadComponent: () => import('./components/home/customers/customers.component').then(m => m.CustomersComponent),
                //canActivate: [AuthGuard],
                title: 'Customers'
            },
            {
                path: 'products',
                loadComponent: () => import('./components/home/products/products.component').then(m => m.ProductsComponent),
                //canActivate: [AuthGuard],
                title: 'Products'
            },
            {
                path: 'orders',
                loadComponent: () => import('./components/home/orders/orders.component').then(m => m.OrdersComponent),
                //canActivate: [AuthGuard],
                title: 'Orders'
            },
            {
                path: 'users',
                loadComponent: () => import('./components/home/user/user.component').then(m => m.UserComponent),
                //canActivate: [AuthGuard],
                title: 'Users'
            },
            {
                path: 'roles',
                loadComponent: () => import('./components/home/role/role.component').then(m => m.RoleComponent),
                //canActivate: [AuthGuard],
                title: 'Roles'
            },
            {
                path: 'profiles',
                loadComponent: () => import('./components/home/profile/profile.component').then(m => m.ProfileComponent),
                //canActivate: [AuthGuard],
                title: 'Profiles'
            }
        ]
    },
    {
        path: 'login',
        loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent),
        title: 'Login'
    },

    {
        path: 'settings',
        loadComponent: () => import('./components/settings/settings.component').then(m => m.SettingsComponent),
        //canActivate: [AuthGuard],
        title: 'Settings'
    },
    {
        path: 'about',
        loadComponent: () => import('./components/about/about.component').then(m => m.AboutComponent),
        title: 'About Us'
    },
    {
        path: 'home',
        redirectTo: '/',
        pathMatch: 'full'
    },
    {
        path: '**',
        loadComponent: () => import('./components/not-found/not-found.component').then(m => m.NotFoundComponent),
        title: 'Page Not Found'
    }
];
