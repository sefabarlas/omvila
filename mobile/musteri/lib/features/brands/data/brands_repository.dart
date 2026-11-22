import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/api_call.dart';

/// Salonun nasıl eklendiği.
enum BrandSource {
  /// "QR kodu okut".
  qr,

  /// "Bağlantıyı yapıştır" veya derin bağlantı.
  link,
}

/// Salonlarım (meListBrands, meSaveBrand).
abstract interface class BrandsRepository {
  /// Randevu alınan ve kaydedilen salonlar.
  Future<List<MeBrand>> list();

  /// Salonu ekler; müşteri kaydı açılmaz (KK1). Bilinmeyen bağlantı
  /// `not_found`.
  Future<MeBrand> save(String brandSlug, BrandSource source);
}

/// Üretilmiş istemciyle.
final class ApiBrandsRepository implements BrandsRepository {
  /// [api] müşteri jetonuyla.
  const ApiBrandsRepository(this.api);

  /// API.
  final OmvilaApi api;

  @override
  Future<List<MeBrand>> list() => apiCall(() async {
    final response = await api.getMeBrandsApi().meListBrands();
    return response.data!.items;
  });

  @override
  Future<MeBrand> save(String brandSlug, BrandSource source) =>
      apiCall(() async {
        final response = await api.getMeBrandsApi().meSaveBrand(
          meSaveBrandRequest: MeSaveBrandRequest(
            brandSlug: brandSlug,
            source_: switch (source) {
              BrandSource.qr => MeSaveBrandRequestSource_Enum.qr,
              BrandSource.link => MeSaveBrandRequestSource_Enum.link,
            },
          ),
        );
        return response.data!;
      });
}
