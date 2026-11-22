import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatMoney, parseMoneyInput } from '@omvila/ui-web/format';
import { Banner, Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { branchToday } from '@omvila/ui-web/time';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useClock } from '@/app/clock';
import { platformTimeZone } from '@/app/config';
import styles from '@/features/approvals/Approvals.module.css';
import { useCreateBankTransaction } from './api';

// Sözleşme sınırları (BankTransactionCreate): gönderen 2–160, açıklama ≤ 300, referans 3–80.
function schemaFor(today: string) {
  return z.object({
    valueDate: z
      .string()
      .regex(/^\d{4}-\d{2}-\d{2}$/, 'Değer tarihini seçin.')
      .refine((v) => v <= today, 'Değer tarihi bugünden sonra olamaz.'),
    amount: z
      .string()
      .refine((v) => (parseMoneyInput(v) ?? 0) > 0, 'Tutarı yazın (ör. 1.000 veya 850,50).'),
    senderName: z
      .string()
      .trim()
      .min(2, 'Gönderen adını yazın.')
      .max(160, 'Gönderen en fazla 160 karakter olabilir.'),
    description: z.string().trim().max(300, 'Açıklama en fazla 300 karakter olabilir.'),
    bankReference: z
      .string()
      .trim()
      .min(3, 'Bankanın hareket referansını yazın.')
      .max(80, 'Referans en fazla 80 karakter olabilir.'),
  });
}

type Form = z.infer<ReturnType<typeof schemaFor>>;

/** "Hareket ekle": CSV'de olmayan havaleyi elle girer (source Manual); aynı referans 409 bank_transaction_duplicate. */
export function AddBankTransactionModal({ onClose }: { onClose: () => void }) {
  const toast = useToast();
  const today = branchToday(useClock(), platformTimeZone);
  const create = useCreateBankTransaction();
  const form = useForm<Form>({
    resolver: zodResolver(schemaFor(today)),
    defaultValues: {
      valueDate: today,
      amount: '',
      senderName: '',
      description: '',
      bankReference: '',
    },
  });

  const submit = form.handleSubmit(async (values) => {
    try {
      const created = await create.mutateAsync({
        valueDate: values.valueDate,
        amount: { amountMinor: parseMoneyInput(values.amount) ?? 0, currency: 'TRY' },
        senderName: values.senderName,
        description: values.description === '' ? null : values.description,
        bankReference: values.bankReference,
      });
      toast.show({
        tone: 'basari',
        title: 'Hareket eklendi',
        message: `${created.senderName} · ${formatMoney(created.amount)}`,
      });
      onClose();
    } catch (problem) {
      if (problem instanceof ApiProblem && problem.code === 'bank_transaction_duplicate') {
        form.setError('bankReference', { message: errorMessage(problem.code) });
        return;
      }

      form.setError('root', {
        message: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      });
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title="Banka hareketi ekle"
      description="Dökümde olmayan bir havaleyi elle girin; denetim kaydına yazılır."
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="hareket-ekle"
            loading={form.formState.isSubmitting}
            loadingLabel="Ekleniyor…"
          >
            Hareketi ekle
          </Button>
        </>
      }
    >
      <form
        id="hareket-ekle"
        className={styles.form}
        onSubmit={(event) => void submit(event)}
        noValidate
      >
        {form.formState.errors.root?.message ? (
          <Banner tone="hata" title="Hareket eklenemedi.">
            {form.formState.errors.root.message}
          </Banner>
        ) : null}
        <TextField
          label="Değer tarihi"
          type="date"
          max={today}
          error={form.formState.errors.valueDate?.message}
          {...form.register('valueDate')}
        />
        <TextField
          label="Tutar (₺)"
          inputMode="decimal"
          placeholder="1.000"
          error={form.formState.errors.amount?.message}
          {...form.register('amount')}
        />
        <TextField
          label="Gönderen"
          placeholder="ELİF KAYA"
          hint="Dökümdeki gibi yazın."
          error={form.formState.errors.senderName?.message}
          {...form.register('senderName')}
        />
        <TextField
          label="Açıklama"
          placeholder="ABN-KE-1125"
          hint="İsteğe bağlı. Açıklama kodu varsa eşleştirmede aranır."
          error={form.formState.errors.description?.message}
          {...form.register('description')}
        />
        <TextField
          label="Banka referansı"
          placeholder="20261118-001204"
          error={form.formState.errors.bankReference?.message}
          {...form.register('bankReference')}
        />
      </form>
    </Modal>
  );
}
