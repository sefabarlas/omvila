// Dart üreticisi (openapi-generator dart-dio) için sözleşmenin geçici kopyasını hazırlar.
// Kaynak sözleşme değişmez; yalnız dist/openapi.dart.json üretilir.
//
// Neden (üretici 7.27.0, json_serializable kipi, derlenmeyen kod üreten iki hata):
// 1. `type: boolean` + `const: true` (acceptedTerms, privacyNoticeAcknowledged) bozuk
//    bir enum olur; SIMPLIFY_BOOLEAN_ENUM normalleştiricisi `const`'u kapsamıyor.
//    Alan istemcide düz bool olur; `true` olma kuralını sunucu doğrular.
// 2. Varsayılanı olan satır içi enum (`scope: ThisOnly`) var olmayan bir const
//    kurucuya başvurur. Varsayılan kaldırılır; alan gönderilmezse sunucu uygular.
import { readFileSync, writeFileSync } from 'node:fs';

const [input = 'dist/openapi.json', output = 'dist/openapi.dart.json'] = process.argv.slice(2);
const spec = JSON.parse(readFileSync(input, 'utf8'));

let boolConst = 0;
let enumDefault = 0;
const walk = (node) => {
  if (Array.isArray(node)) {
    node.forEach(walk);
    return;
  }
  if (node === null || typeof node !== 'object') return;
  if (node.type === 'boolean' && 'const' in node) {
    delete node.const;
    boolConst += 1;
  }
  if (Array.isArray(node.enum) && 'default' in node) {
    delete node.default;
    enumDefault += 1;
  }
  Object.values(node).forEach(walk);
};
walk(spec);

writeFileSync(output, `${JSON.stringify(spec, null, 2)}\n`);
console.log(`${output}: ${boolConst} boolean const, ${enumDefault} enum varsayılanı kaldırıldı`);
