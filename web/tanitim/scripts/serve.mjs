// Statik çıktıyı (out/) yerelde sunar: Lighthouse CI ve Playwright için. Üretimde Caddy sunar (Dockerfile).
// Bağımlılıksız; dizin isteğinde index.html, bilinmeyen yolda 404.html. Metin dosyaları Caddy'deki gibi gzip'li
// gider (Lighthouse aktarım boyutunu ölçer).
import { createReadStream } from 'node:fs';
import { stat } from 'node:fs/promises';
import { createServer } from 'node:http';
import { extname, join, normalize } from 'node:path';
import { fileURLToPath } from 'node:url';
import { createGzip } from 'node:zlib';

const root = fileURLToPath(new URL('../out', import.meta.url));
const port = Number(process.env.PORT ?? 3001);
const types = {
  '.html': 'text/html; charset=utf-8',
  '.js': 'text/javascript; charset=utf-8',
  '.css': 'text/css; charset=utf-8',
  '.svg': 'image/svg+xml',
  '.webp': 'image/webp',
  '.woff2': 'font/woff2',
  '.txt': 'text/plain; charset=utf-8',
  '.xml': 'application/xml',
  '.json': 'application/json',
  '.ico': 'image/x-icon',
};

async function resolve(urlPath) {
  const clean = normalize(decodeURIComponent(urlPath.split('?')[0] ?? '/')).replace(
    /^(\.\.[/\\])+/,
    '',
  );
  let file = join(root, clean);
  if (!file.startsWith(root)) return null;
  try {
    if ((await stat(file)).isDirectory()) file = join(file, 'index.html');
    await stat(file);
    return file;
  } catch {
    return null;
  }
}

createServer((req, res) => {
  void resolve(req.url ?? '/').then((file) => {
    const target = file ?? join(root, '404.html');
    const type = types[extname(target)] ?? 'application/octet-stream';
    const gzip =
      /^(text\/|application\/(json|xml)|image\/svg)/.test(type) &&
      /\bgzip\b/.test(req.headers['accept-encoding'] ?? '');
    res.writeHead(file ? 200 : 404, {
      'Content-Type': type,
      ...(gzip ? { 'Content-Encoding': 'gzip', Vary: 'Accept-Encoding' } : {}),
    });
    const stream = createReadStream(target);
    if (gzip) stream.pipe(createGzip()).pipe(res);
    else stream.pipe(res);
  });
}).listen(port, () => {
  console.log(`Ready on http://localhost:${String(port)}`);
});
