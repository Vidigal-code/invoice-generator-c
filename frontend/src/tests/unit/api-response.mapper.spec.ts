import { unwrapApiResponse } from '../../app/core/http/api-response.mapper';

describe('unwrapApiResponse', () => {
  it('returns null for null/undefined', () => {
    expect(unwrapApiResponse(null)).toBeNull();
    expect(unwrapApiResponse(undefined)).toBeNull();
  });

  it('extracts data when envelope has success true', () => {
    expect(unwrapApiResponse({ success: true, data: { x: 1 } })).toEqual({ x: 1 });
  });

  it('returns null when success is false', () => {
    expect(unwrapApiResponse({ success: false, data: { x: 1 } })).toBeNull();
  });

  it('returns data when success omitted', () => {
    expect(unwrapApiResponse({ data: [1, 2] })).toEqual([1, 2]);
  });

  it('returns raw body when no data property', () => {
    const raw = { token: 'abc' };
    expect(unwrapApiResponse(raw)).toBe(raw);
  });
});
