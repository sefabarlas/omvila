import { describe, expect, it } from 'vitest';
import {
  normalizeSenderName,
  reminderLabel,
  senderNamePattern,
  templateName,
  templateProblem,
} from './notification-text';

// Kural: sözleşme NotificationSettings (hatırlatma dakikaları), manageUpdateNotificationSettings (şablon doğrulaması),
// manageRequestSmsSender (gönderen adı biçimi); salon-paneli/AyarlarBildirim satır adları.
describe('reminderLabel', () => {
  it('gün, saat, dakika', () => {
    expect(reminderLabel(1440)).toBe('1 gün önce');
    expect(reminderLabel(2880)).toBe('2 gün önce');
    expect(reminderLabel(120)).toBe('2 saat önce');
    expect(reminderLabel(90)).toBe('90 dk önce');
    expect(reminderLabel(30)).toBe('30 dk önce');
  });
});

describe('templateName', () => {
  it('olay kodu ve varyanttan', () => {
    expect(templateName({ eventCode: 'appointment.confirmed', variant: null })).toBe(
      'Randevu onaylandı',
    );
    expect(templateName({ eventCode: 'appointment.reminder', variant: 'same_day' })).toBe(
      'Hatırlatma (aynı gün)',
    );
    expect(templateName({ eventCode: 'appointment.declined', variant: null })).not.toMatch(
      /reddedil/i,
    );
  });
});

describe('templateProblem', () => {
  const template = { variables: ['salon', 'tarih', 'bağlantı'], requiredVariables: ['bağlantı'] };

  it('geçerli metin', () => {
    expect(templateProblem('{salon}: {tarih} randevunuz var. {bağlantı}', template)).toBeNull();
  });

  it('zorunlu değişken eksik', () => {
    expect(templateProblem('{salon}: randevunuz var.', template)).toBe(
      '{bağlantı} değişkeni metinde olmalı.',
    );
  });

  it('listede olmayan değişken', () => {
    expect(templateProblem('{ad} {bağlantı}', template)).toBe(
      '{ad} kullanılamaz. Listedeki değişkenleri kullanın.',
    );
  });

  it('boş ve 300 karakteri aşan', () => {
    expect(templateProblem('  ', template)).not.toBeNull();
    expect(templateProblem(`${'a'.repeat(300)}{bağlantı}`, template)).toBe(
      'Metin en fazla 300 karakter olabilir.',
    );
  });
});

describe('normalizeSenderName', () => {
  it('büyük Latin harfe çevirir, boşluk ve noktalamayı atar', () => {
    expect(normalizeSenderName('Kuaför Elif')).toBe('KUAFORELIF');
    expect(normalizeSenderName('çiğdem-şişli')).toBe('CIGDEMSISLI');
    expect(normalizeSenderName('ıhlamur ü.')).toBe('IHLAMURU');
  });

  it('desen: 3–11 karakter, en az bir harf', () => {
    expect(senderNamePattern.test('KUAFORELIF')).toBe(true);
    expect(senderNamePattern.test('AB')).toBe(false);
    expect(senderNamePattern.test('123456')).toBe(false);
    expect(senderNamePattern.test('KUAFORELIFKD')).toBe(false);
  });
});
