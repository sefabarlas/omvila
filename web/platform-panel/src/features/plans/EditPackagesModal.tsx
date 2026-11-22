import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { Banner, Button, Checkbox, Modal, TextField, useToast } from '@omvila/ui-web/panel';
import { useState } from 'react';
import { useUpdatePackages } from './api';
import type { CreditPackage, PackageCatalog } from './api';
import { moneyField, parseCountField, parseMoneyField } from './labels';
import styles from './Plans.module.css';

interface Row {
  code: string;
  name: string;
  credits: string;
  price: string;
  isActive: boolean;
}

/**
 * Kontör paketlerini düzenle (adminUpdateCreditPackages, If-Match): ad, kontör, fiyat (boş = belirlenmedi), satışta.
 * Bekleyen havale bildirimlerinin beklenen tutarı değişmez. Yönetici ve Finans.
 */
export function EditPackagesModal({
  catalog,
  onClose,
  onRefresh,
}: {
  catalog: PackageCatalog;
  onClose: () => void;
  onRefresh: () => void;
}) {
  const toast = useToast();
  const update = useUpdatePackages();
  const [rows, setRows] = useState<Row[]>(() =>
    catalog.packages.map((p) => ({
      code: p.code,
      name: p.name,
      credits: String(p.credits),
      price: moneyField(p.price),
      isActive: p.isActive,
    })),
  );
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [failure, setFailure] = useState<{ conflict: true } | string | null>(null);
  const set = (i: number, patch: Partial<Row>) => {
    setRows((current) => current.map((row, j) => (j === i ? { ...row, ...patch } : row)));
  };

  const submit = async () => {
    setFailure(null);
    const next: Record<string, string> = {};
    const packages = rows.map((row, i): CreditPackage => {
      const name = row.name.trim();
      if (name.length < 2 || name.length > 60)
        next[`${String(i)}.name`] = 'Ad 2–60 karakter olmalı.';
      const credits = parseCountField(row.credits, 1);
      if (credits === null || credits === undefined)
        next[`${String(i)}.credits`] = 'En az 1 kontör yazın.';
      const price = parseMoneyField(row.price);
      if (price === undefined) next[`${String(i)}.price`] = 'Tutarı 1.250 veya 850,50 gibi yazın.';
      return {
        code: row.code,
        name,
        credits: credits ?? 1,
        price: price ?? null,
        isActive: row.isActive,
      };
    });
    setErrors(next);
    if (Object.keys(next).length > 0) return;
    try {
      await update.mutateAsync({ version: catalog.version, packages });
      toast.show({ tone: 'basari', title: 'Kontör paketleri kaydedildi' });
      onClose();
    } catch (problem) {
      setFailure(
        problem instanceof ApiProblem && problem.status === 412
          ? { conflict: true }
          : errorMessage(problem instanceof ApiProblem ? problem.code : undefined),
      );
    }
  };

  return (
    <Modal
      open
      size="lg"
      onClose={onClose}
      title="Kontör paketlerini düzenle"
      description="Bekleyen havale bildirimlerinin tutarı değişmez."
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            loading={update.isPending}
            loadingLabel="Kaydediliyor…"
            onClick={() => void submit()}
          >
            Paketleri kaydet
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
            Paketler başka biri tarafından değiştirildi. Güncel değerlerle yeniden düzenleyin.
          </Banner>
        ) : failure ? (
          <Banner tone="hata" title="Paketler kaydedilemedi.">
            {failure}
          </Banner>
        ) : null}
        {rows.map((row, i) => (
          <fieldset key={row.code} className={styles.planFields}>
            <legend className={styles.legend}>{row.name || row.code}</legend>
            <div className={styles.fieldGrid}>
              <TextField
                label="Paket adı"
                value={row.name}
                error={errors[`${String(i)}.name`]}
                onChange={(event) => {
                  set(i, { name: event.target.value });
                }}
              />
              <TextField
                label="SMS"
                inputMode="numeric"
                value={row.credits}
                error={errors[`${String(i)}.credits`]}
                onChange={(event) => {
                  set(i, { credits: event.target.value });
                }}
              />
              <TextField
                label="Fiyat (₺)"
                inputMode="decimal"
                hint="Boş: belirlenmedi"
                value={row.price}
                error={errors[`${String(i)}.price`]}
                onChange={(event) => {
                  set(i, { price: event.target.value });
                }}
              />
            </div>
            <Checkbox
              label="Satışta"
              checked={row.isActive}
              onChange={(event) => {
                set(i, { isActive: event.target.checked });
              }}
            />
          </fieldset>
        ))}
      </div>
    </Modal>
  );
}
