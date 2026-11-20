#!/usr/bin/env python3
"""OMVILA sözleşme kuralları denetimi.

Kullanım:  python3 contracts/scripts/check_conventions.py contracts/openapi.yaml
Bağımlılık: pyyaml, jsonschema>=4.18 (pip install pyyaml jsonschema)

Kontroller (docs/sozlesme/api-kurallari.md):
  - Tüm $ref'ler çözülüyor
  - operationId benzersiz ve yüzey önekiyle başlıyor
  - Her işlemde tek etiket, en az bir 2xx/3xx yanıt
  - Yol parametreleri ile tanımlı path parametreleri eşleşiyor
  - manage/me/admin uçları (auth hariç) güvenlik tanımlı
  - date-time alanları 'Utc' ile bitiyor
  - 4xx/5xx yanıtları components/responses'a başvuruyor
  - Kaynak oluşturan POST'larda Idempotency-Key, durum eylemlerinde If-Match
  - Kullanılmayan şema yok
  - Şemalar geçerli JSON Schema 2020-12, örnekler şemalarına uyuyor
  - ErrorCode enum'u docs/sozlesme/api-kurallari.md kataloğuyla aynı
"""
import json
import pathlib
import re
import sys

import yaml
from jsonschema import Draft202012Validator
from referencing import Registry, Resource
from referencing.jsonschema import DRAFT202012

spec_path = pathlib.Path(sys.argv[1] if len(sys.argv) > 1 else "contracts/openapi.yaml")
doc = yaml.safe_load(spec_path.read_text(encoding="utf-8"))
errors: list[str] = []
METHODS = {"get", "post", "put", "patch", "delete"}
SURFACES = {"/api/v1/manage": "manage", "/api/v1/public": "public", "/api/v1/me": "me",
            "/api/v1/admin": "admin", "/api/v1/webhooks": "webhook"}
STATE_ACTIONS = ("/approve", "/decline", "/cancel", "/reschedule", "/check-in", "/undo-no-show",
                 "/complete", "/mark-received", "/refund")


def resolve(ref: str):
    node = doc
    for part in ref[2:].split("/"):
        part = part.replace("~1", "/").replace("~0", "~")
        if not isinstance(node, dict) or part not in node:
            return None
        node = node[part]
    return node


refs: list[tuple[str, str]] = []


def collect(node, path):
    if isinstance(node, dict):
        for k, v in node.items():
            if k == "$ref":
                refs.append((v, path))
            collect(v, f"{path}/{k}")
    elif isinstance(node, list):
        for i, v in enumerate(node):
            collect(v, f"{path}[{i}]")


collect(doc, "")
for ref, where in refs:
    if resolve(ref) is None:
        errors.append(f"çözülmeyen $ref {ref} ({where})")

op_ids: dict[str, str] = {}
for path, item in doc["paths"].items():
    surface = next((s for p, s in SURFACES.items() if path.startswith(p)), None)
    if surface is None:
        errors.append(f"bilinmeyen yüzey: {path}")
    path_params = set(re.findall(r"\{(\w+)\}", path))
    shared = item.get("parameters", [])
    for method, op in item.items():
        if method not in METHODS:
            continue
        oid = op.get("operationId", f"<{method} {path}>")
        if oid in op_ids:
            errors.append(f"tekrarlanan operationId {oid}")
        op_ids[oid] = path
        if surface and not oid.startswith(surface):
            errors.append(f"{oid}: '{surface}' önekiyle başlamalı")
        if len(op.get("tags", [])) != 1:
            errors.append(f"{oid}: tam bir etiket olmalı")
        responses = op.get("responses", {})
        if not any(str(c)[0] in "23" for c in responses):
            errors.append(f"{oid}: başarı yanıtı yok")
        if not any(str(c)[0] == "4" for c in responses):
            errors.append(f"{oid}: en az bir 4xx yanıtı olmalı")
        for code, resp in responses.items():
            if str(code)[0] in "45" and "$ref" not in resp:
                errors.append(f"{oid}: {code} yanıtı components/responses'a başvurmalı")
        params = [resolve(p["$ref"]) if "$ref" in p else p for p in shared + op.get("parameters", [])]
        declared = {p["name"] for p in params if p["in"] == "path"}
        if declared != path_params:
            errors.append(f"{oid}: yol parametreleri uyuşmuyor {sorted(declared)} ≠ {sorted(path_params)}")
        header_names = {p["name"] for p in params if p["in"] == "header"}
        security = op.get("security", doc.get("security"))
        if surface in {"manage", "me", "admin"} and "/auth/" not in path and not security:
            errors.append(f"{oid}: güvenlik tanımı yok")
        if method == "post" and oid.startswith(("manageCreate", "publicCreate")) and "Idempotency-Key" not in header_names \
                and "Category" not in oid and "Override" not in oid and "TimeOff" not in oid:
            errors.append(f"{oid}: Idempotency-Key parametresi eksik")
        if method == "post" and path.endswith(STATE_ACTIONS) and "If-Match" not in header_names:
            errors.append(f"{oid}: durum eylemi If-Match almalı")


def walk_props(node, where):
    if isinstance(node, dict):
        for name, prop in (node.get("properties") or {}).items():
            if isinstance(prop, dict) and prop.get("format") == "date-time" and not name.endswith("Utc"):
                errors.append(f"{where}.{name}: date-time alanı 'Utc' ile bitmeli")
        for k, v in node.items():
            walk_props(v, f"{where}/{k}")
    elif isinstance(node, list):
        for v in node:
            walk_props(v, where)


walk_props(doc["components"]["schemas"], "schemas")

used = {r.rsplit("/", 1)[-1] for r, _ in refs if r.startswith("#/components/schemas/")}
for name in doc["components"]["schemas"]:
    if name not in used:
        errors.append(f"kullanılmayan şema {name}")

registry = Registry().with_resource("urn:oas", Resource.from_contents(doc, default_specification=DRAFT202012))


def absolutize(node):
    if isinstance(node, dict):
        return {k: (f"urn:oas{v}" if k == "$ref" else absolutize(v)) for k, v in node.items()}
    if isinstance(node, list):
        return [absolutize(v) for v in node]
    return node


for name, schema in doc["components"]["schemas"].items():
    try:
        Draft202012Validator.check_schema(schema)
    except Exception as exc:  # noqa: BLE001
        errors.append(f"geçersiz şema {name}: {str(exc)[:160]}")


def check_examples(node, where):
    if isinstance(node, dict):
        if "example" in node and any(k in node for k in ("type", "$ref", "properties")):
            schema = absolutize({k: v for k, v in node.items() if k != "example"})
            validator = Draft202012Validator(schema, registry=registry, format_checker=Draft202012Validator.FORMAT_CHECKER)
            for err in validator.iter_errors(node["example"]):
                errors.append(f"örnek şemaya uymuyor {where}: {err.message[:160]}")
        for k, v in node.items():
            check_examples(v, f"{where}/{k}")
    elif isinstance(node, list):
        for i, v in enumerate(node):
            check_examples(v, f"{where}[{i}]")


check_examples(doc, "")

rules_doc = spec_path.parent.parent / "docs" / "sozlesme" / "api-kurallari.md"
if rules_doc.exists():
    catalog = set(re.findall(r"^\| \d{3} \| `([a-z_]+)` \|", rules_doc.read_text(encoding="utf-8"), re.M))
    enum = set(doc["components"]["schemas"]["ErrorCode"]["enum"])
    if catalog != enum:
        errors.append(f"ErrorCode ↔ hata kataloğu farkı: yalnız şemada {sorted(enum - catalog)}, yalnız belgede {sorted(catalog - enum)}")

print(f"{len(op_ids)} işlem, {len(doc['components']['schemas'])} şema denetlendi.")
if errors:
    print("\n".join(f"HATA: {e}" for e in errors))
    sys.exit(1)
print("Sözleşme kuralları: tamam")
