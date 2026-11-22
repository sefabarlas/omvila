import { zodResolver } from '@hookform/resolvers/zod';
import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatCount } from '@omvila/ui-web/format';
import { Banner, Button, Modal, Select, TextField, useToast } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { useApi } from '@/app/api-context';
import { plansQuery, useChangePlan } from './api';
import styles from './Brands.module.css';
import type { BrandDetail } from './labels';

const schema = z.object({
  planCode: z.string().min(1, 'Plan seçin.'),
  billingCycle: z.enum(['Monthly', 'Yearly']),
  reason: z
    .string()
    .trim()
    .min(3, 'Nedeni en az 3 harfle yazın.')
    .max(300, 'Neden en fazla 300 karakter olabilir.'),
});

type Form = z.infer<typeof schema>;

type Failure =
  { kind: 'conflict' } | { kind: 'limit'; text: string } | { kind: 'other'; text: string };

/** 409 plan_limit_exceeded ayrıntısı (meta.staffCount/staffLimit, branchCount/includedBranchCount). */
function limitText(meta: Record<string, unknown>): string {
  const parts: string[] = [];
  const n = (key: string) => (typeof meta[key] === 'number' ? meta[key] : undefined);
  const staff = n('staffCount');
  const staffLimit = n('staffLimit');
  const branches = n('branchCount');
  const branchLimit = n('includedBranchCount');
  if (staff !== undefined && staffLimit !== undefined && staff > staffLimit) {
    parts.push(`${formatCount(staff)} etkin personel, plan sınırı ${formatCount(staffLimit)}`);
  }

  if (branches !== undefined && branchLimit !== undefined && branches > branchLimit) {
    parts.push(`${formatCount(branches)} şube, plana dahil ${formatCount(branchLimit)}`);
  }

  return parts.length > 0 ? `${parts.join('; ')}.` : errorMessage('plan_limit_exceeded');
}

/**
 * "Planı değiştir" (adminChangePlan, If-Match): yeni plan hemen geçerli, yeni fiyat sonraki faturadan. Yalnız etkin
 * planlar seçilir; 409 plan_limit_exceeded sınırı yazar, 412 "Kayıt çakışması" + Yenile.
 */
export function ChangePlanModal({
  brand,
  onClose,
  onRefresh,
}: {
  brand: BrandDetail;
  onClose: () => void;
  onRefresh: () => void;
}) {
  const toast = useToast();
  const plans = useQuery(plansQuery(useApi()));
  const change = useChangePlan(brand);
  const [failure, setFailure] = useState<Failure | null>(null);
  const form = useForm<Form>({
    resolver: zodResolver(schema),
    defaultValues: {
      planCode: brand.subscription.planCode,
      billingCycle: brand.subscription.billingCycle,
      reason: '',
    },
  });
  const options = (plans.data?.plans ?? [])
    .filter((plan) => plan.isActive || plan.code === brand.subscription.planCode)
    .map((plan) => ({ value: plan.code, label: plan.name }));

  const submit = form.handleSubmit(async (values) => {
    setFailure(null);
    if (
      values.planCode === brand.subscription.planCode &&
      values.billingCycle === brand.subscription.billingCycle
    ) {
      form.setError('planCode', { message: 'Marka zaten bu planda.' });
      return;
    }

    try {
      const detail = await change.mutateAsync(values);
      toast.show({
        tone: 'basari',
        title: 'Plan değişti',
        message: `${detail.name} artık ${detail.subscription.planName} planında.`,
      });
      onClose();
    } catch (problem) {
      if (problem instanceof ApiProblem && problem.status === 412) {
        setFailure({ kind: 'conflict' });
      } else if (problem instanceof ApiProblem && problem.code === 'plan_limit_exceeded') {
        setFailure({ kind: 'limit', text: limitText(problem.meta) });
      } else {
        setFailure({
          kind: 'other',
          text: errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
        });
      }
    }
  });

  return (
    <Modal
      open
      onClose={onClose}
      title="Planı değiştir"
      description={`${brand.name} · şu an ${brand.subscription.planName}`}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form="plan-degistir"
            loading={form.formState.isSubmitting}
            loadingLabel="Değiştiriliyor…"
            disabled={plans.isPending}
          >
            Planı değiştir
          </Button>
        </>
      }
    >
      <form
        id="plan-degistir"
        className={styles.form}
        onSubmit={(event) => void submit(event)}
        noValidate
      >
        {failure?.kind === 'conflict' ? (
          <Banner
            tone="uyari"
            title="Kayıt çakışması."
            action={
              <Button
                size="sm"
                onClick={() => {
                  setFailure(null);
                  onRefresh();
                }}
              >
                Yenile
              </Button>
            }
          >
            Marka başka biri tarafından değiştirildi. Güncel bilgilerle yeniden deneyin.
          </Banner>
        ) : null}
        {failure?.kind === 'limit' ? (
          <Banner tone="hata" title="Bu plan markaya yetmiyor.">
            {failure.text}
          </Banner>
        ) : null}
        {failure?.kind === 'other' ? (
          <Banner tone="hata" title="Plan değiştirilemedi.">
            {failure.text}
          </Banner>
        ) : null}
        {plans.isError ? (
          <Banner tone="hata" title="Planlar yüklenemedi.">
            {errorMessage(plans.error instanceof ApiProblem ? plans.error.code : undefined)}
          </Banner>
        ) : null}
        <Select
          key={options.length}
          label="Plan"
          options={options}
          error={form.formState.errors.planCode?.message}
          {...form.register('planCode')}
        />
        <Select
          label="Fatura dönemi"
          options={[
            { value: 'Monthly', label: 'Aylık' },
            { value: 'Yearly', label: 'Yıllık' },
          ]}
          {...form.register('billingCycle')}
        />
        <TextField
          label="Neden"
          placeholder="Şube sayısı arttı"
          hint="Denetim kaydına yazılır. Yeni fiyat bir sonraki faturadan uygulanır."
          error={form.formState.errors.reason?.message}
          {...form.register('reason')}
        />
      </form>
    </Modal>
  );
}
