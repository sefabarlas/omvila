import { describe, expect, it } from 'vitest';
import { linkAppointmentFixture, linkDepositFixture } from '@/test/appointment-fixtures';
import {
  awaitingTransfer,
  carriesDeposit,
  depositHintTotal,
  minus,
  minutesLeft,
  receivedDeposit,
} from './deposit';

const tl = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' as const });

describe('kapora yardımcıları', () => {
  it('kalan süre dakikaya yukarı yuvarlanır, dolunca 0', () => {
    const due = '2026-11-24T08:58:00Z';
    expect(minutesLeft(due, new Date('2026-11-24T07:00:00Z'))).toBe(118);
    expect(minutesLeft(due, new Date('2026-11-24T08:57:30Z'))).toBe(1);
    expect(minutesLeft(due, new Date('2026-11-24T08:58:00Z'))).toBe(0);
    expect(minutesLeft(due, new Date('2026-11-24T09:30:00Z'))).toBe(0);
  });

  it('ipucu yalnız hasDeposit olan kalemlerden toplanır', () => {
    expect(
      depositHintTotal([
        { hasDeposit: true, depositHint: tl(150) },
        { hasDeposit: false, depositHint: tl(500) },
        { hasDeposit: true, depositHint: tl(300) },
      ]),
    ).toEqual(tl(450));
    expect(depositHintTotal([{ hasDeposit: true, depositHint: null }])).toBeNull();
    expect(depositHintTotal([])).toBeNull();
  });

  it('salonda ödenecek eksiye inmez', () => {
    expect(minus(tl(750), tl(150))).toEqual(tl(600));
    expect(minus(tl(100), tl(150))).toEqual(tl(0));
  });

  it('durumlara göre: Havale, alınmış kapora, saat değişikliğinde aktarım', () => {
    expect(awaitingTransfer(linkDepositFixture('Awaiting'))?.instructions.transferCode).toBe(
      'OMV-7K2X',
    );
    expect(awaitingTransfer(linkDepositFixture('CustomerReported'))).not.toBeNull();
    expect(awaitingTransfer(linkDepositFixture('Received'))).toBeNull();
    expect(awaitingTransfer(linkAppointmentFixture())).toBeNull();
    expect(receivedDeposit(linkDepositFixture('Received'))?.required).toEqual(tl(150));
    expect(receivedDeposit(linkDepositFixture('Awaiting'))).toBeNull();
    expect(carriesDeposit(linkDepositFixture('Received'))).toBe(true);
    expect(carriesDeposit(linkDepositFixture('Waived'))).toBe(false);
    expect(carriesDeposit(linkAppointmentFixture())).toBe(false);
  });
});
