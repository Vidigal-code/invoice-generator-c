import {
  agreementHistoryDisplayLabel,
  contractStatusLabel
} from '../../app/core/constants/contract-status.constants';

describe('contract-status.constants', () => {
  it('contractStatusLabel maps known statuses', () => {
    expect(contractStatusLabel('Active')).toBe('Ativo');
    expect(contractStatusLabel('Negotiated')).toBe('Negociado');
  });

  it('contractStatusLabel returns dash for empty', () => {
    expect(contractStatusLabel(undefined)).toBe('—');
    expect(contractStatusLabel(null)).toBe('—');
  });

  it('agreementHistoryDisplayLabel translates fragments', () => {
    expect(agreementHistoryDisplayLabel({ status: 'Pending' })).toContain('Pendente');
    expect(agreementHistoryDisplayLabel({ effectiveStatus: 'Active' })).toContain('Ativo');
  });
});
