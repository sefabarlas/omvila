import { render } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { ThemeSync } from './ThemeSync';

function mockMedia(initial: boolean) {
  let listener: (() => void) | undefined;
  const media = {
    matches: initial,
    addEventListener: (_: string, fn: () => void) => {
      listener = fn;
    },
    removeEventListener: vi.fn(),
  };
  vi.stubGlobal('matchMedia', () => media);
  return {
    change(dark: boolean) {
      media.matches = dark;
      listener?.();
    },
  };
}

describe('ThemeSync', () => {
  it('data-theme’i sistem temasından yazar ve değişikliği izler', () => {
    const media = mockMedia(true);
    render(<ThemeSync />);
    expect(document.documentElement.dataset.theme).toBe('dark');

    media.change(false);
    expect(document.documentElement.dataset.theme).toBe('light');
  });
});
