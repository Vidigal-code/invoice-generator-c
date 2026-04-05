import { Pipe, PipeTransform } from '@angular/core';
import { portfolioLabelForApiValue } from '../../core/constants/portfolio.constants';

@Pipe({ name: 'portfolioLabel', standalone: true })
export class PortfolioLabelPipe implements PipeTransform {
  transform(apiValue: string | null | undefined): string {
    return portfolioLabelForApiValue(apiValue);
  }
}
