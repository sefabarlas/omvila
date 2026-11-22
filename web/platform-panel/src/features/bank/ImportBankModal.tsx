import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatCount } from '@omvila/ui-web/format';
import { Banner, BentoBlock, Button, DataTable, Modal, useToast } from '@omvila/ui-web/panel';
import { FileText, Upload } from 'lucide-react';
import { useRef, useState } from 'react';
import styles from '@/features/approvals/Approvals.module.css';
import { useImportBankTransactions } from './api';
import type { ImportResult } from './api';

const rowErrorLabels: Record<string, string> = {
  required: 'Boş',
  invalid_format: 'Biçim hatalı',
  out_of_range: 'Aralık dışında',
};

/** Dosya alanı hataları (errors.file): CSV değil veya boyut/satır sınırı aşıldı. */
function fileErrorText(problem: unknown): string | null {
  if (!(problem instanceof ApiProblem)) {
    return problem ? errorMessage(undefined) : null;
  }

  const codes = problem.errors.file ?? [];
  if (codes.includes('too_large')) {
    return 'Dosya çok büyük. Dökümü daha kısa aralıklara bölüp ayrı ayrı yükleyin.';
  }

  if (codes.includes('invalid_format')) {
    return 'Dosya okunamadı. UTF-8 CSV seçin; ilk satır başlık olmalı.';
  }

  return errorMessage(problem.code);
}

/**
 * Banka dökümünü CSV'den içe aktar: dosya seçilince önizleme (dryRun; kayıt yazılmaz), sonra "N hareketi içe aktar"
 * aynı dosyayı uygular. Referansı kayıtlı satırlar atlanır; hatalı satırlar listede. platform.billing.manage.
 */
export function ImportBankModal({ onClose }: { onClose: () => void }) {
  const toast = useToast();
  const input = useRef<HTMLInputElement>(null);
  const preview = useImportBankTransactions();
  const apply = useImportBankTransactions();
  const [file, setFile] = useState<File | null>(null);
  const [result, setResult] = useState<ImportResult | null>(null);
  const previewError = fileErrorText(preview.error);
  const applyError = fileErrorText(apply.error);

  const choose = (next: File | undefined) => {
    if (!next) {
      return;
    }

    apply.reset();
    setResult(null);
    setFile(next);
    preview.mutate(
      { file: next, dryRun: true },
      {
        onSuccess: (data) => {
          setResult(data);
        },
      },
    );
  };

  const submit = () => {
    if (!file) {
      return;
    }

    apply.mutate(
      { file, dryRun: false },
      {
        onSuccess: (done) => {
          toast.show({
            tone: 'basari',
            title: `${formatCount(done.imported)} hareket içe aktarıldı`,
            message: `${formatCount(done.duplicates)} zaten kayıtlı, ${formatCount(done.invalidRows)} hatalı satır atlandı.`,
          });
          onClose();
        },
      },
    );
  };

  return (
    <Modal
      open
      size="lg"
      title="Banka dökümünü içe aktar"
      onClose={onClose}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="primary"
            disabled={!result || result.imported === 0}
            loading={apply.isPending}
            loadingLabel="İçe aktarılıyor…"
            onClick={submit}
          >
            {result && result.imported > 0
              ? `${formatCount(result.imported)} hareketi içe aktar`
              : 'İçe aktar'}
          </Button>
        </>
      }
    >
      <div className={styles.form}>
        <input
          ref={input}
          type="file"
          accept=".csv,text/csv"
          className={styles.visuallyHidden}
          aria-label="CSV dosyası"
          tabIndex={-1}
          onChange={(event) => {
            choose(event.target.files?.[0]);
            event.target.value = '';
          }}
        />
        <div className={styles.file}>
          <span className={styles.fileIcon} aria-hidden="true">
            <FileText size={18} />
          </span>
          <div className={styles.candidateText}>
            {result ? (
              <>
                <strong>{result.fileName}</strong>
                <span>{`${formatCount(result.totalRows)} satır · önizleme, henüz kayıt yazılmadı`}</span>
              </>
            ) : (
              <>
                <strong>{preview.isPending ? 'Dosya okunuyor…' : 'CSV dosyası seçin'}</strong>
                <span>
                  İlk satır başlık olmalı: Tarih, Tutar, Gönderen, Açıklama, Referans. Yalnız gelen
                  havaleler alınır.
                </span>
              </>
            )}
          </div>
          <Button
            size="sm"
            icon={result ? undefined : <Upload size={14} aria-hidden="true" />}
            loading={preview.isPending}
            loadingLabel="Okunuyor…"
            onClick={() => input.current?.click()}
          >
            {result ? 'Değiştir' : 'Dosya seç'}
          </Button>
        </div>
        {previewError ? (
          <Banner tone="hata" title="Dosya içe aktarılamıyor.">
            {previewError}
          </Banner>
        ) : null}
        {applyError ? (
          <Banner tone="hata" title="İçe aktarılamadı.">
            {applyError}
          </Banner>
        ) : null}
        {result ? (
          <>
            <div className={styles.summary} role="group" aria-label="İçe aktarma özeti">
              <BentoBlock
                tone="marka"
                label="Eklenecek"
                value={formatCount(result.imported)}
                sub="yeni hareket"
              />
              <BentoBlock
                tone="kum"
                label="Zaten kayıtlı"
                value={formatCount(result.duplicates)}
                sub="referansı var, atlanır"
              />
              <BentoBlock
                label="Giden"
                value={formatCount(result.skippedOutgoing)}
                sub="eksi tutar, atlanır"
              />
              <BentoBlock
                tone="vurgu"
                label="Hatalı satır"
                value={formatCount(result.invalidRows)}
                sub="atlanır"
              />
            </div>
            {result.errors.length > 0 ? (
              <DataTable
                caption="Hatalı satırlar"
                rows={result.errors}
                rowKey={(row) => `${String(row.rowNumber)}-${row.column}`}
                columns={[
                  {
                    key: 'row',
                    label: 'Satır',
                    numeric: true,
                    render: (row) => formatCount(row.rowNumber),
                  },
                  { key: 'column', label: 'Sütun', render: (row) => row.column },
                  {
                    key: 'code',
                    label: 'Hata',
                    render: (row) => rowErrorLabels[row.code] ?? row.code,
                  },
                ]}
              />
            ) : null}
          </>
        ) : null}
      </div>
    </Modal>
  );
}
