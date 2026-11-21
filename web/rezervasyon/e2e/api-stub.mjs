// Uçtan uca testlerin API önü: salon kartı (publicGetBrand) sunucuda okunduğu için page.route ile yanıtlanamaz.
// Bu küçük sunucu salon kartlarını test verisiyle verir, diğer her isteği olduğu gibi Prism mock'a iletir.
// Kurgusal salonlar: kuafor-elif (tek şube), kuafor-elif-subeler (iki şube, Sube.png), kuafor-elif-kapali
// (online randevu kapalı, Durumlar.png). Bağımlılık yok (node:http).
import http from 'node:http';

const port = Number(process.env.STUB_PORT ?? 4011);
const prism = new URL(process.env.PRISM_URL ?? 'http://localhost:4010');

const branch = (overrides) => ({
  id: '0190a000-0000-7000-8000-000000000002',
  name: 'Kadıköy',
  addressLine: 'Moda Cad. 42',
  district: 'Kadıköy',
  city: 'İstanbul',
  phone: '+905321234567',
  latitude: 40.987,
  longitude: 29.026,
  timeZone: 'Europe/Istanbul',
  openNow: true,
  todayClosesAt: '20:00',
  nextOpeningUtc: null,
  ...overrides,
});

const brand = (overrides) => ({
  id: '0190a000-0000-7000-8000-000000000001',
  name: 'Kuaför Elif',
  slug: 'kuafor-elif',
  shortDescription: 'Saç, bakım ve makyajda 12 yıllık deneyim.',
  logoUrl: null,
  industryLabel: 'Saç, bakım, makyaj',
  coverColor: 'petrol',
  showPrices: true,
  bookingEnabled: true,
  rating: null,
  branches: [branch({})],
  ...overrides,
});

const brands = {
  'kuafor-elif': brand({}),
  'kuafor-elif-subeler': brand({
    slug: 'kuafor-elif-subeler',
    branches: [
      branch({}),
      branch({
        id: '0190a000-0000-7000-8000-000000000003',
        name: 'Nişantaşı',
        addressLine: 'Abdi İpekçi Cad. 18',
        district: 'Şişli',
        phone: '+905321234568',
        openNow: false,
        todayClosesAt: null,
        // Açılış saati verilmez: "Yarın 10:00'da açılır" etiketi saate bağlıdır; şube adımı sunucuda gerçek saatle,
        // tarayıcıda testin sabit saatiyle çizilince hidrasyon uyuşmazlığı olur. Etiket "Şu an kapalı".
        nextOpeningUtc: null,
      }),
    ],
  }),
  'kuafor-elif-kapali': brand({ slug: 'kuafor-elif-kapali', bookingEnabled: false }),
};

const brandPath = /^\/api\/v1\/public\/brands\/([a-z0-9-]+)$/;

http
  .createServer((request, response) => {
    const url = new URL(request.url ?? '/', prism);
    const slug = request.method === 'GET' ? brandPath.exec(url.pathname)?.[1] : undefined;
    const stub = slug ? brands[slug] : undefined;
    if (stub) {
      response.writeHead(200, { 'Content-Type': 'application/json' });
      response.end(JSON.stringify(stub));
      return;
    }

    const upstream = http.request(
      url,
      { method: request.method, headers: { ...request.headers, host: prism.host } },
      (reply) => {
        response.writeHead(reply.statusCode ?? 502, reply.headers);
        reply.pipe(response);
      },
    );
    upstream.on('error', () => {
      response.writeHead(502);
      response.end();
    });
    request.pipe(upstream);
  })
  .listen(port, () => {
    console.log(`api-stub http://localhost:${String(port)} -> ${prism.origin}`);
  });
