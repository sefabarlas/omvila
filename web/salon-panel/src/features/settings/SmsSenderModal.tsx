import { zodResolver } from '@hookform/resolvers/zod';
import { Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useRequestSmsSender } from './notification-api';
import { normalizeSenderName, senderNamePattern } from './notification-text';
import { describeSaveError } from './save-error';

const formId = 'gonderen-adi';
const nameMessage = '3–11 karakter; harf ve rakam kullanın, en az bir harf olsun.';

// SmsSenderRequest.name: giriş büyük Latin harfe çevrilip desenle denetlenir.
const schema = z.object({
  name: z
    .string()
    .transform(normalizeSenderName)
    .pipe(z.string().regex(senderNamePattern, nameMessage)),
});

type FormInput = z.input<typeof schema>;
type FormValues = z.output<typeof schema>;

export interface SmsSenderModalProps {
  open: boolean;
  onClose: () => void;
}

/** Gönderen adı başvurusu: onaylanana kadar SMS'ler mevcut adla (OMVILA veya önceki onaylı ad) gider. */
export function SmsSenderModal({ open, onClose }: SmsSenderModalProps) {
  const toast = useToast();
  const request = useRequestSmsSender();
  // Aynı başvurunun tekrarı (ağ hatasından sonra yeniden gönder) aynı anahtarla gider.
  const [key, setKey] = useState(() => crypto.randomUUID());
  const form = useForm<FormInput, unknown, FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { name: '' },
  });
  const { errors } = form.formState;

  const close = () => {
    form.reset({ name: '' });
    setKey(crypto.randomUUID());
    onClose();
  };

  const onSubmit = form.handleSubmit(async ({ name }) => {
    try {
      await request.mutateAsync({ key, name });
      toast.show({ tone: 'basari', title: 'Başvurunuz alındı' });
      close();
    } catch (error) {
      const view = describeSaveError(error);
      if (view.fields.name) {
        form.setError('name', { message: nameMessage });
        return;
      }

      toast.show({ tone: 'hata', title: view.message });
    }
  });

  return (
    <Modal
      open={open}
      onClose={close}
      title="Gönderen adı başvurusu"
      description="Operatör onayından sonra SMS'ler bu adla gider. Onaylanana kadar mevcut ad kullanılır."
      footer={
        <>
          <Button onClick={close}>Vazgeç</Button>
          <Button variant="primary" type="submit" form={formId} loading={request.isPending}>
            Başvur
          </Button>
        </>
      }
    >
      <form id={formId} noValidate onSubmit={(e) => void onSubmit(e)}>
        <TextField
          label="Gönderen adı"
          placeholder="KUAFORELIF"
          autoCapitalize="characters"
          autoComplete="off"
          spellCheck={false}
          maxLength={20}
          hint="Türkçe karakter, boşluk ve noktalama kullanılmaz; büyük harfe çevrilir."
          error={errors.name?.message}
          {...form.register('name', {
            onChange: (event: { target: HTMLInputElement }) => {
              event.target.value = normalizeSenderName(event.target.value);
            },
          })}
        />
      </form>
    </Modal>
  );
}
