// Dart üreticisi (openapi-generator dart-dio) için sözleşmenin geçici kopyasını hazırlar.
// Kaynak sözleşme değişmez; yalnız dist/openapi.dart.json üretilir.
//
// Neden (üretici 7.27.0, json_serializable kipi, derlenmeyen kod üreten iki hata):
// 1. `type: boolean` + `const: true` (acceptedTerms, privacyNoticeAcknowledged) bozuk
//    bir enum olur; SIMPLIFY_BOOLEAN_ENUM normalleştiricisi `const`'u kapsamıyor.
//    Alan istemcide düz bool olur; `true` olma kuralını sunucu doğrular.
// 2. Varsayılanı olan satır içi enum (`scope: ThisOnly`) var olmayan bir const
//    kurucuya başvurur. Varsayılan kaldırılır; alan gönderilmezse sunucu uygular.
//
// Ayrıca istek gövdelerinden ulaşılan şemalarda isteğe bağlı ve null alabilen alanlar
// `x-send-null: true` ile işaretlenir; class.mustache bunlara `includeIfNull: true` yazar
// (null "boşalt" demektir, ör. MeProfileUpdate.email). Null alamayan isteğe bağlı alan
// gönderilmemeye devam eder (PATCH anlamı). T-113.
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

const schemas = spec.components?.schemas ?? {};
const isNullable = (prop) =>
  prop.nullable === true ||
  (Array.isArray(prop.type) && prop.type.includes('null')) ||
  [...(prop.oneOf ?? []), ...(prop.anyOf ?? [])].some((option) => option.type === 'null');
const visited = new Set();
let sendNull = 0;
const markRequest = (schema) => {
  if (schema === null || typeof schema !== 'object' || visited.has(schema)) return;
  visited.add(schema);
  if (typeof schema.$ref === 'string') {
    markRequest(schemas[schema.$ref.split('/').pop()]);
    return;
  }
  const required = new Set(schema.required ?? []);
  for (const [name, prop] of Object.entries(schema.properties ?? {})) {
    if (!required.has(name) && isNullable(prop) && !prop['x-send-null']) {
      prop['x-send-null'] = true;
      sendNull += 1;
    }
    markRequest(prop);
  }
  for (const key of ['items', 'additionalProperties']) markRequest(schema[key]);
  for (const key of ['allOf', 'oneOf', 'anyOf']) (schema[key] ?? []).forEach(markRequest);
};
for (const item of Object.values(spec.paths ?? {})) {
  for (const operation of Object.values(item)) {
    const content = operation?.requestBody?.content ?? {};
    Object.values(content).forEach((media) => markRequest(media.schema));
  }
}

writeFileSync(output, `${JSON.stringify(spec, null, 2)}\n`);
console.log(
  `${output}: ${boolConst} boolean const, ${enumDefault} enum varsayılanı kaldırıldı, ${sendNull} alan null gönderir`,
);
