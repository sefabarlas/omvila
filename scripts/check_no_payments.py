#!/usr/bin/env python3
"""Ödeme SDK'sı ve kart formu taraması (ürün kuralı 1: OMVILA ödeme almaz; Dilim 4 kabul kriteri 10).

İki tarama yapar, ikisi de yalnız git'in izlediği dosyalarda (node_modules, derleme çıktısı vb. dışarıda):
  1. Bağımlılık bildirimleri: package.json, pnpm-lock.yaml, *.csproj, Directory.Packages.props,
     pubspec.yaml, pubspec.lock. Paket adları aşağıdaki YASAK_PAKETLER listesiyle karşılaştırılır
     (doğrudan ve dolaylı bağımlılıklar; kilit dosyaları dolaylıları da içerir).
  2. Kaynak kod (web/, packages/, mobile/, api/src/, contracts/openapi.yaml): KART_FORMU_IZLERI
     (cc-number/cc-exp/cc-csc otomatik doldurma, cardNumber, CVV/CVC, "Kart numarası",
     "Son kullanma tarihi", ödeme SDK'sı betik adresleri).

Taranmayanlar: design/ ve docs/ (salt okunur referans ve kural metni), test dosyaları (yokluğu
doğrulayan testler), bu betik.

İzin (gerçekten gerekiyorsa, gerekçesiyle):
  - Kaynak kodda: aynı satıra veya bir üst satıra `odeme-taramasi: izinli <gerekçe>` yorumu.
    Gerekçesiz işaret geçersizdir.
  - Paketlerde: IZINLI_PAKETLER sözlüğüne paket adı ve gerekçe.
Not: OMVILA aboneliğinin kartla çekilmesi (Dilim 6) tek istisnadır; o iş bu listeleri bilinçli olarak
günceller. in_app_purchase gibi mağaza içi satın alma paketleri müşteriden ödeme almak olmadığından
listede yoktur.

Kullanım (depo kökünden): python3 scripts/check_no_payments.py
"""
import re
import subprocess
import sys
from pathlib import PurePosixPath

# Paket adıyla tam eşleşen düzenli ifadeler (büyük/küçük harf duyarsız). npm, NuGet ve pub bir arada.
YASAK_PAKETLER = [
    # Stripe
    r"stripe", r"stripe[-_.].+", r"@stripe/.+", r"flutter_stripe", r"stripe\.net",
    # iyzico
    r"iyzipay.*", r"iyzico.*", r"@iyzico/.+",
    # PayTR, Param, Craftgate, Sipay, Papara (Türkiye)
    r"paytr.*", r"@paytr/.+", r"param-pos", r"craftgate.*", r"@craftgate/.+", r"sipay.*", r"papara.*",
    # Braintree (@braintree/sanitize-url bir URL temizleme aracıdır, ödeme değildir)
    r"braintree.*", r"@braintree/(?!sanitize-url$).+", r"flutter_braintree",
    # Adyen, PayPal, Square, Mollie, Checkout.com, Razorpay, Klarna
    r"adyen.*", r"@adyen/.+", r"paypal.*", r"@paypal/.+", r"flutter_paypal.*",
    r"square", r"square\..+", r"@square/.+", r"square_in_app_payments",
    r"mollie.*", r"@mollie/.+", r"@checkout\.com/.+", r"checkout-sdk-node", r"checkoutsdk",
    r"razorpay.*", r"klarna.*", r"@klarna/.+",
    # Google Pay / Apple Pay (Dart `pay` ailesi)
    r"pay", r"pay_(android|ios|platform_interface)",
    # Kart formu bileşenleri
    r"react-credit-cards.*", r"react-payment-inputs", r"card-validator", r"credit-card-type",
    r"payment", r"flutter_credit_card", r"credit_card_.+",
]

# Paket adı -> gerekçe. Boş tutulması beklenir.
IZINLI_PAKETLER = {}

KART_FORMU_IZLERI = [
    (r"autocomplete\s*[=:]\s*[\"'{]?\s*[\"']?cc-", "kart otomatik doldurma (cc-*)"),
    (r"AutofillHints\.creditCard", "kart otomatik doldurma (Flutter)"),
    (r"\b(credit_?)?card_?number\b", "kart numarası alanı"),
    (r"\bcard_?(cvc|cvv|expiry|expiration)\w*", "kart güvenlik/son kullanma alanı"),
    (r"\bcv[cv]2?\b", "CVV/CVC alanı"),
    (r"kart\s+numaras[ıi]", "\"Kart numarası\" metni"),
    (r"son\s+kullanma\s+tarihi", "\"Son kullanma tarihi\" metni"),
    (r"js\.stripe\.com|iyzipay\.com|paytr\.com|checkout\.com|paypal\.com/sdk|js\.braintreegateway",
     "ödeme SDK'sı adresi"),
]

KAYNAK_KOKLERI = ("web/", "packages/", "mobile/", "api/src/")
KAYNAK_DOSYALARI = {"contracts/openapi.yaml"}
HARIC_KOKLER = ("design/", "docs/")
TEST_YOLU = re.compile(
    r"(^|/)(tests?|e2e|__tests__|integration_test)/|\.(test|spec)\.[cm]?[jt]sx?$|_test\.dart$|Tests?\.cs$"
)
IZIN_ISARETI = re.compile(r"odeme-taramasi:\s*izinli\b\s*[-:—]?\s*(\S.{4,})", re.IGNORECASE)
BU_BETIK = "scripts/check_no_payments.py"

yasak_re = re.compile(r"^(?:" + "|".join(YASAK_PAKETLER) + r")$", re.IGNORECASE)
iz_re = [(re.compile(d, re.IGNORECASE), ad) for d, ad in KART_FORMU_IZLERI]


def izlenen_dosyalar():
    cikti = subprocess.run(["git", "ls-files", "-z"], check=True, capture_output=True).stdout
    return [p for p in cikti.decode("utf-8").split("\0") if p]


def oku(yol):
    try:
        with open(yol, encoding="utf-8") as f:
            return f.read()
    except (UnicodeDecodeError, FileNotFoundError, IsADirectoryError):
        return None


def paket_adlari(yol, metin):
    """(satır no, paket adı) çiftleri."""
    ad = PurePosixPath(yol).name
    satirlar = metin.splitlines()
    if ad == "package.json":
        bolum = False
        for i, s in enumerate(satirlar, 1):
            if re.match(r'\s*"(dependencies|devDependencies|peerDependencies|optionalDependencies)"\s*:', s):
                bolum = True
                continue
            if bolum and re.match(r"\s*}", s):
                bolum = False
            m = bolum and re.match(r'\s*"([^"]+)"\s*:', s)
            if m:
                yield i, m.group(1)
    elif ad == "pnpm-lock.yaml":
        for i, s in enumerate(satirlar, 1):
            m = re.match(r"^  '?((?:@[^/\s']+/)?[^@\s'(/]+)@", s)
            if m:
                yield i, m.group(1)
            m = re.match(r"^      '?((?:@[^/\s']+/)?[^@\s':]+)'?:\s*$", s)
            if m and m.group(1) not in ("dependencies", "devDependencies", "optionalDependencies"):
                yield i, m.group(1)
    elif ad.endswith(".csproj") or ad == "Directory.Packages.props":
        for i, s in enumerate(satirlar, 1):
            for m in re.finditer(r'<Package(?:Reference|Version)\s+(?:Include|Update)="([^"]+)"', s):
                yield i, m.group(1)
    elif ad == "pubspec.yaml":
        bolum = False
        for i, s in enumerate(satirlar, 1):
            if re.match(r"^(dependencies|dev_dependencies|dependency_overrides):\s*$", s):
                bolum = True
                continue
            if re.match(r"^\S", s):
                bolum = False
            m = bolum and re.match(r"^  ([a-z0-9_]+):", s)
            if m:
                yield i, m.group(1)
    elif ad == "pubspec.lock":
        for i, s in enumerate(satirlar, 1):
            m = re.match(r"^  ([a-z0-9_]+):\s*$", s)
            if m:
                yield i, m.group(1)


def bildirim_mi(yol):
    ad = PurePosixPath(yol).name
    return ad in ("package.json", "pnpm-lock.yaml", "Directory.Packages.props", "pubspec.yaml", "pubspec.lock") \
        or ad.endswith(".csproj")


def kaynak_mi(yol):
    if yol in KAYNAK_DOSYALARI:
        return True
    return yol.startswith(KAYNAK_KOKLERI) and not TEST_YOLU.search(yol) and not bildirim_mi(yol)


def main():
    bulgular = []
    for yol in izlenen_dosyalar():
        if yol.startswith(HARIC_KOKLER) or yol == BU_BETIK:
            continue
        if bildirim_mi(yol):
            metin = oku(yol)
            if metin is None:
                continue
            gorulen = set()
            for no, paket in paket_adlari(yol, metin):
                if paket in gorulen or not yasak_re.match(paket):
                    continue
                gorulen.add(paket)
                if paket in IZINLI_PAKETLER and IZINLI_PAKETLER[paket].strip():
                    continue
                bulgular.append(f"{yol}:{no}: ödeme SDK'sı / kart paketi: {paket}")
        elif kaynak_mi(yol):
            metin = oku(yol)
            if metin is None:
                continue
            satirlar = metin.splitlines()
            for no, s in enumerate(satirlar, 1):
                for desen, ad in iz_re:
                    if not desen.search(s):
                        continue
                    onceki = satirlar[no - 2] if no > 1 else ""
                    if IZIN_ISARETI.search(s) or IZIN_ISARETI.search(onceki):
                        continue
                    bulgular.append(f"{yol}:{no}: {ad}: {s.strip()[:120]}")
                    break

    if bulgular:
        print("Ödeme SDK'sı veya kart formu izi bulundu. OMVILA ödeme almaz (AGENTS.md § 1, kural 1).")
        print("Kapora salonun IBAN'ına havaledir; kart formu ve ödeme SDK'sı eklenmez.\n")
        for b in bulgular:
            print("  " + b)
        print(f"\n{len(bulgular)} bulgu. Gerçekten gerekiyorsa ve ürün sahibi onayladıysa: {BU_BETIK} başındaki "
              "izin yöntemine bakın.")
        return 1
    print("Ödeme SDK'sı ve kart formu taraması temiz.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
