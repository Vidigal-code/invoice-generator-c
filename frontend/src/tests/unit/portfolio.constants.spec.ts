import {
  DEFAULT_PORTFOLIO_OPTIONS,
  PORTFOLIO_API_PRIMARY,
  PORTFOLIO_DISPLAY_NAME,
  portfolioLabelForApiValue
} from '../../app/core/constants/portfolio.constants';

describe('portfolio.constants', () => {
  it('maps primary API value to display name', () => {
    expect(portfolioLabelForApiValue(PORTFOLIO_API_PRIMARY)).toBe(PORTFOLIO_DISPLAY_NAME);
  });

  it('uses display name for empty', () => {
    expect(portfolioLabelForApiValue('')).toBe(PORTFOLIO_DISPLAY_NAME);
    expect(portfolioLabelForApiValue(undefined)).toBe(PORTFOLIO_DISPLAY_NAME);
  });

  it('passes through unknown API values', () => {
    expect(portfolioLabelForApiValue('OtherWallet')).toBe('OtherWallet');
  });

  it('display name matches product slug', () => {
    expect(PORTFOLIO_DISPLAY_NAME).toBe('invoice-generator-c');
  });

  it('DEFAULT_PORTFOLIO_OPTIONS links label to API value', () => {
    expect(DEFAULT_PORTFOLIO_OPTIONS.length).toBe(1);
    expect(DEFAULT_PORTFOLIO_OPTIONS[0].value).toBe(PORTFOLIO_API_PRIMARY);
    expect(DEFAULT_PORTFOLIO_OPTIONS[0].label).toBe(PORTFOLIO_DISPLAY_NAME);
  });
});
