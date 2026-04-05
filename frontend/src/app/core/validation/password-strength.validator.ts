import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function strongPasswordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const v = control.value as string | null | undefined;
    if (v == null || v === '') {
      return null;
    }
    if (v.length < 8) {
      return { strongPassword: true };
    }
    if (!/[A-Z]/.test(v)) {
      return { strongPassword: true };
    }
    if (!/[a-z]/.test(v)) {
      return { strongPassword: true };
    }
    if (!/\d/.test(v)) {
      return { strongPassword: true };
    }
    if (!/[^A-Za-z0-9]/.test(v)) {
      return { strongPassword: true };
    }
    return null;
  };
}

export const STRONG_PASSWORD_HINT =
  'Mínimo 8 caracteres, com maiúscula, minúscula, número e um caractere especial.';
