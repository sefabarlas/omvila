/// Kişiye özel bağlantı önekleri (bildirimler.md: /r/ randevu, /b/ bekleme
/// teklifi, /d/ değerlendirme); salon adı olamaz.
const linkPrefixes = {'r', 'b', 'd'};

final _slug = RegExp(r'^[a-z0-9](?:[a-z0-9-]{0,58}[a-z0-9])?$');

/// Yapıştırılan metinden veya QR değerinden salon bağlantı adını çıkarır
/// ("randevu.omvila.com.tr/kuafor-elif" → "kuafor-elif"). Salon bağlantısı
/// değilse null.
String? brandSlugFromText(String text, Set<String> hosts) {
  for (final token in text.trim().split(RegExp(r'\s+'))) {
    final raw = token.contains('://') ? token : 'https://$token';
    final uri = Uri.tryParse(raw);
    if (uri == null || !(uri.scheme == 'https' || uri.scheme == 'http')) {
      continue;
    }

    final host = uri.host.toLowerCase().replaceFirst(RegExp(r'^www\.'), '');
    if (!hosts.contains(host)) {
      continue;
    }

    final segments = uri.pathSegments.where((s) => s.isNotEmpty).toList();
    if (segments.length != 1) {
      return null;
    }

    final slug = segments.single.toLowerCase();
    return _slug.hasMatch(slug) && !linkPrefixes.contains(slug) ? slug : null;
  }

  return null;
}

/// Uygulama içi adresin salon bağlantı adı olup olmadığı (derin bağlantı
/// `/{brandSlug}`).
bool isBrandSlug(String value) =>
    _slug.hasMatch(value) && !linkPrefixes.contains(value);
