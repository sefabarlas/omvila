import { Button, Modal, TextArea } from '@omvila/ui-web/panel';
import { useState } from 'react';
import type { NotificationTemplateSetting } from './notification-api';
import { templateName, templateProblem } from './notification-text';
import styles from './NotificationSettings.module.css';

export interface TemplateEditModalProps {
  template: NotificationTemplateSetting;
  /** Formdaki güncel salon metni (kaydedilmemiş olabilir); null = sistem şablonu. */
  customBody: string | null;
  onClose: () => void;
  /** Metni forma yazar; kalıcı olması için üst bardaki "Değişiklikleri kaydet" gerekir. */
  onApply: (customBody: string | null) => void;
}

/** "Düzenle": salon metni, kullanılabilir değişkenler, sistem şablonuna dönüş. */
export function TemplateEditModal({
  template,
  customBody,
  onClose,
  onApply,
}: TemplateEditModalProps) {
  const [body, setBody] = useState(customBody ?? template.systemBody);
  const [error, setError] = useState<string | undefined>();
  const variables = template.variables.map((name) => `{${name}}`).join(', ');

  const apply = () => {
    const problem = templateProblem(body, template);
    if (problem !== null) {
      setError(problem);
      return;
    }

    onApply(body.trim() === template.systemBody ? null : body.trim());
  };

  return (
    <Modal
      open
      onClose={onClose}
      title={templateName(template)}
      description="Değişiklik, sayfada “Değişiklikleri kaydet” ile kesinleşir."
      footer={
        <>
          {customBody !== null ? (
            <Button
              variant="ghost"
              onClick={() => {
                onApply(null);
              }}
            >
              Sistem şablonuna dön
            </Button>
          ) : null}
          <Button onClick={onClose}>Vazgeç</Button>
          <Button variant="primary" onClick={apply}>
            Metni kullan
          </Button>
        </>
      }
    >
      <div className={styles.templateEditor}>
        <TextArea
          label="Salon metni"
          rows={4}
          maxLength={300}
          value={body}
          error={error}
          hint={`Değişkenler: ${variables}. En uzun değerlerle ${String(template.maxLength)} karakter = 1 kontör.`}
          onChange={(event) => {
            setBody(event.target.value);
            setError(undefined);
          }}
        />
        <div className={styles.systemBody}>
          <span className={styles.systemBodyLabel}>Sistem şablonu</span>
          <p className={styles.systemBodyText}>{template.systemBody}</p>
        </div>
      </div>
    </Modal>
  );
}
