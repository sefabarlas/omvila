import { ApiProblem } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDate, formatTime } from '@omvila/ui-web/format';
import { Button, Modal, Select, Switch, TextField, useToast } from '@omvila/ui-web/panel';
import { toBranchDate } from '@omvila/ui-web/time';
import { useState } from 'react';
import { useSession } from '@/app/session';
import { useCancelAppointment } from './api';
import type { AppointmentDetail, CancelRequest } from './api';
import styles from './Calendar.module.css';

const reasons: { value: CancelRequest['reason']; label: string }[] = [
  { value: 'CustomerRequested', label: 'Müşteri istedi' },
  { value: 'StaffUnavailable', label: 'Personel müsait değil' },
  { value: 'Other', label: 'Diğer' },
];

type Scope = NonNullable<CancelRequest['scope']>;

interface CancelAppointmentModalProps {
  detail: AppointmentDetail;
  onClose: () => void;
  onCancelled: () => void;
  /** 412: yan panel "Kayıt çakışması" şeridini gösterir. */
  onConflict: () => void;
}

/**
 * Randevuyu iptal et (salon iptali, A7). Tekrarlayanda kapsam seçilir (salon-paneli/TekrarIptal): yalnızca bu
 * randevu ya da bu ve sonrakiler (geçmiş randevular etkilenmez). "Müşteriye bildir" notifyCustomer'dır.
 */
export function CancelAppointmentModal({
  detail,
  onClose,
  onCancelled,
  onConflict,
}: CancelAppointmentModalProps) {
  const toast = useToast();
  const cancel = useCancelAppointment();
  const [scope, setScope] = useState<Scope>('ThisOnly');
  const [reason, setReason] = useState<CancelRequest['reason']>('CustomerRequested');
  const [note, setNote] = useState('');
  const [notify, setNotify] = useState(true);
  // Boşalan saat bekleme listesine teklif edilsin (varsayılan açık, A7). Anahtar listeyi yönetene görünür; diğerlerinde
  // alan gönderilmez, sunucu varsayılanı (true) geçerlidir (T-98).
  const { can } = useSession();
  const managesWaitlist = can('waitlist.manage');
  const [offerToWaitlist, setOfferToWaitlist] = useState(true);
  const tz = detail.timeZone;
  const date = toBranchDate(new Date(detail.startUtc), tz);
  const time = formatTime(detail.startUtc, tz);
  const staff = detail.items.find((item) => item.staffName)?.staffName;
  const summary = [
    detail.customer.fullName,
    detail.items.map((item) => item.serviceName).join(' + '),
    `${formatDate(date, 'panel').split(',')[0] ?? ''} ${time}`,
    staff,
  ]
    .filter(Boolean)
    .join(' · ');

  const submit = () => {
    const body: CancelRequest = {
      reason,
      notifyCustomer: notify,
      ...(note.trim() !== '' ? { note: note.trim() } : {}),
      ...(detail.isRecurring ? { scope } : {}),
      ...(managesWaitlist ? { offerToWaitlist } : {}),
    };
    cancel.mutate(
      { detail, input: body },
      {
        onSuccess: () => {
          toast.show({ tone: 'basari', title: 'Randevu iptal edildi' });
          onCancelled();
        },
        onError: (error) => {
          if (error instanceof ApiProblem && error.status === 412) {
            onConflict();
            return;
          }

          toast.show({
            tone: 'hata',
            title: errorMessage(error instanceof ApiProblem ? error.code : undefined),
          });
        },
      },
    );
  };

  return (
    <Modal
      open
      onClose={onClose}
      title={detail.isRecurring ? 'Tekrarlayan randevuyu iptal et' : 'Randevuyu iptal et'}
      description={summary}
      footer={
        <>
          <Button onClick={onClose}>Vazgeç</Button>
          <Button
            variant="danger"
            loading={cancel.isPending}
            loadingLabel="İptal ediliyor…"
            onClick={submit}
          >
            Randevuyu iptal et
          </Button>
        </>
      }
    >
      <div className={styles.cancelForm}>
        {detail.isRecurring ? (
          <fieldset className={styles.scopes}>
            <legend className={styles.srOnly}>Hangi randevular</legend>
            {(
              [
                ['ThisOnly', 'Yalnızca bu randevu', `${formatDate(date, 'dayMonth')} ${time}`],
                [
                  'ThisAndFollowing',
                  'Bu ve sonraki randevular',
                  'Bu tarihten itibaren · geçmiş randevular etkilenmez',
                ],
              ] as const
            ).map(([value, label, hint]) => (
              <label key={value} className={styles.scope}>
                <input
                  type="radio"
                  name="kapsam"
                  checked={scope === value}
                  onChange={() => {
                    setScope(value);
                  }}
                />
                <span>
                  {label}
                  <small>{hint}</small>
                </span>
              </label>
            ))}
          </fieldset>
        ) : null}
        <Select
          label="İptal nedeni"
          value={reason}
          options={reasons}
          onChange={(event) => {
            setReason(event.target.value as CancelRequest['reason']);
          }}
        />
        {reason === 'Other' ? (
          <TextField
            label="Not"
            maxLength={300}
            value={note}
            onChange={(event) => {
              setNote(event.target.value);
            }}
          />
        ) : null}
        <Switch
          label="Müşteriye bildir"
          description="İptal mesajı müşteriye SMS ile gönderilir."
          checked={notify}
          onChange={(event) => {
            setNotify(event.target.checked);
          }}
        />
        {managesWaitlist ? (
          <Switch
            label="Boşalan saat bekleme listesine teklif edilsin"
            description="Uygun bekleyen varsa sıradaki kişiye teklif gider; saat bir süre onun için tutulur."
            checked={offerToWaitlist}
            onChange={(event) => {
              setOfferToWaitlist(event.target.checked);
            }}
          />
        ) : null}
      </div>
    </Modal>
  );
}
