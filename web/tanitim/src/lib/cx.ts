/** CSS Modules sınıf adlarını birleştirir; boş, false ve tanımsız değerler atlanır. */
export function cx(...names: (string | false | null | undefined)[]): string {
  return names.filter(Boolean).join(' ');
}
