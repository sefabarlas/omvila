import { idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDuration, parseMoneyInput } from '@omvila/ui-web/format';
import { Banner, Button, Checkbox, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { ApiProblem } from '@omvila/api-client-ts';
import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useCreateFromTemplates } from './api';
import type { ServiceTemplate } from './api';
import styles from './Services.module.css';

/**
 * Hazır listeden başla (kabul kriteri 8): sektörün şablonlarından seçilenler eklenir, kategoriler yoksa oluşur.
 * Fiyat isteğe bağlı (boşsa 0 ₺ eklenir, sonra düzenlenir). Tasarımı yok (acik-sorular.md T-43). Açıkken bağlanır.
 */
export function TemplatesModal({
  industryLabel,
  templates,
  onClose,
}: {
  industryLabel: string;
  templates: readonly ServiceTemplate[];
  onClose: () => void;
}) {
  const toast = useToast();
  const create = useCreateFromTemplates();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const [selected, setSelected] = useState<Set<string>>(() => new Set(templates.map((t) => t.id)));
  const [prices, setPrices] = useState<Record<string, string>>({});
  const [error, setError] = useState<string>();
  const categories = [...new Set(templates.map((t) => t.categoryName))];
  const invalidPrice = (id: string) => {
    const text = prices[id]?.trim() ?? '';
    return text !== '' && parseMoneyInput(text) === null;
  };

  const toggle = (id: string) => {
    const next = new Set(selected);
    if (next.has(id)) {
      next.delete(id);
    } else {
      next.add(id);
    }

    setSelected(next);
  };

  const onSubmit = async (event: SyntheticEvent) => {
    event.preventDefault();
    const chosen = templates.filter((t) => selected.has(t.id));
    if (chosen.length === 0) {
      setError('En az bir hizmet seçin.');
      return;
    }

    if (chosen.some((t) => invalidPrice(t.id))) {
      setError('İşaretli fiyatları düzeltin: 450 veya 85,50 gibi.');
      return;
    }

    try {
      const result = await create.mutateAsync({
        key,
        items: chosen.map((t) => {
          const amount = parseMoneyInput(prices[t.id]?.trim() ?? '');
          return {
            templateId: t.id,
            ...(amount !== null
              ? { price: { amountMinor: amount, currency: 'TRY' as const } }
              : {}),
          };
        }),
      });
      toast.show({ tone: 'basari', title: `${String(result.items.length)} hizmet eklendi` });
      onClose();
    } catch (problem) {
      toast.show({
        tone: 'hata',
        title: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  };

  return (
    <Modal
      open
      size="lg"
      onClose={onClose}
      title={`${industryLabel} hazır listesi`}
      description="Verdiğiniz hizmetleri seçin; fiyatı şimdi veya sonra girebilirsiniz."
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="hazir-liste"
            loading={create.isPending}
            loadingLabel="Ekleniyor…"
          >
            {`${String(selected.size)} hizmeti ekle`}
          </Button>
        </>
      }
    >
      <form id="hazir-liste" noValidate onSubmit={(e) => void onSubmit(e)}>
        {error ? <Banner tone="hata">{error}</Banner> : null}
        {categories.map((category) => (
          <fieldset key={category} className={styles.templateGroup}>
            <legend className={styles.groupTitle}>{category}</legend>
            {templates
              .filter((t) => t.categoryName === category)
              .map((t) => (
                <div key={t.id} className={styles.templateRow}>
                  <Checkbox
                    label={t.name}
                    checked={selected.has(t.id)}
                    onChange={() => {
                      toggle(t.id);
                    }}
                  />
                  <span className={styles.muted}>{formatDuration(t.defaultDurationMinutes)}</span>
                  <TextField
                    label={`${t.name} fiyatı`}
                    hideLabel
                    inputMode="decimal"
                    placeholder="Fiyat"
                    suffix="₺"
                    disabled={!selected.has(t.id)}
                    error={invalidPrice(t.id) ? 'Fiyatı kontrol edin.' : undefined}
                    value={prices[t.id] ?? ''}
                    onChange={(event) => {
                      setPrices({ ...prices, [t.id]: event.target.value });
                      setError(undefined);
                    }}
                  />
                </div>
              ))}
          </fieldset>
        ))}
      </form>
    </Modal>
  );
}
