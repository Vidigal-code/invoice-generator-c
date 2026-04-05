import { Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { UnauthGuard } from './core/guards/unauth.guard';
import { AdminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
    { path: '', loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent), canActivate: [UnauthGuard] },
    { path: 'login', loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent), canActivate: [UnauthGuard] },
    { path: 'register', loadComponent: () => import('./features/auth/register.component').then(m => m.RegisterComponent), canActivate: [UnauthGuard] },
    {
        path: '',
        loadComponent: () => import('./shared/components/layout/layout.component').then(m => m.LayoutComponent),
        canActivate: [AuthGuard],
        children: [
            { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
            { path: 'admin', loadComponent: () => import('./features/admin/admin-logs.component').then(m => m.AdminLogsComponent), canActivate: [AdminGuard] }
        ]
    },
    { path: '**', redirectTo: '/login' }
];
