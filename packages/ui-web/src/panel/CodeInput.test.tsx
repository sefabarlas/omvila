import { cleanup, render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { useState } from 'react';
import { afterEach, expect, it, vi } from 'vitest';
import { CodeInput } from './CodeInput';

// Kural: salon-paneli/GirisKod (6 hane), erişilebilirlik (grup adı, hane adları, hata bağlantısı).
afterEach(cleanup);

function Harness({ onChange }: { onChange?: (value: string) => void }) {
  const [value, setValue] = useState('');
  return (
    <CodeInput
      label="Doğrulama kodu"
      value={value}
      onChange={(next) => {
        setValue(next);
        onChange?.(next);
      }}
    />
  );
}

it('yazınca sonraki haneye geçer, geri silme öncekine döner', async () => {
  const onChange = vi.fn();
  render(<Harness onChange={onChange} />);
  await userEvent.click(screen.getByLabelText('Doğrulama kodu, hane 1'));
  await userEvent.keyboard('482');
  expect(document.activeElement).toBe(screen.getByLabelText('Doğrulama kodu, hane 4'));
  expect(onChange).toHaveBeenLastCalledWith('482');
  await userEvent.keyboard('{Backspace}');
  expect(document.activeElement).toBe(screen.getByLabelText('Doğrulama kodu, hane 3'));
  expect(onChange).toHaveBeenLastCalledWith('48');
});

it('yapıştırma hanelere dağıtılır, rakam dışı atlanır', async () => {
  const onChange = vi.fn();
  render(<Harness onChange={onChange} />);
  await userEvent.click(screen.getByLabelText('Doğrulama kodu, hane 1'));
  await userEvent.paste('482 193');
  expect(onChange).toHaveBeenLastCalledWith('482193');
});

it('grup adlı, hata bağlı', () => {
  render(
    <CodeInput label="Doğrulama kodu" value="" onChange={() => undefined} error="Kod hatalı." />,
  );
  const group = screen.getByRole('group', { name: 'Doğrulama kodu' });
  expect(document.getElementById(group.getAttribute('aria-describedby') ?? '')?.textContent).toBe(
    'Kod hatalı.',
  );
});
