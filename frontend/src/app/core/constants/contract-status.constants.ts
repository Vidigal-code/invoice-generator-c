const CONTRACT_STATUS_LABELS: Record<string, string> = {
  Active: 'Ativo',
  Negotiated: 'Negociado',
  Defaulted: 'Inadimplente',
  Closed: 'Encerrado',
  Cancelled: 'Cancelado'
};

export function contractStatusLabel(status: string | undefined | null): string {
  if (!status) {
    return '—';
  }
  return CONTRACT_STATUS_LABELS[status] ?? status;
}

const AGREEMENT_STATUS_FRAGMENTS: [string, string][] = [
  ['Pending', 'Pendente'],
  ['Active', 'Ativo'],
  ['Broken', 'Inviabilizado'],
  ['Completed', 'Concluído']
];

export function agreementHistoryDisplayLabel(
  row: { effectiveStatus?: string; status?: string } | null | undefined
): string {
  const s = row?.effectiveStatus ?? row?.status ?? '—';
  let out = s;
  for (const [en, pt] of AGREEMENT_STATUS_FRAGMENTS) {
    out = out.replace(en, pt);
  }
  return out;
}
