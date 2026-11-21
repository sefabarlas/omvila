import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatCount } from '@omvila/ui-web/format';
import { Banner, BentoBlock, Button, Checkbox, Modal, useToast } from '@omvila/ui-web/panel';
import { FileText, Upload } from 'lucide-react';
import { useRef, useState } from 'react';
import type { CustomerImportPreview } from './api';
import styles from './Customers.module.css';
import {
  errorRowsCsv,
  errorRowsFileName,
  fileErrorText,
  invalidRowsSummary,
  mappedColumnsText,
} from './import-view';
import { downloadFile, useCommitImport, usePreviewImport } from './records-api';

/**
 * Müşterileri içe aktar (salon-paneli/MusteriIceAktar): CSV seç → önizleme (eklenecek, birleşecek, hatalı satır) →
 * "N kaydı içe aktar". Hatalı satırlar istemcide CSV olarak indirilir. customer.import (yalnız sahip).
 */
export function ImportCustomersModal({ onClose }: { onClose: () => void }) {
  const toast = useToast();
  const input = useRef<HTMLInputElement>(null);
  const preview = usePreviewImport();
  const commit = useCommitImport();
  const [result, setResult] = useState<CustomerImportPreview | null>(null);
  const [sendInfoSms, setSendInfoSms] = useState(false);
  const importable = result ? result.toCreate + result.toMerge : 0;
  const previewError =
    preview.error instanceof ApiProblem
      ? (fileErrorText(preview.error.errors.file) ?? errorMessage(preview.error.code))
      : preview.isError
        ? errorMessage(undefined)
        : null;
  const commitError =
    commit.error instanceof ApiProblem && commit.error.code === 'hold_expired'
      ? 'Önizlemenin süresi doldu. Dosyayı yeniden seçin.'
      : commit.isError
        ? errorMessage(commit.error instanceof ApiProblem ? commit.error.code : undefined)
        : null;

  const choose = (file: File | undefined) => {
    if (!file) {
      return;
    }

    commit.reset();
    setResult(null);
    preview.mutate(
      { file, key: idempotencyKey()['Idempotency-Key'] },
      {
        onSuccess: (data) => {
          setResult(data);
        },
      },
    );
  };

  const submit = () => {
    if (!result) {
      return;
    }

    commit.mutate(
      { importId: result.importId, sendInfoSms },
      {
        onSuccess: (done) => {
          toast.show({
            tone: 'basari',
            title: `${formatCount(done.created + done.merged)} kayıt içe aktarıldı`,
            message: `${formatCount(done.created)} yeni, ${formatCount(done.merged)} birleşti, ${formatCount(done.skipped)} satır atlandı.`,
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
      title="Müşterileri içe aktar"
      onClose={onClose}
      footer={
        <div className={styles.importFoot}>
          {result && result.errors.length > 0 ? (
            <Button
              variant="ghost"
              icon={<FileText size={16} aria-hidden="true" />}
              onClick={() => {
                downloadFile(
                  new Blob([errorRowsCsv(result.errors)], { type: 'text/csv;charset=utf-8' }),
                  errorRowsFileName(result.fileName),
                );
              }}
            >
              Hatalı satırları indir
            </Button>
          ) : (
            <span />
          )}
          <span className={styles.importActions}>
            <Button onClick={onClose}>Vazgeç</Button>
            <Button
              variant="primary"
              disabled={importable === 0}
              loading={commit.isPending}
              loadingLabel="İçe aktarılıyor…"
              onClick={submit}
            >
              {importable > 0 ? `${formatCount(importable)} kaydı içe aktar` : 'İçe aktar'}
            </Button>
          </span>
        </div>
      }
    >
      <div className={styles.dialogBody}>
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
        <div className={styles.fileCard}>
          <FileText size={20} className={styles.tipIcon} aria-hidden="true" />
          <div className={styles.fileText}>
            {result ? (
              <>
                <strong>{result.fileName}</strong>
                <span className={styles.muted}>
                  {`${formatCount(result.totalRows)} satır · ${mappedColumnsText(result.mappedColumns)}`}
                </span>
              </>
            ) : (
              <>
                <strong>{preview.isPending ? 'Dosya okunuyor…' : 'CSV dosyası seçin'}</strong>
                <span className={styles.muted}>
                  İlk satır başlık olmalı: Ad, Telefon, Not, Doğum yılı, E-posta. Excel listenizi
                  CSV olarak kaydedebilirsiniz.
                </span>
              </>
            )}
          </div>
          <Button
            variant={result ? 'ghost' : 'secondary'}
            icon={result ? undefined : <Upload size={16} aria-hidden="true" />}
            loading={preview.isPending}
            loadingLabel="Okunuyor…"
            onClick={() => input.current?.click()}
          >
            {result ? 'Değiştir' : 'Dosya seç'}
          </Button>
        </div>
        {previewError ? (
          <Banner tone="hata" title="Dosya içe aktarılamıyor">
            {previewError}
          </Banner>
        ) : null}
        {commitError ? (
          <Banner tone="hata" title="İçe aktarılamadı">
            {commitError}
          </Banner>
        ) : null}
        {result ? (
          <>
            <div className={styles.importBlocks} role="group" aria-label="İçe aktarma özeti">
              <BentoBlock
                tone="marka"
                label="Eklenecek"
                value={formatCount(result.toCreate)}
                sub="yeni müşteri"
              />
              <BentoBlock
                tone="kum"
                label="Birleşecek"
                value={formatCount(result.toMerge)}
                sub="numarası zaten kayıtlı"
              />
              <BentoBlock
                tone="vurgu"
                label="Hatalı satır"
                value={formatCount(result.invalidRows)}
                {...(result.invalidRows > 0 ? { sub: invalidRowsSummary(result.errors) } : {})}
              />
            </div>
            <p className={styles.formNote}>
              Telefonlar tek biçime çevrilir: 0532 123 45 67, 5321234567 ve +90 532 123 4567 aynı
              kişi sayılır. Birleşenlerde mevcut notlar korunur.
            </p>
            <Checkbox
              label="İçe aktarılanlara randevu bilgilendirme SMS’i gönderilebilir"
              description="Kampanya izni içe aktarılmaz; müşteriden ayrıca alınır."
              checked={sendInfoSms}
              onChange={(event) => {
                setSendInfoSms(event.target.checked);
              }}
            />
          </>
        ) : null}
      </div>
    </Modal>
  );
}
