import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Banner, Button, Checkbox, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { useUpdatePlans } from './api';
import type { PlanCatalog, PlanUpdate } from './api';
import { moneyField, parseCountField, parseMoneyField } from './labels';
import styles from './Plans.module.css';

type Features = PlanUpdate['features'];

interface Row {
  code: string;
  name: string;
  description: string;
  monthlyPrice: string;
  yearlyPrice: string;
  includedBranchCount: string;
  extraBranchPrice: string;
  staffLimit: string;
  includedSmsCredits: string;
  features: Features;
  isPublic: boolean;
  isActive: boolean;
}

type Errors = Record<string, string>;

const featureLabels: Record<keyof Features, string> = {
  walkInQueue: 'Kapıda sıra',
  waitlist: 'Bekleme listesi',
  packages: 'Paketler ve prim raporu',
  crossBranchReports: 'Şubeler arası rapor',
  prioritySupport: 'Öncelikli destek',
};

const countText = (value: number | null) => (value === null ? '' : String(value));

/** Satırları sözleşmeye çevirir; hatalı alanlar `i.alan` anahtarıyla döner. */
export function toPlanUpdates(rows: Row[]): { plans: PlanUpdate[]; errors: Errors } {
  const errors: Errors = {};
  const plans = rows.map((row, i): PlanUpdate => {
    const money = (key: 'monthlyPrice' | 'yearlyPrice' | 'extraBranchPrice') => {
      const value = parseMoneyField(row[key]);
      if (value === undefined)
        errors[`${String(i)}.${key}`] = 'Tutarı 1.250 veya 850,50 gibi yazın.';
      return value ?? null;
    };
    const count = (
      key: 'includedBranchCount' | 'staffLimit' | 'includedSmsCredits',
      min: number,
    ) => {
      const value = parseCountField(row[key], min);
      if (value === undefined)
        errors[`${String(i)}.${key}`] = `En az ${String(min)} olan tam sayı yazın.`;
      return value ?? null;
    };
    const name = row.name.trim();
    if (name.length < 2 || name.length > 60)
      errors[`${String(i)}.name`] = 'Ad 2–60 karakter olmalı.';
    const description = row.description.trim();
    if (description.length > 120) errors[`${String(i)}.description`] = 'En fazla 120 karakter.';
    return {
      code: row.code,
      name,
      description: description === '' ? null : description,
      monthlyPrice: money('monthlyPrice'),
      yearlyPrice: money('yearlyPrice'),
      includedBranchCount: count('includedBranchCount', 1),
      extraBranchPrice: money('extraBranchPrice'),
      staffLimit: count('staffLimit', 1),
      includedSmsCredits: count('includedSmsCredits', 0),
      features: row.features,
      isPublic: row.isPublic,
      isActive: row.isActive,
    };
  });
  return { plans, errors };
}

/**
 * "Planı düzenle" (adminUpdatePlans, If-Match): ad, açıklama, fiyat, sınır ve özellikler. Boş fiyat "belirlenmedi",
 * boş sınır "sınırsız". Yeni kayıtlar için geçerli; mevcut aboneler korunur. Yönetici ve Finans.
 */
export function EditPlansModal({
  catalog,
  onClose,
  onRefresh,
}: {
  catalog: PlanCatalog;
  onClose: () => void;
  onRefresh: () => void;
}) {
  const toast = useToast();
  const update = useUpdatePlans();
  const [rows, setRows] = useState<Row[]>(() =>
    catalog.plans.map((plan) => ({
      code: plan.code,
      name: plan.name,
      description: plan.description ?? '',
      monthlyPrice: moneyField(plan.monthlyPrice),
      yearlyPrice: moneyField(plan.yearlyPrice),
      includedBranchCount: countText(plan.includedBranchCount),
      extraBranchPrice: moneyField(plan.extraBranchPrice),
      staffLimit: countText(plan.staffLimit),
      includedSmsCredits: countText(plan.includedSmsCredits),
      features: plan.features,
      isPublic: plan.isPublic,
      isActive: plan.isActive,
    })),
  );
  const [errors, setErrors] = useState<Errors>({});
  const [failure, setFailure] = useState<{ conflict: true } | string | null>(null);
  const set = (i: number, patch: Partial<Row>) => {
    setRows((current) => current.map((row, j) => (j === i ? { ...row, ...patch } : row)));
  };

  const submit = async () => {
    setFailure(null);
    const result = toPlanUpdates(rows);
    setErrors(result.errors);
    if (Object.keys(result.errors).length > 0) return;
    try {
      await update.mutateAsync({ version: catalog.version, plans: result.plans });
      toast.show({
        tone: 'basari',
        title: 'Planlar kaydedildi',
        message: 'Yeni kayıtlar ve sonraki faturalar için geçerli.',
      });
      onClose();
    } catch (problem) {
      setFailure(
        problem instanceof ApiProblem && problem.status === 412
          ? { conflict: true }
          : errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      );
    }
  };

  const field = (i: number, key: keyof Row, label: string, extra: Record<string, string> = {}) => (
    <TextField
      label={label}
      value={rows[i]?.[key] as string}
      error={errors[`${String(i)}.${key}`]}
      onChange={(event) => {
        set(i, { [key]: event.target.value });
      }}
      {...extra}
    />
  );

  return (
    <Modal
      open
      size="lg"
      onClose={onClose}
      title="Planı düzenle"
      description="Yeni kayıtlar için geçerli · mevcut aboneler korunur"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={update.isPending}
            loadingLabel="Kaydediliyor…"
            onClick={() => void submit()}
          >
            Planları kaydet
          </Button>
        </>
      }
    >
      <div className={styles.editor}>
        {typeof failure === 'object' && failure !== null ? (
          <Banner
            tone="uyari"
            title="Kayıt çakışması."
            action={
              <Button
                size="sm"
                onClick={() => {
                  onRefresh();
                  onClose();
                }}
              >
                Yenile
              </Button>
            }
          >
            Planlar başka biri tarafından değiştirildi. Güncel değerlerle yeniden düzenleyin.
          </Banner>
        ) : failure ? (
          <Banner tone="hata" title="Planlar kaydedilemedi.">
            {failure}
          </Banner>
        ) : null}
        {Object.keys(errors).length > 0 ? (
          <Banner tone="hata" title="Bazı alanlar hatalı.">
            İşaretli alanları düzeltin.
          </Banner>
        ) : null}
        {rows.map((row, i) => (
          <fieldset key={row.code} className={styles.planFields}>
            <legend className={styles.legend}>{row.name || row.code}</legend>
            <div className={styles.fieldGrid}>
              {field(i, 'name', 'Ad')}
              {field(i, 'description', 'Açıklama', { placeholder: 'Küçük ve orta salonlar' })}
              {field(i, 'monthlyPrice', 'Aylık fiyat (₺)', {
                inputMode: 'decimal',
                hint: 'Boş: belirlenmedi',
              })}
              {field(i, 'yearlyPrice', 'Yıllık fiyat (₺)', {
                inputMode: 'decimal',
                hint: 'Boş: yıllık yok',
              })}
              {field(i, 'staffLimit', 'Personel sınırı', {
                inputMode: 'numeric',
                hint: 'Boş: sınırsız',
              })}
              {field(i, 'includedBranchCount', 'Dahil şube', {
                inputMode: 'numeric',
                hint: 'Boş: sınırsız',
              })}
              {field(i, 'extraBranchPrice', 'Ek şube ücreti (₺/ay)', {
                inputMode: 'decimal',
                hint: 'Boş: ek şube yok',
              })}
              {field(i, 'includedSmsCredits', 'Ayda dahil SMS', {
                inputMode: 'numeric',
                hint: 'Boş: tanımsız',
              })}
            </div>
            <div className={styles.checks}>
              {(Object.keys(featureLabels) as (keyof Features)[]).map((key) => (
                <Checkbox
                  key={key}
                  label={featureLabels[key]}
                  checked={row.features[key]}
                  onChange={(event) => {
                    set(i, { features: { ...row.features, [key]: event.target.checked } });
                  }}
                />
              ))}
              <Checkbox
                label="Yeni kayıtlarda seçilebilir"
                checked={row.isActive}
                onChange={(event) => {
                  set(i, { isActive: event.target.checked });
                }}
              />
              <Checkbox
                label="Tanıtım sitesinde göster"
                checked={row.isPublic}
                onChange={(event) => {
                  set(i, { isPublic: event.target.checked });
                }}
              />
            </div>
          </fieldset>
        ))}
      </div>
    </Modal>
  );
}
