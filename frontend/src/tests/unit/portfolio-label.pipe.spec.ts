import { PortfolioLabelPipe } from '../../app/shared/pipes/portfolio-label.pipe';
import { PORTFOLIO_API_PRIMARY, PORTFOLIO_DISPLAY_NAME } from '../../app/core/constants/portfolio.constants';

describe('PortfolioLabelPipe', () => {
  const pipe = new PortfolioLabelPipe();

  it('transforms API primary to display name', () => {
    expect(pipe.transform(PORTFOLIO_API_PRIMARY)).toBe(PORTFOLIO_DISPLAY_NAME);
  });
});
