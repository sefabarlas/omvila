import { describe, expect, it } from 'vitest';
import { packageOptions, sessionsLeftLabel } from '@/booking/packages';
import { ids } from '@/test/booking-fixtures';
import { packageFixture } from '@/test/d5-fixtures';
import { appleAppSiteAssociation, assetLinks, wellKnownResponse } from './app-links';
import { withLocative, withPossessive } from './suffix';

describe('Türkçe ekler', () => {
  it('salon adına bulunma eki', () => {
    expect(withLocative('Kuaför Elif')).toBe("Kuaför Elif'te");
    expect(withLocative('Spa Moda')).toBe("Spa Moda'da");
    expect(withLocative('Berber Ozan')).toBe("Berber Ozan'da");
    expect(withLocative('Salon 34')).toBeNull();
    expect(withLocative('XYZ')).toBeNull();
  });

  it('sayıya iyelik eki ("10 seansın 7\'si")', () => {
    expect(withPossessive(7)).toBe("7'si");
    expect(withPossessive(3)).toBe("3'ü");
    expect(withPossessive(6)).toBe("6'sı");
    expect(withPossessive(10)).toBe("10'u");
    expect(withPossessive(1)).toBe("1'i");
    expect(withPossessive(40)).toBe("40'ı");
    expect(withPossessive(100)).toBe("100'ü");
  });
});

describe('paket kutusu', () => {
  const services = [
    { id: ids.sac, name: 'Saç kesimi' },
    { id: ids.fon, name: 'Fön' },
  ];

  it('yalnız seçilen hizmetin seansı kalan paketi; süresi en yakın biten önce', () => {
    const later = packageFixture({ customerPackageItemId: 'b', expiresOn: '2027-06-01' });
    const sooner = packageFixture({ customerPackageItemId: 'a', expiresOn: '2027-03-30' });
    const empty = packageFixture({ customerPackageItemId: 'c', remainingSessions: 0 });
    const other = packageFixture({ customerPackageItemId: 'd', serviceId: ids.bakim });
    const options = packageOptions([later, empty, other, sooner], services);
    expect(options).toHaveLength(1);
    expect(options[0]?.serviceName).toBe('Fön');
    expect(options[0]?.item.customerPackageItemId).toBe('a');
  });

  it('kalan seans metni', () => {
    expect(sessionsLeftLabel(packageFixture())).toBe("10 seansın 7'si kaldı");
  });
});

describe('derin bağlantı doğrulama dosyaları', () => {
  it('Apple Team ID yoksa veya biçimi bozuksa dosya yok (404)', async () => {
    expect(appleAppSiteAssociation({})).toBeNull();
    expect(appleAppSiteAssociation({ APPLE_TEAM_ID: 'kisa' })).toBeNull();
    const response = wellKnownResponse(null);
    expect(response.status).toBe(404);
    expect(await response.text()).toBe('');
  });

  it('apple-app-site-association: com.omvila.app, /r/ /b/ /d/ ve tek bölümlü salon adresi', async () => {
    const body = appleAppSiteAssociation({ APPLE_TEAM_ID: 'AB12CD34EF' });
    const [detail] = body?.applinks.details ?? [];
    expect(detail?.appIDs).toEqual(['AB12CD34EF.com.omvila.app']);
    expect(detail?.components).toEqual([
      { '/': '/r/*' },
      { '/': '/b/*' },
      { '/': '/d/*' },
      { '/': '/', exclude: true },
      { '/': '/*/*', exclude: true },
      { '/': '/*' },
    ]);
    const response = wellKnownResponse(body);
    expect(response.headers.get('Content-Type')).toContain('application/json');
    expect(await response.json()).toEqual(body);
  });

  it('assetlinks.json: geçerli parmak izleri (virgülle birden çok), geçersizler atılır', () => {
    const print = Array.from({ length: 32 }, () => 'ab').join(':');
    expect(assetLinks({})).toBeNull();
    expect(assetLinks({ ANDROID_CERT_SHA256: 'bozuk' })).toBeNull();
    expect(assetLinks({ ANDROID_CERT_SHA256: ` ${print} , bozuk` })).toEqual([
      {
        relation: ['delegate_permission/common.handle_all_urls'],
        target: {
          namespace: 'android_app',
          package_name: 'com.omvila.app',
          sha256_cert_fingerprints: [print.toUpperCase()],
        },
      },
    ]);
  });
});
