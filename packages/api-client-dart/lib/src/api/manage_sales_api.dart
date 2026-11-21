//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/cash_day.dart';
import 'package:omvila_api/src/model/cash_day_close_request.dart';
import 'package:omvila_api/src/model/problem.dart';

class ManageSalesApi {

  final Dio _dio;

  const ManageSalesApi(this._dio);

  /// Günü kapat (\&quot;Günü kapat\&quot;, Kasa sayımı)
  /// Kasa ekranı \&quot;Kasa sayımı\&quot; + \&quot;Günü kapat\&quot;. Beklenen nakit sunucuda hesaplanır (nakit tahsilatlar − o gün işaretlenen nakit iadeler, docs/acik-sorular.md T-86); fark &#x3D; sayılan − beklenen. Fark 0 değilse differenceNote zorunludur (yoksa 400 validation_failed, errors.differenceNote &#x3D; [required]). Gün bugünden sonraysa 400 validation_failed (errors.date &#x3D; [out_of_range]). Gün zaten kapatılmışsa 409 cash_day_already_closed. Kapanmış güne yeni tahsilat yazılabilir; raporda recordedAfterDayClose ile işaretlenir. Para kaydı oluşturduğu için Idempotency-Key ister. İzin: cash.close (Owner, BranchManager kendi şubesinde). 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [cashDayCloseRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CashDay] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CashDay>> manageCloseCashDay({ 
    required String idempotencyKey,
    required CashDayCloseRequest cashDayCloseRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/cash/close';
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'Idempotency-Key': idempotencyKey,
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
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(cashDayCloseRequest);

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

    CashDay? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CashDay, CashDay>(rawData, 'CashDay', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CashDay>(
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

  /// Kasa (günün tahsilatları)
  /// Kasa ve KasaBos ekranları. Şubenin bir yerel günündeki bütün tahsilat kayıtları (randevu, kapora, paket satışı), yöntem bazında toplamlar, beklenen nakit ve gün kapatma kaydı. date verilmezse şubenin bugünü. \&quot;Önceki günler\&quot; aynı uçla date değiştirilerek açılır. Kayıt yoksa payments boş, toplamlar 0 döner (KasaBos). İzin: payment.collect (Owner, BranchManager kendi şubesinde); personel 403. 
  ///
  /// Parameters:
  /// * [branchId] 
  /// * [date] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CashDay] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CashDay>> manageGetCashDay({ 
    required String branchId,
    DateTime? date,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/cash';
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
      r'branchId': branchId,
      if (date != null) r'date': date,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    CashDay? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CashDay, CashDay>(rawData, 'CashDay', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CashDay>(
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
