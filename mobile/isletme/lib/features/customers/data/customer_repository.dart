import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_riverpod/misc.dart';
import 'package:omvila_api/omvila_api.dart';
import 'package:omvila_isletme/app/providers.dart';
import 'package:omvila_ui/omvila_ui.dart';

Future<T> _call<T>(Future<Response<T>> Function() request) async {
  try {
    return (await request()).data as T;
  } on DioException catch (e) {
    throw ApiProblem.fromDioException(e);
  }
}

/// Müşteri listesi süzgeci (sözleşmedeki `filter` değerleri).
enum CustomerFilter {
  /// Süzgeçsiz.
  all('All', 'Tümü'),

  /// "Düzenli".
  regular('Regular', 'Düzenli'),

  /// "Yeni".
  newCustomers('New', 'Yeni'),

  /// "Paketli".
  withPackage('WithPackage', 'Paketli');

  const CustomerFilter(this.value, this.label);

  /// Sorgu değeri.
  final String value;

  /// Çip metni.
  final String label;
}

/// Liste sorgusu.
typedef CustomerQuery = ({String q, CustomerFilter filter});

/// Yüklenen sayfalar.
@immutable
class CustomerPage {
  /// Sayfalar.
  const CustomerPage(this.items, this.nextCursor, {this.loadingMore = false});

  /// Şimdiye kadar okunanlar.
  final List<CustomerSummary> items;

  /// Sonraki sayfa imleci.
  final String? nextCursor;

  /// Sonraki sayfa okunuyor.
  final bool loadingMore;
}

/// Müşteriler (Musteriler, MusterilerAramaYok, MusterilerBos): arama ad,
/// telefon veya son 4 hane; personel rolünde telefon maskeli döner.
class CustomerListController extends AsyncNotifier<CustomerPage> {
  /// Denetleyici.
  CustomerListController(this.query);

  /// Sorgu.
  final CustomerQuery query;

  Future<ManageListCustomers200Response> _fetch(String? cursor) => _call(
    () => ref
        .read(apiProvider)
        .getManageCustomersApi()
        .manageListCustomers(
          q: query.q.isEmpty ? null : query.q,
          filter: query.filter == CustomerFilter.all
              ? null
              : query.filter.value,
          cursor: cursor,
        ),
  );

  @override
  Future<CustomerPage> build() async {
    ref.watch(apiProvider);
    final page = await _fetch(null);
    return CustomerPage(page.items, page.nextCursor);
  }

  /// "Daha fazla göster".
  Future<void> loadMore() async {
    final current = state.value;
    final cursor = current?.nextCursor;
    if (current == null || cursor == null || current.loadingMore) {
      return;
    }

    state = AsyncData(CustomerPage(current.items, cursor, loadingMore: true));
    try {
      final page = await _fetch(cursor);
      state = AsyncData(
        CustomerPage([...current.items, ...page.items], page.nextCursor),
      );
    } on ApiProblem {
      state = AsyncData(current);
      rethrow;
    }
  }
}

/// Müşteri listesi.
final AsyncNotifierProviderFamily<
  CustomerListController,
  CustomerPage,
  CustomerQuery
>
customerListProvider = AsyncNotifierProvider.autoDispose
    .family<CustomerListController, CustomerPage, CustomerQuery>(
      CustomerListController.new,
    );

/// Müşteri kartı (MusteriKarti).
final FutureProviderFamily<CustomerSummary, String> customerProvider =
    FutureProvider.autoDispose.family<CustomerSummary, String>(
      (ref, id) => _call(
        () => ref
            .watch(apiProvider)
            .getManageCustomersApi()
            .manageGetCustomer(brandCustomerId: id),
      ),
    );

/// Kart "Geçmiş": yeniden eskiye; personel rolünde yalnız kendi randevuları.
final FutureProviderFamily<List<CustomerAppointmentRow>, String>
customerAppointmentsProvider = FutureProvider.autoDispose
    .family<List<CustomerAppointmentRow>, String>((ref, id) async {
      final page = await _call(
        () => ref
            .watch(apiProvider)
            .getManageCustomersApi()
            .manageListCustomerAppointments(brandCustomerId: id, limit: 20),
      );
      return page.items;
    });

/// "Müşteri ekle" (Idempotency-Key). Numara kayıtlıysa sunucu
/// `409 phone_already_exists` + `meta.brandCustomerId` döner.
final createCustomerProvider =
    Provider<Future<CustomerSummary> Function(CustomerCreate, String)>(
      (ref) =>
          (body, key) => _call(
            () => ref
                .read(apiProvider)
                .getManageCustomersApi()
                .manageCreateCustomer(
                  idempotencyKey: key,
                  customerCreate: body,
                ),
          ),
    );
