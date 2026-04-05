import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AppState } from '../../state/app.state';

@Injectable({
    providedIn: 'root'
})
export class AdminGuard implements CanActivate {
    constructor(private appState: AppState, private router: Router) { }

    canActivate(): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
        if (this.appState.isAuthenticated() && this.appState.currentUserRole() === 'Admin') {
            return true;
        }

        this.router.navigate(['/dashboard']);
        return false;
    }
}
