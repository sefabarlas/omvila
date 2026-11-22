//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/manage_today.dart';
import 'package:omvila_api/src/model/my_commission_summary.dart';
import 'package:omvila_api/src/model/problem.dart';

class ManageTodayApi {

  final Dio _dio;

  const ManageTodayApi(this._dio);

  /// Primim (kişisel prim özeti)
  /// İşletme uygulaması Profil (\&quot;KASIM PRİMİM 6.840 ₺\&quot;, \&quot;Bu hafta\&quot;, \&quot;Son primler\&quot;). Yalnız oturumdaki kullanıcının kendi personel kaydının primleri (KK10, K2); rolü ne olursa olsun başka personelin primi bu uçtan görülmez (salon geneli rapor manageGetCommissionReport, Dilim 6). İzin commission.view; kullanıcının bu markada personel kaydı yoksa 404 not_found. Ay (PeriodYearMonth) şubenin yerel takvimidir; month verilmezse bu ay. Toplam &#x3D; ay içindeki bütün kayıtlar (Accrued, Approved, Paid) + ters kayıtlar (Reversed, eksi); ters kayıt yüzünden eksi olabileceği için tamsayı totalMinor ile döner. serviceRevenueMinor \&quot;Hizmet geliri\&quot; (hizmet primi tabanları toplamı). appointmentCount ayda tamamlanan randevu sayısı, noShowPercent ayın sonuçlanan randevularında Gelmedi oranı, averageRating son 90 günün ortalaması. \&quot;Prim randevu tamamlandığında hak edilir. Selin Öztürk randevusu bitince eklenecek.\&quot; metni nextAccrual&#39;dan (bugün Geldi olup tamamlanmamış ilk randevu) üretilir. Ödeme günü (\&quot;ödeme ayın 1&#39;inde\&quot;) sözleşmede yoktur (ayar değil; docs/acik-sorular.md T-101). 
  ///
  /// Parameters:
  /// * [month] - Yıl-ay (YYYY-MM), şubenin yerel takvimi
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MyCommissionSummary] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MyCommissionSummary>> manageGetMyCommissionSummary({ 
    String? month,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/me/commission-summary';
    final _options = Options(
      method: r'GET',
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
            'type': 'http',
            'scheme': 'bearer',
            'name': 'manageBearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _queryParameters = <String, dynamic>{
      if (month != null) r'month': month,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    MyCommissionSummary? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MyCommissionSummary, MyCommissionSummary>(rawData, 'MyCommissionSummary', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MyCommissionSummary>(
      data: _responseData,
      headers: _response.headers,
      isRedirect: _response.isRedirect,
      requestOptions: _response.requestOptions,
      redirects: _response.redirects,
      statusCode: _response.statusCode,
      statusMessage: _response.statusMessage,
      extra: _response.extra,
    );
  }

  /// Bugün (işletme uygulaması ana ekranı)
  /// OMVILA İşletme \&quot;Bugün\&quot; (Main, MainBos). Tek çağrıda: \&quot;Sıradaki\&quot; kartı, \&quot;Onay bekliyor\&quot; sayısı, \&quot;Sırada · ~35 dk bekleme\&quot;, kişisel prim kartı (\&quot;KASIM PRİMİ\&quot;) ve \&quot;Gün akışı\&quot; (\&quot;4/12 bitti\&quot;). Gün, şubenin saat dilimindeki bugündür; kalan süreler (\&quot;50 dk sonra\&quot;) generatedUtc&#39;ye göre sayılır, cihaz saati kullanılmaz. branchId verilmezse kullanıcının birincil şubesi. Kapsam: Staff rolü (calendar.view K) yalnız kendi kalemlerinin randevularını görür (KK10); telefon bu yanıtta hiç dönmez. Yetkiye bağlı kartlar: pendingApprovalCount yalnız appointment.approve izninde, walkIns yalnız walkin.manage izninde, commission yalnız commission.view izninde ve kullanıcının bu markada personel kaydı varsa dolu; aksi hâlde null (kart gösterilmez). Gün akışında iptal edilen randevular yoktur. 
  ///
  /// Parameters:
  /// * [branchId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [ManageToday] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ManageToday>> manageGetToday({ 
    String? branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/today';
    final _options = Options(
      method: r'GET',
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
            'type': 'http',
            'scheme': 'bearer',
            'name': 'manageBearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _queryParameters = <String, dynamic>{
      if (branchId != null) r'branchId': branchId,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    ManageToday? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ManageToday, ManageToday>(rawData, 'ManageToday', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ManageToday>(
      data: _responseData,
      headers: _response.headers,
      isRedirect: _response.isRedirect,
      requestOptions: _response.requestOptions,
      redirects: _response.redirects,
      statusCode: _response.statusCode,
      statusMessage: _response.statusMessage,
      extra: _response.extra,
    );
  }

}
