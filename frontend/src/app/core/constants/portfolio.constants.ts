export const PORTFOLIO_API_PRIMARY = 'invoice-generator-c';

export const PORTFOLIO_DISPLAY_NAME = 'invoice-generator-c';

export interface PortfolioOption {
  label: string;
  value: string;
}

export const DEFAULT_PORTFOLIO_OPTIONS: readonly PortfolioOption[] = [
  { label: PORTFOLIO_DISPLAY_NAME, value: PORTFOLIO_API_PRIMARY }
];

export function portfolioLabelForApiValue(apiValue: string | null | undefined): string {
  if (!apiValue) {
    return PORTFOLIO_DISPLAY_NAME;
  }
  if (apiValue === PORTFOLIO_API_PRIMARY) {
    return PORTFOLIO_DISPLAY_NAME;
  }
  return apiValue;
}
