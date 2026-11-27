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

import 'dart:typed_data';
import 'package:omvila_api/src/model/problem.dart';

class PublicDataExportApi {

  final Dio _dio;

  const PublicDataExportApi(this._dio);

  /// Dışa aktarma arşivini indir (e-postadaki bağlantı)
  /// owner.data_export_ready e-postasındaki bağlantının açtığı yol (\&quot;Tüm verileri dışa aktar\&quot;, manageRequestDataExport). Giriş istemez: yetki jetonun imzası ve süresidir (DataExportLinkValidityDays). Yanıt musteriler.csv, randevular.csv ve odemeler.csv dosyalarını taşıyan tek ZIP arşividir. Jeton bozuk, imzası geçersiz veya süresi dolmuşsa ya da arşiv silinmişse 404 not_found (neden ayrıştırılmaz). Bağlantı süresi içinde yeniden açılabilir. Panel ve uygulamalar bu yolu çağırmaz; bağlantıyı tarayıcı açar. 
  ///
  /// Parameters:
  /// * [token] - E-postadaki bağlantının imzalı ve süreli jetonu (base64url; marka, istek ve geçerlilik sonunu taşır)
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [Uint8List] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<Uint8List>> publicDownloadDataExport({ 
    required String token,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/data-exports/{token}'.replaceAll('{' r'token' '}', token.toString());
    final _options = Options(
      method: r'GET',
      responseType: ResponseType.bytes,
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[],
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

    Uint8List? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : rawData as Uint8List;

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<Uint8List>(
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
