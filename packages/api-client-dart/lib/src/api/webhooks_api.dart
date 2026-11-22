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

import 'package:omvila_api/src/model/problem.dart';

class WebhooksApi {

  final Dio _dio;

  const WebhooksApi(this._dio);

  /// SMS teslim raporu (sağlayıcıdan gelen geri çağrı)
  /// SMS sağlayıcısı mesaj teslim edildiğinde veya başarısız olduğunda çağırır. OMVILA JWT&#39;si taşımaz (security: []); kimlik doğrulama sağlayıcıya göre adaptörde yapılır: sağlayıcının kendi imza başlığı (HMAC, ör. X-Signature) gövdenin ham baytları üzerinden ve ortam gizlisiyle sabit zamanlı karşılaştırılır; imza sunmayan bir sağlayıcı seçilirse yol gizli jetonu ve IP izin listesi kullanılır. İmzasız veya imzası tutmayan istek 401 unauthorized alır ve hiçbir kayda dokunmaz (kabul kriteri 10). Gövde sağlayıcıya özgüdür; sözleşme serbest biçimli JSON nesnesi olarak tanımlar, adaptör ProviderMessageId ile NotificationOutbox satırını bulur. Teslim edildi → Sent; başarısız → üstel geri çekilmeyle yeniden denenir, 5. denemede Failed. Bilinmeyen mesaj kimliği ve tekrar gelen rapor 204 ile sessizce kabul edilir (sağlayıcı tekrar denemesin; işlem idempotent). Form kodlu gövde gönderen bir sağlayıcı seçilirse ilgili içerik türü buraya eklenir (geriye uyumlu). 
  ///
  /// Parameters:
  /// * [provider] - SMS sağlayıcı adaptörünün kodu (ör. netgsm). Sağlayıcı henüz seçilmedi (acik-sorular T-03); bilinmeyen kod 404.
  /// * [requestBody] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future]
  /// Throws [DioException] if API call or serialization fails
  Future<Response<void>> webhookSmsDeliveryReport({ 
    required String provider,
    required Map<String, Object> requestBody,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/webhooks/sms/{provider}'.replaceAll('{' r'provider' '}', provider.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[],
        ...?extra,
      },
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(requestBody);

    } catch(error, stackTrace) {
      throw DioException(
         requestOptions: _options.compose(
          _dio.options,
          _path,
        ),
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    final _response = await _dio.request<Object>(
      _path,
      data: _bodyData,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    return _response;
  }

}
