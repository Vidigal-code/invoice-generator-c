import { Component, inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { RouterOutlet } from '@angular/router';

import { environment } from '../environments/environment';

type WeatherForecast = {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string | null;
};

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  private readonly http = inject(HttpClient);

  readonly title = environment.appTitle;
  readonly apiUrl = environment.apiUrl;
  forecasts: WeatherForecast[] | null = null;
  error: string | null = null;

  ngOnInit(): void {
    const url = `${this.apiUrl}/weatherforecast`;
    this.http.get<WeatherForecast[]>(url).subscribe({
      next: (data) => {
        this.forecasts = data;
      },
      error: () => {
        this.error = `Não foi possível obter dados de ${url}. Confirma que a API está a correr e o CORS está ativo.`;
      },
    });
  }
}
