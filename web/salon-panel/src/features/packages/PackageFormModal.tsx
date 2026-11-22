import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { moneyInputValue, parseMoneyInput } from '@omvila/ui-web/format';
import { Banner, Button, Modal, Select, Switch, TextField, useToast } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { Plus } from 'lucide-react';
import { useState } from 'react';
import type { SyntheticEvent } from 'react';
import { useApi } from '@/app/api-context';
import { servicesQuery } from '@/features/services/api';
import { ConflictBanner } from '@/shared/ConflictBanner';
import { useCreatePackage, useUpdatePackage } from './api';
import type { PackageDefinition, PackageWrite } from './api';
import { validityLabel, validityOptions } from './labels';
import styles from './Packages.module.css';

const formId = 'paket-formu';
const maxItems = 10;
const maxSessions = 100;

interface Line {
  serviceId: string;
  sessions: string;
}

interface Errors {
  name?: string;
  price?: string;
  items?: string;
  commission?: string;
}

/**
 * Paket oluştur / düzenle (salon-paneli/PaketOlustur; package.manage). İçerik birden fazla hizmet: her satır hizmet ve
 * seans sayısı, "Kaldır", "+ Hizmet ekle" (aynı hizmet iki kez seçilemez). Satış primi boşsa satan personelin oranı
 * kullanılır (sözleşme: null). Düzenleme If-Match ile; 412'de "Kayıt çakışması". Değişiklik satılmış paketleri etkilemez.
 */
export function PackageFormModal({
  pkg,
  onClose,
}: {
  pkg?: PackageDefinition | undefined;
  onClose: () => void;
}) {
  const api = useApi();
  const toast = useToast();
  const services = useQuery(servicesQuery(api, undefined));
  const create = useCreatePackage();
  const update = useUpdatePackage();
  const [key] = useState(() => idempotencyKey()['Idempotency-Key']);
  const [name, setName] = useState(pkg?.name ?? '');
  const [price, setPrice] = useState(pkg ? moneyInputValue(pkg.price.amountMinor) : '');
  const [lines, setLines] = useState<Line[]>(
    pkg
      ? pkg.items.map((i) => ({ serviceId: i.serviceId, sessions: String(i.sessionCount) }))
      : [{ serviceId: '', sessions: '' }],
  );
  const [validityDays, setValidityDays] = useState(pkg?.validityDays ?? 90);
  const [commission, setCommission] = useState(
    pkg?.saleCommissionPercent != null ? String(pkg.saleCommissionPercent) : '',
  );
  const [allBranches, setAllBranches] = useState(pkg?.isAvailableAllBranches ?? true);
  const [errors, setErrors] = useState<Errors>({});
  const [problem, setProblem] = useState<string | null>(null);
  const [conflict, setConflict] = useState(false);
  const active = (services.data?.items ?? []).filter((s) => s.isActive);
  const serviceName = new Map([
    ...(pkg?.items.map((i) => [i.serviceId, i.serviceName] as const) ?? []),
    ...active.map((s) => [s.id, s.name] as const),
  ]);
  const options = validityOptions.some((o) => o.days === validityDays)
    ? validityOptions
    : [...validityOptions, { days: validityDays, label: validityLabel(validityDays) }];
  const saving = create.isPending || update.isPending;

  const setLine = (index: number, patch: Partial<Line>) => {
    setLines((list) => list.map((line, i) => (i === index ? { ...line, ...patch } : line)));
  };

  const validate = (): PackageWrite | null => {
    const next: Errors = {};
    const trimmed = name.trim();
    if (trimmed.length < 2 || trimmed.length > 80) {
      next.name = 'Paket adı 2–80 karakter olmalı.';
    }

    const minor = parseMoneyInput(price);
    if (minor === null || minor <= 0) {
      next.price = 'Fiyatı yazın: 1.700';
    }

    const items = lines.map((line) => ({
      serviceId: line.serviceId,
      sessionCount: Number(line.sessions),
    }));
    if (
      items.length === 0 ||
      items.some(
        (i) =>
          i.serviceId === '' ||
          !Number.isInteger(i.sessionCount) ||
          i.sessionCount < 1 ||
          i.sessionCount > maxSessions,
      )
    ) {
      next.items = `Her satırda bir hizmet ve 1–${String(maxSessions)} arası seans seçin.`;
    }

    const percent = commission.trim() === '' ? null : Number(commission.replace(',', '.'));
    if (percent !== null && (Number.isNaN(percent) || percent < 0 || percent > 100)) {
      next.commission = 'Satış primi 0 ile 100 arasında olmalı.';
    }

    setErrors(next);
    if (Object.keys(next).length > 0 || minor === null) {
      return null;
    }

    return {
      name: trimmed,
      price: { amountMinor: minor, currency: pkg?.price.currency ?? 'TRY' },
      items,
      validityDays,
      saleCommissionPercent: percent,
      isAvailableAllBranches: allBranches,
      isActive: pkg?.isActive ?? true,
    };
  };

  const onError = (error: unknown) => {
    if (error instanceof ApiProblem && error.status === 412) {
      setConflict(true);
      return;
    }

    if (error instanceof ApiProblem && error.errors.items) {
      setErrors((e) => ({ ...e, items: 'Hizmetler etkin olmalı ve tekrar etmemeli.' }));
      return;
    }

    setProblem(errorMessage(error instanceof ApiProblem ? error.code : undefined));
  };

  const submit = (event: SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();
    setProblem(null);
    const body = validate();
    if (!body) {
      return;
    }

    const done = (title: string) => () => {
      toast.show({ tone: 'basari', title, message: body.name });
      onClose();
    };
    if (pkg) {
      update.mutate({ pkg, body }, { onSuccess: done('Paket güncellendi'), onError });
    } else {
      create.mutate({ key, body }, { onSuccess: done('Paket oluşturuldu'), onError });
    }
  };

  return (
    <Modal
      open
      onClose={onClose}
      title={pkg ? 'Paketi düzenle' : 'Paket oluştur'}
      size="lg"
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            type="submit"
            form={formId}
            loading={saving}
            loadingLabel="Kaydediliyor…"
          >
            {pkg ? 'Değişiklikleri kaydet' : 'Paketi oluştur'}
          </Button>
        </>
      }
    >
      <form id={formId} className={styles.form} noValidate onSubmit={submit}>
        {conflict ? <ConflictBanner onReload={onClose} /> : null}
        {problem ? <Banner tone="hata" title={problem} /> : null}
        <div className={styles.row}>
          <TextField
            label="Paket adı"
            placeholder="Manikür · 6 seans"
            maxLength={80}
            value={name}
            error={errors.name}
            onChange={(event) => {
              setName(event.target.value);
            }}
          />
          <TextField
            label="Fiyat"
            inputMode="decimal"
            placeholder="1.700"
            suffix="₺"
            value={price}
            error={errors.price}
            onChange={(event) => {
              setPrice(event.target.value);
            }}
          />
        </div>
        <section className={styles.contents} aria-labelledby="paket-icerik">
          <h3 id="paket-icerik" className={styles.sectionTitle}>
            İçerik
          </h3>
          <p className={styles.hint}>
            Bir pakette birden fazla hizmet olabilir; kalan seans hizmet başına izlenir.
          </p>
          <ul className={styles.lines}>
            {lines.map((line, index) => {
              const taken = new Set(lines.filter((_, i) => i !== index).map((l) => l.serviceId));
              const label = serviceName.get(line.serviceId);
              return (
                <li key={index} className={styles.line}>
                  {line.serviceId && label ? (
                    <strong className={styles.lineName}>{label}</strong>
                  ) : (
                    <Select
                      label={`${String(index + 1)}. hizmet`}
                      hideLabel
                      value={line.serviceId}
                      options={[
                        { value: '', label: services.isPending ? 'Yükleniyor…' : 'Hizmet seçin' },
                        ...active
                          .filter((s) => !taken.has(s.id))
                          .map((s) => ({ value: s.id, label: s.name })),
                      ]}
                      onChange={(event) => {
                        setLine(index, { serviceId: event.target.value });
                      }}
                    />
                  )}
                  <TextField
                    label={`${label ?? `${String(index + 1)}. hizmet`} seans sayısı`}
                    hideLabel
                    inputMode="numeric"
                    placeholder="6"
                    suffix="seans"
                    className={styles.sessions}
                    value={line.sessions}
                    onChange={(event) => {
                      setLine(index, { sessions: event.target.value.replace(/\D/g, '') });
                    }}
                  />
                  <Button
                    size="sm"
                    variant="ghost"
                    aria-label={`Kaldır: ${label ?? `${String(index + 1)}. hizmet`}`}
                    onClick={() => {
                      setLines((list) => list.filter((_, i) => i !== index));
                    }}
                  >
                    Kaldır
                  </Button>
                </li>
              );
            })}
          </ul>
          {errors.items ? (
            <p className={styles.error} role="alert">
              {errors.items}
            </p>
          ) : null}
          {lines.length < maxItems ? (
            <Button
              size="sm"
              variant="ghost"
              icon={<Plus size={16} aria-hidden="true" />}
              onClick={() => {
                setLines((list) => [...list, { serviceId: '', sessions: '' }]);
              }}
            >
              Hizmet ekle
            </Button>
          ) : null}
        </section>
        <div className={styles.row}>
          <Select
            label="Geçerlilik"
            hint="Satış tarihinden itibaren"
            value={String(validityDays)}
            options={options.map((o) => ({ value: String(o.days), label: o.label }))}
            onChange={(event) => {
              setValidityDays(Number(event.target.value));
            }}
          />
          <TextField
            label="Satış primi"
            inputMode="decimal"
            placeholder="10"
            suffix="%"
            value={commission}
            error={errors.commission}
            hint="Boş bırakırsanız satan personelin oranı kullanılır."
            onChange={(event) => {
              setCommission(event.target.value);
            }}
          />
        </div>
        <Switch
          label="Bütün şubelerde kullanılabilir"
          {...(allBranches ? {} : { description: 'Paket yalnız satıldığı şubede kullanılır.' })}
          checked={allBranches}
          onChange={(event) => {
            setAllBranches(event.target.checked);
          }}
        />
      </form>
    </Modal>
  );
}
