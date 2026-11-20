import { act, cleanup, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useState } from 'react';
import { afterEach, describe, expect, it, vi } from 'vitest';
import {
  Avatar,
  Button,
  Drawer,
  initials,
  Modal,
  Skeleton,
  Tabs,
  TextField,
  ToastProvider,
  Tooltip,
  useToast,
} from './index';

// Kural: tasarım sistemi README'leri (Modal, Toast, Tabs, Avatar, Tooltip), urun-kurallari.md (iskelet 150 ms,
// "başarıyla" yok), packages/ui-web/AGENTS.md (klavye erişimi).
afterEach(() => {
  cleanup();
  vi.useRealTimers();
});

function ModalHarness({ drawer = false }: { drawer?: boolean }) {
  const [open, setOpen] = useState(false);
  const Dialog = drawer ? Drawer : Modal;
  return (
    <>
      <Button
        onClick={() => {
          setOpen(true);
        }}
      >
        Aç
      </Button>
      <Dialog
        open={open}
        onClose={() => {
          setOpen(false);
        }}
        title="Yeni kategori"
        description="Hizmetler"
        footer={<Button variant="primary">Kaydet</Button>}
      >
        <TextField label="Kategori adı" />
      </Dialog>
    </>
  );
}

describe.each([false, true])('diyalog (yan panel: %s)', (drawer) => {
  it('başlıkla adlandırılır, ilk öğeye odaklanır, Esc kapatır ve odak açana döner', async () => {
    render(<ModalHarness drawer={drawer} />);
    const opener = screen.getByRole('button', { name: 'Aç' });
    await userEvent.click(opener);

    const dialog = screen.getByRole('dialog', { name: 'Yeni kategori' });
    expect(dialog.getAttribute('aria-modal')).toBe('true');
    expect(document.activeElement).toBe(screen.getByRole('button', { name: 'Kapat' }));

    await userEvent.keyboard('{Escape}');
    expect(screen.queryByRole('dialog')).toBeNull();
    expect(document.activeElement).toBe(opener);
  });

  it('arka plana tıklamak kapatır', async () => {
    render(<ModalHarness drawer={drawer} />);
    await userEvent.click(screen.getByRole('button', { name: 'Aç' }));
    const overlay = screen.getByRole('dialog').parentElement;
    if (!overlay) {
      throw new Error('Arka plan yok');
    }

    await userEvent.pointer({ keys: '[MouseLeft]', target: overlay });
    expect(screen.queryByRole('dialog')).toBeNull();
  });

  it('Tab içeride döner', async () => {
    render(<ModalHarness drawer={drawer} />);
    await userEvent.click(screen.getByRole('button', { name: 'Aç' }));
    await userEvent.tab();
    expect(document.activeElement).toBe(screen.getByLabelText('Kategori adı'));
    await userEvent.tab();
    expect(document.activeElement).toBe(screen.getByRole('button', { name: 'Kaydet' }));
    await userEvent.tab();
    expect(document.activeElement).toBe(screen.getByRole('button', { name: 'Kapat' }));
  });
});

function ToastHarness() {
  const toast = useToast();
  return (
    <>
      <Button
        onClick={() => {
          toast.show({ title: 'Randevu oluşturuldu' });
        }}
      >
        Başarılı
      </Button>
      <Button
        onClick={() => {
          toast.show({ tone: 'hata', title: 'Kaydedilemedi' });
        }}
      >
        Hata
      </Button>
    </>
  );
}

describe('Toast', () => {
  it('başarı kendiliğinden kapanır, hata kalır; en fazla üç', () => {
    vi.useFakeTimers();
    render(
      <ToastProvider durationMs={5000}>
        <ToastHarness />
      </ToastProvider>,
    );
    act(() => {
      screen.getByRole('button', { name: 'Hata' }).click();
      for (let i = 0; i < 3; i++) {
        screen.getByRole('button', { name: 'Başarılı' }).click();
      }
    });
    expect(screen.getAllByText('Randevu oluşturuldu')).toHaveLength(3);
    expect(screen.queryByText('Kaydedilemedi')).toBeNull();

    act(() => {
      screen.getByRole('button', { name: 'Hata' }).click();
      vi.advanceTimersByTime(5000);
    });
    expect(screen.queryAllByText('Randevu oluşturuldu')).toHaveLength(0);
    expect(screen.getByRole('alert').textContent).toContain('Kaydedilemedi');
  });

  it('ToastProvider dışında hata verir', () => {
    vi.spyOn(console, 'error').mockImplementation(() => undefined);
    expect(() => render(<ToastHarness />)).toThrow(/ToastProvider/);
  });
});

it('Skeleton 150 ms dolmadan görünmez', () => {
  vi.useFakeTimers();
  const { container } = render(<Skeleton />);
  expect(container.firstChild).toBeNull();
  act(() => {
    vi.advanceTimersByTime(150);
  });
  expect(container.firstChild).not.toBeNull();
});

it('Tabs ok tuşlarıyla dolaşır, sayı yalnız sıfırdan büyükse', async () => {
  const onChange = vi.fn();
  render(
    <Tabs
      label="Randevular"
      value="gelecek"
      onChange={onChange}
      items={[
        { value: 'gelecek', label: 'Gelecek', count: 2 },
        { value: 'iptal', label: 'İptal', count: 0 },
      ]}
    />,
  );
  const first = screen.getByRole('tab', { name: /Gelecek/ });
  expect(first.getAttribute('aria-selected')).toBe('true');
  expect(screen.getByRole('tab', { name: 'İptal' }).textContent).toBe('İptal');
  first.focus();
  await userEvent.keyboard('{ArrowRight}');
  expect(onChange).toHaveBeenCalledWith('iptal');
  expect(document.activeElement).toBe(screen.getByRole('tab', { name: 'İptal' }));
});

it('Avatar: Türkçe baş harfler, ad okunur', () => {
  expect(initials('ilker şahin')).toBe('İŞ');
  expect(initials('Elif')).toBe('E');
  render(<Avatar name="Elif Kaya" />);
  expect(screen.getByRole('img', { name: 'Elif Kaya' }).textContent).toBe('EK');
});

it('Tooltip öğeye aria-describedby ile bağlanır', () => {
  render(
    <Tooltip content="Onay bekleyen 3 randevu">
      <button type="button">Bildirimler</button>
    </Tooltip>,
  );
  const button = screen.getByRole('button', { name: 'Bildirimler' });
  expect(document.getElementById(button.getAttribute('aria-describedby') ?? '')?.textContent).toBe(
    'Onay bekleyen 3 randevu',
  );
});
