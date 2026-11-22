//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/admin_overview.dart';
import 'package:omvila_api/src/model/admin_session.dart';
import 'package:omvila_api/src/model/problem.dart';

class AdminOverviewApi {

  final Dio _dio;

  const AdminOverviewApi(this._dio);

  /// Platform genel bakış (aktivasyon hunisi, dikkat isteyen markalar)
  /// Platform paneli Genel bakış (Main): \&quot;AKTİF MARKA 412 · +18 bu ay\&quot;, \&quot;ONAY BEKLEYEN 7 · 5 havale bildirimi · 2 SMS başlığı\&quot;, \&quot;DENEMEDE 64 · 9&#39;u bu hafta bitiyor\&quot;, \&quot;ÖDEME GECİKTİ 11 · 3&#39;ü salt okunura geçmek üzere\&quot;, \&quot;Aktivasyon hunisi · son 30 gün\&quot; ve \&quot;Dikkat isteyen markalar\&quot;. Aktif marka: Brand.Status Active ve aboneliği Trialing, Active veya PastDue olan markalar; newThisMonth bu ay kaydolanlar (Europe/Istanbul takvimi). \&quot;Bu hafta bitiyor\&quot;: deneme bitişi bugünden itibaren 7 gün içinde. \&quot;Salt okunura geçmek üzere\&quot;: salt okunur günü 2 gün içinde. Huni: son 30 günde kaydolan markalardan kaçının hizmet eklediği, çalışma saatini girdiği ve ilk online randevuyu aldığı. Dikkat isteyen markalar: ödemesi geciken, kontörü biten ve denemesi 3 gün içinde biten markalar; en fazla 10 satır, aciliyete göre sıralı. İzin: platform.brands.view (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminOverview] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminOverview>> adminGetOverview({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/overview';
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    AdminOverview? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminOverview, AdminOverview>(rawData, 'AdminOverview', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminOverview>(
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

  /// Platform oturumu (kullanıcı, rol, izinler)
  /// Platform paneli açılışında bir kez çağrılır; menü ve butonlar permissions listesine göre kurulur (ör. Destek rolünde \&quot;Kontör ekle\&quot; ve \&quot;Planı değiştir\&quot; kilitli). Gizlemek yetkilendirme değildir; her uç izni sunucuda yeniden denetler (docs/alan/roller-yetkiler.md § Platform rolleri). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminSession] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminSession>> adminGetSession({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/session';
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    AdminSession? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminSession, AdminSession>(rawData, 'AdminSession', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminSession>(
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
