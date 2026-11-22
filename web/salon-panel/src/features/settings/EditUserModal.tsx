import { zodResolver } from '@hookform/resolvers/zod';
import { Banner, Button, Modal, Select, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { useForm, useWatch } from 'react-hook-form';
import { z } from 'zod';
import { useSession } from '@/app/session';
import { CheckboxGroup } from '@/shared/CheckboxGroup';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { membershipRoleLabels, useUpdateUser } from './roles-api';
import type { ManageUser } from './roles-api';
import { describeSaveError } from './save-error';
import roles from './Roles.module.css';
import styles from './Settings.module.css';

const formId = 'kullanici-duzenle';
const branchRequired = 'En az bir şube seçin.';

const schema = z
  .object({
    role: z.enum(['Owner', 'BranchManager', 'Staff']),
    branchIds: z.array(z.string()),
  })
  .superRefine((values, ctx) => {
    if (values.role !== 'Owner' && values.branchIds.length === 0) {
      ctx.addIssue({ code: 'custom', path: ['branchIds'], message: branchRequired });
    }
  });

type FormValues = z.infer<typeof schema>;

const roleOptions = (['Owner', 'BranchManager', 'Staff'] as const).map((value) => ({
  value,
  label: membershipRoleLabels[value],
}));

/**
 * Kullanıcının rolü ve şubeleri (AyarlarRoller "Düzenle"; K6). İşletme sahibi tüm şubeleri görür, şube seçimi yalnız
 * diğer rollerde. Markadaki son işletme sahibi başka role alınırsa sunucu 409 last_owner_required döner.
 */
export function EditUserModal({ user, onClose }: { user: ManageUser; onClose: () => void }) {
  const toast = useToast();
  const { session } = useSession();
  const update = useUpdateUser();
  const [problem, setProblem] = useState<string | null>(null);
  const [conflict, setConflict] = useState(false);
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { role: user.role, branchIds: user.branchIds },
  });
  const role = useWatch({ control: form.control, name: 'role' });
  const { errors } = form.formState;

  const onSubmit = form.handleSubmit(async (values) => {
    setProblem(null);
    try {
      await update.mutateAsync({
        userId: user.userId,
        version: user.version,
        body: { role: values.role, branchIds: values.role === 'Owner' ? [] : values.branchIds },
      });
      toast.show({ tone: 'basari', title: `${user.fullName} için rol kaydedildi` });
      onClose();
    } catch (error) {
      const view = describeSaveError(error);
      setConflict(view.conflict);
      if (view.fields.branchIds) {
        form.setError('branchIds', { message: branchRequired });
      } else if (!view.conflict) {
        setProblem(view.message);
      }
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title={user.fullName}
      description={`${user.phoneMasked} · rolü ve çalıştığı şubeler`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button variant="primary" type="submit" form={formId} loading={update.isPending}>
            Kaydet
          </Button>
        </>
      }
    >
      <form id={formId} className={roles.form} noValidate onSubmit={(e) => void onSubmit(e)}>
        {conflict ? <ConflictBanner onReload={onClose} /> : null}
        {problem ? (
          <Banner tone="hata" title="Rol kaydedilemedi">
            {problem}
          </Banner>
        ) : null}
        <Select label="Rol" options={roleOptions} {...form.register('role')} />
        {role === 'Owner' ? (
          <p className={styles.cardText}>İşletme sahibi tüm şubeleri ve ayarları görür.</p>
        ) : (
          <fieldset className={roles.group} aria-describedby="sube-hata">
            <legend className={roles.groupTitle}>Şubeler</legend>
            <CheckboxGroup
              control={form.control}
              name="branchIds"
              options={session.branches.map((b) => ({ value: b.id, label: b.name }))}
            />
            {errors.branchIds?.message ? (
              <p id="sube-hata" role="alert" className={roles.fieldError}>
                {errors.branchIds.message}
              </p>
            ) : null}
          </fieldset>
        )}
      </form>
    </Modal>
  );
}
