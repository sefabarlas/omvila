import { zodResolver } from '@hookform/resolvers/zod';
import { Banner, Button, Modal, TextArea, useToast } from '@omvila/ui-web/panel';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useRejectSender } from './api';
import type { QueueSender } from './api';
import styles from './Approvals.module.css';
import { decisionError } from './decision-error';

// Sözleşme (AdminSmsSenderRejection): neden 3–200 karakter; salonun ayarlarında görünür.
const schema = z.object({
  reason: z
    .string()
    .trim()
    .min(3, 'Nedeni en az 3 harfle yazın.')
    .max(200, 'Neden en fazla 200 karakter olabilir.'),
});

type Form = z.infer<typeof schema>;

/** SMS gönderen adını reddet: neden salonun Bildirimler ve SMS ayarında görünür; önceki ad sürer. */
export function RejectSenderModal({
  brandName,
  senderId,
  sender,
  onClose,
}: {
  brandName: string;
  senderId: string;
  sender: QueueSender;
  onClose: () => void;
}) {
  const toast = useToast();
  const reject = useRejectSender();
  const form = useForm<Form>({ resolver: zodResolver(schema), defaultValues: { reason: '' } });

  const submit = form.handleSubmit(async ({ reason }) => {
    try {
      await reject.mutateAsync({ senderId, version: sender.version, reason });
      toast.show({
        tone: 'basari',
        title: 'SMS gönderen adı reddedildi',
        message: `${brandName} SMS'leri ${sender.currentEffectiveName} adıyla gitmeye devam eder.`,
      });
      onClose();
    } catch (problem) {
      const failure = decisionError(problem);
      form.setError('root', { message: `${failure.title} ${failure.text}` });
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title="SMS gönderen adını reddet"
      description={`${brandName} · ${sender.requestedName}`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="danger"
            type="submit"
            form="sms-adi-reddet"
            loading={form.formState.isSubmitting}
            loadingLabel="Reddediliyor…"
          >
            Adı reddet
          </Button>
        </>
      }
    >
      <form
        id="sms-adi-reddet"
        className={styles.form}
        onSubmit={(event) => void submit(event)}
        noValidate
      >
        {form.formState.errors.root?.message ? (
          <Banner tone="hata" title="Reddedilemedi.">
            {form.formState.errors.root.message}
          </Banner>
        ) : null}
        <TextArea
          label="Neden"
          placeholder="Başlık işletme unvanıyla uyuşmuyor."
          hint="Salon bu nedeni Bildirimler ve SMS ayarında görür."
          error={form.formState.errors.reason?.message}
          {...form.register('reason')}
        />
      </form>
    </Modal>
  );
}
