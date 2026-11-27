import { zodResolver } from '@hookform/resolvers/zod';
import { Button, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useRequestDataExport } from './privacy-api';
import roles from './Roles.module.css';
import { describeSaveError } from './save-error';

const formId = 'veri-disa-aktar';
const emailMessage = 'Geçerli bir e-posta adresi girin: ornek@alanadi.com';

const schema = z.object({ email: z.email(emailMessage).max(160, emailMessage) });
type FormValues = z.infer<typeof schema>;

/**
 * "Tüm verileri dışa aktar": müşteriler, randevular ve ödemeler CSV olarak hazırlanır, indirme bağlantısı bu adrese
 * gider. Kullanıcı kaydında e-posta olmadığından adres her istekte sorulur (acik-sorular.md T-121 (3), T-127).
 */
export function DataExportModal({ onClose }: { onClose: () => void }) {
  const toast = useToast();
  const request = useRequestDataExport();
  // Ağ hatasından sonra yeniden gönderim aynı anahtarla gider.
  const [key] = useState(() => crypto.randomUUID());
  const form = useForm<FormValues>({ resolver: zodResolver(schema), defaultValues: { email: '' } });
  const { errors } = form.formState;

  const onSubmit = form.handleSubmit(async ({ email }) => {
    try {
      await request.mutateAsync({ key, email });
      toast.show({ tone: 'basari', title: 'Dışa aktarma isteğiniz alındı' });
      onClose();
    } catch (error) {
      const view = describeSaveError(error);
      if (view.fields.email) {
        form.setError('email', { message: emailMessage });
        return;
      }

      toast.show({ tone: 'hata', title: view.message });
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title="Tüm verileri dışa aktar"
      description="Müşteriler, randevular ve ödemeler CSV olarak hazırlanır; indirme bağlantısı e-postanıza gönderilir."
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button variant="primary" type="submit" form={formId} loading={request.isPending}>
            Dışa aktarma iste
          </Button>
        </>
      }
    >
      <form
        id={formId}
        className={roles.form}
        noValidate
        onSubmit={(e) => {
          // Pencere KVKK formunun içinden açılır; React gönderimi portal üzerinden üst forma taşımasın.
          e.stopPropagation();
          void onSubmit(e);
        }}
      >
        <TextField
          label="E-posta"
          type="email"
          autoComplete="email"
          placeholder="ornek@alanadi.com"
          error={errors.email?.message}
          {...form.register('email')}
        />
      </form>
    </Modal>
  );
}
