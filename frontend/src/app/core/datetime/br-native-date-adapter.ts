import { Injectable } from '@angular/core';
import { NativeDateAdapter } from '@angular/material/core';

/**
 * O {@link NativeDateAdapter} do Material usa `Date.parse()`, que interpreta datas no estilo US.
 * Aqui forçamos texto `dd/MM/yyyy` como calendário brasileiro (dia/mês/ano).
 */
@Injectable()
export class BrNativeDateAdapter extends NativeDateAdapter {
  override parse(value: unknown, parseFormat?: object): Date | null {
    if (value == null || value === '') {
      return null;
    }
    if (typeof value === 'number') {
      return new Date(value);
    }
    if (value instanceof Date) {
      return isNaN(value.getTime()) ? null : value;
    }
    if (typeof value === 'string') {
      const trimmed = value.trim();
      const m = trimmed.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
      if (m) {
        const day = Number(m[1]);
        const month = Number(m[2]) - 1;
        const year = Number(m[3]);
        const dt = new Date(year, month, day);
        if (dt.getFullYear() === year && dt.getMonth() === month && dt.getDate() === day) {
          return dt;
        }
        return null;
      }
      return super.parse(trimmed, parseFormat);
    }
    return null;
  }
}
