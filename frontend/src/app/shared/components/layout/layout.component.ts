import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AppState } from '../../../state/app.state';
import { ThemeService } from '../../../core/services/theme.service';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FooterComponent } from '../ui/footer/footer.component';
import { APP_DISPLAY_NAME } from '../../../core/constants/app-branding.constants';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule, RouterModule,
    MatToolbarModule, MatSidenavModule, MatIconModule,
    MatListModule, MatButtonModule, MatDividerModule, MatTooltipModule,
    FooterComponent
  ],
  templateUrl: './layout.component.html',
  styleUrl: './layout.component.css',
})
export class LayoutComponent implements OnInit {
  appState = inject(AppState);
  router = inject(Router);
  themeService = inject(ThemeService);

  readonly appTitle = APP_DISPLAY_NAME;

  get isDarkMode() { return this.themeService.isDarkMode(); }

  ngOnInit() { }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  logout() {
    this.appState.logout();
    this.router.navigate(['/login']);
  }
}
