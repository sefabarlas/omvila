import { zodResolver } from '@hookform/resolvers/zod';
import { Banner, Button, Modal, TextArea, useToast } from '@omvila/ui-web/panel';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useResolveNotice } from './api';
import type { NoticeDetail } from './api';
import styles from './Approvals.module.css';
import { decisionError } from './decision-error';

// Sözleşme (AdminPaymentNoticeResolution): not 3–300 karakter; Bulunamadı'da isteğe bağlı, retde zorunlu.
const message = z.string().trim().max(300, 'Not en fazla 300 karakter olabilir.');
const schemas = {
  'not-found': z.object({
    message: message.refine((v) => v === '' || v.length >= 3, 'Notu en az 3 harfle yazın.'),
  }),
  reject: z.object({ message: message.min(3, 'Reddetme nedenini yazın.') }),
};

const copy = {
  'not-found': {
    title: 'Bulunamadı, markaya yaz',
    submit: 'Markaya yaz',
    loading: 'Yazılıyor…',
    placeholder:
      '18 Kasım tarihli havaleyi hesabımızda göremedik. Dekontu kontrol edip yeniden bildirin.',
    hint: 'İsteğe bağlı. Markanın bildiriminde görünür; salon yeniden bildirebilir.',
    done: 'Markaya yazıldı',
    doneText: (brand: string) => `${brand} havalenin bulunamadığını görecek.`,
  },
  reject: {
    title: 'Havale bildirimini reddet',
    submit: 'Bildirimi reddet',
    loading: 'Reddediliyor…',
    placeholder: 'Bu bildirim aynı havale için ikinci kez gönderildi.',
    hint: 'Geçersiz veya yinelenen bildirim için. Not markanın bildiriminde görünür.',
    done: 'Bildirim reddedildi',
    doneText: (brand: string) => `${brand} markasına bildirim gitti.`,
  },
} as const;

/** "Bulunamadı, markaya yaz" (NotFound) ve "Reddet" (Rejected); not markaya gider, denetime yazılır. */
export function ResolveNoticeModal({
  kind,
  notice,
  onClose,
}: {
  kind: 'not-found' | 'reject';
  notice: NoticeDetail;
  onClose: () => void;
}) {
  const toast = useToast();
  const resolve = useResolveNotice(kind);
  const text = copy[kind];
  const form = useForm<{ message: string }>({
    resolver: zodResolver(schemas[kind]),
    defaultValues: { message: '' },
  });

  const submit = form.handleSubmit(async (values) => {
    try {
      await resolve.mutateAsync({
        noticeId: notice.id,
        version: notice.version,
        message: values.message.trim(),
      });
      toast.show({ tone: 'basari', title: text.done, message: text.doneText(notice.brandName) });
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
      title={text.title}
      description={`${notice.brandName} · ${notice.transferCode}`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant={kind === 'reject' ? 'danger' : 'primary'}
            type="submit"
            form="havale-karar"
            loading={form.formState.isSubmitting}
            loadingLabel={text.loading}
          >
            {text.submit}
          </Button>
        </>
      }
    >
      <form
        id="havale-karar"
        className={styles.form}
        onSubmit={(event) => void submit(event)}
        noValidate
      >
        {form.formState.errors.root?.message ? (
          <Banner tone="hata" title="Kaydedilemedi.">
            {form.formState.errors.root.message}
          </Banner>
        ) : null}
        <TextArea
          label="Markaya not"
          placeholder={text.placeholder}
          hint={text.hint}
          error={form.formState.errors.message?.message}
          {...form.register('message')}
        />
      </form>
    </Modal>
  );
}
