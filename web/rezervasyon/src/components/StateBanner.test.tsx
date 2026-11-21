import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import { monogram } from './SalonHeader';
import { StateBanner } from './StateBanner';

describe('StateBanner', () => {
  it('hata tonunu ekran okuyucuya hemen okutur (alert)', () => {
    render(
      <StateBanner
        tone="hata"
        icon={null}
        title="Çok fazla deneme yaptınız."
        description="Güvenliğiniz için 5 dakika sonra yeni kod isteyebilirsiniz."
      />,
    );
    const alert = screen.getByRole('alert');
    expect(alert.textContent).toContain('Çok fazla deneme yaptınız.');
    expect(alert.className).toContain('hata');
  });

  it('uyarı tonu status rolüyle ve eylemleriyle çizilir', () => {
    render(
      <StateBanner tone="uyari" icon={null} title="Başlık" description="Açıklama">
        <button type="button">Salonu ara</button>
      </StateBanner>,
    );
    expect(screen.getByRole('status').className).toContain('uyari');
    expect(screen.getByRole('button', { name: 'Salonu ara' })).toBeTruthy();
  });
});

describe('monogram', () => {
  it('ilk iki kelimenin baş harflerini Türkçe büyük harfle verir', () => {
    expect(monogram('Kuaför Elif')).toBe('KE');
    expect(monogram('ışıl  güzellik salonu')).toBe('IG');
    expect(monogram('Berber')).toBe('B');
  });
});
