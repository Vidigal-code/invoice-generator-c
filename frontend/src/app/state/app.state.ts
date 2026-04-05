import { Injectable, signal } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class AppState {
    public isAuthenticated = signal<boolean>(!!localStorage.getItem('role'));
    public currentUserRole = signal<string>(localStorage.getItem('role') || '');
    public currentUsername = signal<string>(localStorage.getItem('username') || '');
    public activeContract = signal<any>(null);
    public debtCalculationResult = signal<any>(null);

    setLoginState(role: string, username: string = '') {
        localStorage.setItem('role', role);
        localStorage.setItem('username', username);
        this.isAuthenticated.set(true);
        this.currentUserRole.set(role);
        this.currentUsername.set(username);
    }

    logout() {
        localStorage.removeItem('role');
        localStorage.removeItem('username');
        this.isAuthenticated.set(false);
        this.currentUserRole.set('');
        this.currentUsername.set('');
        this.activeContract.set(null);
        this.debtCalculationResult.set(null);
    }
}
