import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_musteri/shared/api_call.dart';

/// Paketlerim ve Ödemelerim (meListPackages, meListPayments); salt okunur.
abstract interface class WalletRepository {
  /// Paketler: önce kullanılabilirler, sonra bitenler.
  Future<MePackageList> packages();

  /// Ödeme kayıtları, yeniden eskiye.
  Future<MePaymentPage> payments({String? cursor});
}

/// Üretilmiş istemciyle.
final class ApiWalletRepository implements WalletRepository {
  /// [api] müşteri jetonuyla.
  const ApiWalletRepository(this.api);

  /// API.
  final OmvilaApi api;

  @override
  Future<MePackageList> packages() => apiCall(() async {
    final response = await api.getMeWalletApi().meListPackages();
    return response.data!;
  });

  @override
  Future<MePaymentPage> payments({String? cursor}) => apiCall(() async {
    final response = await api.getMeWalletApi().meListPayments(cursor: cursor);
    return response.data!;
  });
}
