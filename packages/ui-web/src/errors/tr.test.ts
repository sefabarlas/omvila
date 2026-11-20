/// <reference types="node" />
import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { describe, expect, it } from 'vitest';
import { errorMessage, errorMessages, unknownErrorMessage } from './index';

// Kural: packages/ui-web/AGENTS.md — sözleşmedeki her ErrorCode için mesaj; metin kuralları (AGENTS.md § 5).
// Testler paket klasöründen çalışır (pnpm --filter, turbo).
const contract = readFileSync(resolve(process.cwd(), '../../contracts/openapi.yaml'), 'utf8');
const enumBlock = /ErrorCode:\n(?:.*\n)*?\s+enum:\n((?:\s+- .+\n)+)/.exec(contract)?.[1] ?? '';
const contractCodes = [...enumBlock.matchAll(/- (\w+)/g)].map((m) => m[1]);

describe('errorMessages', () => {
  it('sözleşmedeki kodların hepsi ve yalnız onlar', () => {
    expect(contractCodes.length).toBeGreaterThan(20);
    expect(Object.keys(errorMessages).sort()).toEqual([...contractCodes].sort());
  });

  it.each(Object.entries(errorMessages))('%s: ünlem, "lütfen", "başarıyla" yok', (_, message) => {
    expect(message).not.toMatch(/!|lütfen|başarıyla|kolayca|sadece/i);
  });

  it('bilinmeyen kod genel mesaj', () => {
    expect(errorMessage('yeni_kod')).toBe(unknownErrorMessage);
    expect(errorMessage(undefined)).toBe(unknownErrorMessage);
    expect(errorMessage('slot_taken')).toBe(errorMessages.slot_taken);
  });
});
