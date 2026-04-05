import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ThemeService } from '../../../../core/services/theme.service';
import { AppState } from '../../../../state/app.state';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatButtonModule, MatIconModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  private readonly themeService = inject(ThemeService);
  private readonly appState = inject(AppState);

  menuOpen = false;

  get isDarkMode(): boolean { return this.themeService.isDarkMode(); }
  get isAuthenticated(): boolean { return this.appState.isAuthenticated(); }

  toggleTheme(): void { this.themeService.toggleTheme(); }
  toggleMenu(): void { this.menuOpen = !this.menuOpen; }
  closeMenu(): void { this.menuOpen = false; }
}
