import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AppState } from '../../state/app.state';

export const UnauthGuard = () => {
    const appState = inject(AppState);
    const router = inject(Router);

    if (appState.isAuthenticated() || localStorage.getItem('role')) {
        return router.parseUrl('/dashboard');
    }

    return true;
};
