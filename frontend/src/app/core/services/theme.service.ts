import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
    public isDarkMode = signal<boolean>(localStorage.getItem('theme') !== 'light');

    toggleTheme() {
        this.isDarkMode.set(!this.isDarkMode());
        const theme = this.isDarkMode() ? 'dark' : 'light';
        localStorage.setItem('theme', theme);

        if (this.isDarkMode()) {
            document.body.classList.add('dark-theme');
        } else {
            document.body.classList.remove('dark-theme');
        }
    }
}
