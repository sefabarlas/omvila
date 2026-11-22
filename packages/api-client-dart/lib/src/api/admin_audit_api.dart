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

import 'package:omvila_api/src/model/audit_log_page.dart';
import 'package:omvila_api/src/model/problem.dart';

class AdminAuditApi {

  final Dio _dio;

  const AdminAuditApi(this._dio);

  /// Denetim kaydını CSV olarak dışa aktar (\&quot;Dışa aktar\&quot;)
  /// adminListAuditLog ile aynı süzgeçler; aralık en fazla 92 gün (aşılırsa 400 validation_failed, errors.toUtc &#x3D; [out_of_range]). Sütunlar: Zaman (Europe/Istanbul), Kim, Rol, İşlem, Varlık, Marka, Ayrıntı (maskeli). Dışa aktarma da denetime yazılır (Export, AuditLog). İzin: platform.audit.export (yalnız PlatformAdmin); Destek ve Finans 403. 
  ///
  /// Parameters:
  /// * [category] - Görüntüleme, Onay ve ret, Kontör ve plan, KVKK
  /// * [q] - \"Marka veya kişi ara\": marka adı veya işlemi yapanın adı
  /// * [brandId] - Yalnız bu markanın kayıtları (marka detayı \"Denetim kaydı\" sekmesi)
  /// * [fromUtc] - Alt sınır (dahil); verilmezse son 24 saat
  /// * [toUtc] - Üst sınır (hariç); verilmezse şimdi
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [String] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<String>> adminExportAuditLog({ 
    String? category,
    String? q,
    String? brandId,
    DateTime? fromUtc,
    DateTime? toUtc,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/audit-log/export';
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

    final _queryParameters = <String, dynamic>{
      if (category != null) r'category': category,
      if (q != null) r'q': q,
      if (brandId != null) r'brandId': brandId,
      if (fromUtc != null) r'fromUtc': fromUtc,
      if (toUtc != null) r'toUtc': toUtc,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    String? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<String, String>(rawData, 'String', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<String>(
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

  /// Denetim kaydı
  /// PDenetim \&quot;Tüm platform işlemleri · silinemez\&quot;: filtreler (Son 24 saat, Görüntüleme, Onay ve ret, Kontör ve plan, KVKK), \&quot;Marka veya kişi ara\&quot; ve tablo (Zaman, Kim, İşlem, Marka, Ayrıntı). \&quot;Son 24 saat\&quot; istemcide fromUtc ile verilir; fromUtc verilmezse son 24 saat. Kategori eşlemesi: View &#x3D; Action View; ApproveReject &#x3D; Approve, Match ve PaymentNotice/BrandSmsSender güncellemeleri; CreditsAndPlan &#x3D; CreditTransaction ve Subscription kayıtları; Kvkk &#x3D; Anonymize ve Export. İşlem adı (\&quot;Marka görüntülendi\&quot;, \&quot;Havale eşleştirildi\&quot;, \&quot;Salt okunura geçti\&quot;) istemcide action + entityType + changes&#39;ten üretilir. changes&#39;te telefon ve ad maskelidir. Kayıtlar 2 yıl saklanır ve değiştirilemez. Yeniden eskiye. İzin: platform.audit.view (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [category] - Görüntüleme, Onay ve ret, Kontör ve plan, KVKK
  /// * [q] - \"Marka veya kişi ara\": marka adı veya işlemi yapanın adı
  /// * [brandId] - Yalnız bu markanın kayıtları (marka detayı \"Denetim kaydı\" sekmesi)
  /// * [fromUtc] - Alt sınır (dahil); verilmezse son 24 saat
  /// * [toUtc] - Üst sınır (hariç); verilmezse şimdi
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AuditLogPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AuditLogPage>> adminListAuditLog({ 
    String? category,
    String? q,
    String? brandId,
    DateTime? fromUtc,
    DateTime? toUtc,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/audit-log';
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

    final _queryParameters = <String, dynamic>{
      if (category != null) r'category': category,
      if (q != null) r'q': q,
      if (brandId != null) r'brandId': brandId,
      if (fromUtc != null) r'fromUtc': fromUtc,
      if (toUtc != null) r'toUtc': toUtc,
      if (limit != null) r'limit': limit,
      if (cursor != null) r'cursor': cursor,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    AuditLogPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AuditLogPage, AuditLogPage>(rawData, 'AuditLogPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AuditLogPage>(
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
