export function unwrapApiResponse<T>(res: unknown): T | null {
  if (res === null || res === undefined) {
    return null;
  }
  if (typeof res === 'object' && 'data' in res) {
    const r = res as { success?: boolean; data?: T };
    if (r.success === false) {
      return null;
    }
    return (r.data ?? null) as T | null;
  }
  return res as T;
}
