import { cleanup, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Plus, Search } from 'lucide-react';
import { afterEach, describe, expect, it, vi } from 'vitest';
import {
  Badge,
  Banner,
  Button,
  Checkbox,
  EmptyState,
  IconButton,
  Select,
  StatusBadge,
  Switch,
  TextArea,
  TextField,
} from './index';

// Kural: packages/ui-web/AGENTS.md (erişilebilirlik, klavye), tasarım sistemi README'leri (varyant, durum adı, hata metni).
afterEach(cleanup);

describe('Button', () => {
  it('varsayılan type=button, tıklama çalışır', async () => {
    const onClick = vi.fn();
    render(<Button onClick={onClick}>Kaydet</Button>);
    const button = screen.getByRole('button', { name: 'Kaydet' });
    expect(button.getAttribute('type')).toBe('button');
    await userEvent.click(button);
    expect(onClick).toHaveBeenCalledOnce();
  });

  it('loading: etkileşim kapanır, metin değişir, aria-busy', async () => {
    const onClick = vi.fn();
    render(
      <Button variant="primary" loading loadingLabel="Kaydediliyor…" onClick={onClick}>
        Kaydet
      </Button>,
    );
    const button = screen.getByRole('button', { name: 'Kaydediliyor…' });
    expect(button.getAttribute('aria-busy')).toBe('true');
    await userEvent.click(button);
    expect(onClick).not.toHaveBeenCalled();
  });

  it('klavyeyle odaklanır ve Enter ile çalışır', async () => {
    const onClick = vi.fn();
    render(
      <Button icon={<Plus size={16} />} onClick={onClick}>
        Hizmet ekle
      </Button>,
    );
    await userEvent.tab();
    expect(document.activeElement).toBe(screen.getByRole('button', { name: 'Hizmet ekle' }));
    await userEvent.keyboard('{Enter}');
    expect(onClick).toHaveBeenCalledOnce();
  });
});

it('IconButton ad taşır', () => {
  render(<IconButton label="Bildirimler" icon={<Search size={16} />} />);
  expect(screen.getByRole('button', { name: 'Bildirimler' })).toBeTruthy();
});

describe('TextField', () => {
  it('etiket alanı adlandırır, hata alana bağlıdır', () => {
    render(
      <TextField
        label="Telefon"
        placeholder="0532 123 45 67"
        error="Telefon numarası 10 haneli olmalı."
      />,
    );
    const input = screen.getByLabelText('Telefon');
    expect(input.getAttribute('aria-invalid')).toBe('true');
    const describedBy = input.getAttribute('aria-describedby') ?? '';
    expect(document.getElementById(describedBy)?.textContent).toBe(
      'Telefon numarası 10 haneli olmalı.',
    );
  });

  it('yardımcı metin hatasızken gösterilir, birim okunmaz', async () => {
    render(<TextField label="Süre" hint="Aşama varsa toplamı" suffix="dk" />);
    const input = screen.getByLabelText('Süre');
    expect(input.getAttribute('aria-invalid')).toBeNull();
    await userEvent.type(input, '45');
    expect((input as HTMLInputElement).value).toBe('45');
  });
});

it('TextArea: etiket ve hata alana bağlı, çok satır yazılır', async () => {
  render(<TextArea label="Salon metni" error="{bağlantı} değişkeni metinde olmalı." />);
  const area = screen.getByRole('textbox', { name: 'Salon metni' });
  expect(area.getAttribute('aria-invalid')).toBe('true');
  expect(document.getElementById(area.getAttribute('aria-describedby') ?? '')?.textContent).toBe(
    '{bağlantı} değişkeni metinde olmalı.',
  );
  await userEvent.type(area, 'a{Enter}b');
  expect((area as HTMLTextAreaElement).value).toBe('a\nb');
});

it('Select seçenekleri metin veya {value,label}', async () => {
  const onChange = vi.fn();
  render(
    <Select
      label="Tür"
      options={['Kuaför', { value: 'barber', label: 'Berber' }]}
      onChange={onChange}
    />,
  );
  const select = screen.getByLabelText('Tür');
  await userEvent.selectOptions(select, 'barber');
  expect(onChange).toHaveBeenCalled();
  expect((select as HTMLSelectElement).value).toBe('barber');
});

it('TextField ve Select hideLabel: ad korunur, etiket gizli sınıfta', () => {
  render(
    <>
      <TextField label="Bağlantı adı" hideLabel />
      <Select label="Yanıt süresi" options={['2 saat']} hideLabel />
    </>,
  );
  expect(screen.getByRole('textbox', { name: 'Bağlantı adı' })).toBeTruthy();
  expect(screen.getByRole('combobox', { name: 'Yanıt süresi' })).toBeTruthy();
  expect(screen.getByText('Bağlantı adı').className).toContain('visuallyHidden');
});

it('Checkbox açıklamasıyla etiketlenir ve boşlukla işaretlenir', async () => {
  render(
    <Checkbox
      label="Bu bilgisayarda 30 gün hatırla"
      description="Ortak bilgisayarda işaretlemeyin."
    />,
  );
  const box = screen.getByRole('checkbox', { name: /30 gün hatırla/ });
  box.focus();
  await userEvent.keyboard(' ');
  expect((box as HTMLInputElement).checked).toBe(true);
});

it('Switch role=switch', async () => {
  const onChange = vi.fn();
  render(<Switch label="Online randevu açık" onChange={onChange} />);
  await userEvent.click(screen.getByRole('switch', { name: 'Online randevu açık' }));
  expect(onChange).toHaveBeenCalledOnce();
});

it('Switch hideLabel: ad ekran okuyucuda, görünür metin yok', () => {
  render(<Switch label="Fiyatları göster" hideLabel />);
  const control = screen.getByRole('switch', { name: 'Fiyatları göster' });
  expect(control.parentElement?.querySelector('[class*="text"]')).toBeNull();
});

it('Badge ve StatusBadge metinle', () => {
  render(
    <>
      <Badge tone="vurgu">Yeni</Badge>
      <StatusBadge status="kapora" />
    </>,
  );
  expect(screen.getByText('Yeni')).toBeTruthy();
  expect(screen.getByText('Kapora bekleniyor')).toBeTruthy();
});

it('Banner: hata alert, diğerleri status; tek eylem', () => {
  render(
    <>
      <Banner tone="hata" title="Abonelik ödemesi alınamadı" />
      <Banner
        tone="bilgi"
        title="Deneme sürenizin bitmesine 9 gün var"
        action={<Button size="sm">Planları gör</Button>}
      />
    </>,
  );
  expect(screen.getByRole('alert').textContent).toContain('Abonelik ödemesi alınamadı');
  expect(screen.getByRole('status').textContent).toContain('9 gün');
  expect(screen.getByRole('button', { name: 'Planları gör' })).toBeTruthy();
});

it('EmptyState başlık düzeyi ve eylem', () => {
  render(
    <EmptyState
      title="İlk hizmetinizi ekleyin"
      description="Online randevu için en az bir hizmet gerekir."
      actions={<Button>Hizmet ekle</Button>}
      headingLevel={2}
    />,
  );
  expect(screen.getByRole('heading', { level: 2, name: 'İlk hizmetinizi ekleyin' })).toBeTruthy();
  expect(screen.getByRole('button', { name: 'Hizmet ekle' })).toBeTruthy();
});
