import { Component, inject, OnInit, Renderer2 } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  readonly title = environment.appTitle;

  constructor(private renderer: Renderer2) { }

  ngOnInit(): void {
    const savedTheme = localStorage.getItem('theme');
    if (!savedTheme || savedTheme === 'dark') {
      this.renderer.addClass(document.body, 'dark-theme');
      localStorage.setItem('theme', 'dark');
    } else {
      this.renderer.removeClass(document.body, 'dark-theme');
    }
  }
}
