import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatCount } from '@omvila/ui-web/format';
import { Banner, Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useGrantCredits } from './api';
import styles from './Brands.module.css';
import type { BrandDetail } from './labels';
import { signedCount } from './labels';

// Sözleşme sınırları (AdminCreditGrant): ±100.000, 0 olamaz; neden 3–200 karakter.
const maxCredits = 100_000;

const schema = z.object({
  credits: z
    .string()
    .trim()
    .regex(/^-?\d+$/, 'Tam sayı girin; düzeltme için eksi yazın.')
    .refine((v) => Number(v) !== 0, 'Kontör 0 olamaz.')
    .refine(
      (v) => Math.abs(Number(v)) <= maxCredits,
      `En fazla ${formatCount(maxCredits)} kontör.`,
    ),
  reason: z
    .string()
    .trim()
    .min(3, 'Nedeni en az 3 harfle yazın.')
    .max(200, 'Neden en fazla 200 karakter olabilir.'),
});

type Form = z.infer<typeof schema>;

/** "Kontör ekle" (adminGrantCredits): hediye veya düzeltme; neden denetime yazılır. Yalnız PlatformAdmin açar. */
export function GrantCreditsModal({ brand, onClose }: { brand: BrandDetail; onClose: () => void }) {
  const toast = useToast();
  const grant = useGrantCredits(brand.id);
  const form = useForm<Form>({
    resolver: zodResolver(schema),
    defaultValues: { credits: '', reason: '' },
  });

  const submit = form.handleSubmit(async ({ credits, reason }) => {
    try {
      const result = await grant.mutateAsync({ credits: Number(credits), reason });
      toast.show({
        tone: 'basari',
        title: 'Kontör eklendi',
        message: `${signedCount(result.transaction.credits)} kontör yazıldı. Bakiye ${formatCount(result.balance)}.`,
      });
      onClose();
    } catch (problem) {
      const code = problem instanceof ApiProblem ? problem.code : undefined;
      form.setError('root', {
        message:
          code === 'insufficient_credits'
            ? 'Düzeltme bakiyeyi sıfırın altına indiremez.'
            : errorMessage(code),
      });
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title="Kontör ekle"
      description={`${brand.name} · bakiye ${formatCount(brand.credits.balance)}`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="kontor-ekle"
            loading={form.formState.isSubmitting}
            loadingLabel="Ekleniyor…"
          >
            Kontörü ekle
          </Button>
        </>
      }
    >
      <form
        id="kontor-ekle"
        className={styles.form}
        onSubmit={(event) => void submit(event)}
        noValidate
      >
        {form.formState.errors.root?.message ? (
          <Banner tone="hata" title="Kontör eklenemedi.">
            {form.formState.errors.root.message}
          </Banner>
        ) : null}
        <TextField
          label="Kontör"
          inputMode="numeric"
          placeholder="500"
          hint="Düzeltme için eksi yazın (ör. -200); bakiye sıfırın altına inmez."
          error={form.formState.errors.credits?.message}
          {...form.register('credits')}
        />
        <TextField
          label="Neden"
          placeholder="Destek talebi #4821"
          hint="Denetim kaydına yazılır."
          error={form.formState.errors.reason?.message}
          {...form.register('reason')}
        />
      </form>
    </Modal>
  );
}
