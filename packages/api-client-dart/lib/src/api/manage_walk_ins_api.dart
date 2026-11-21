//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/walk_in_create.dart';
import 'package:omvila_api/src/model/walk_in_queue.dart';
import 'package:omvila_api/src/model/walk_in_start_request.dart';
import 'package:omvila_api/src/model/walk_in_started.dart';
import 'package:omvila_api/src/model/walk_in_ticket_view.dart';

class ManageWalkInsApi {

  final Dio _dio;

  const ManageWalkInsApi(this._dio);

  /// Sıraya ekle (\&quot;Sıraya ekle\&quot;)
  /// Kapıda sıra \&quot;Sıraya ekle\&quot; (salon paneli, işletme uygulaması SiraEkle). Numara şube + gün bazında atomik verilir (BranchQueueCounter). Telefon isteğe bağlıdır; verilirse önünde 1 kişi kalınca walkin.almost_turn SMS&#39;i gider (\&quot;Sıranız yaklaştı\&quot;). Müşteri kaydı bilet oluşurken açılmaz, \&quot;Başlat\&quot;ta açılır (telefonsuz bilet adla açılır, docs/acik-sorular.md T-86). İzin: walkin.manage. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [walkInCreate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [WalkInTicketView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WalkInTicketView>> manageCreateWalkIn({ 
    required String idempotencyKey,
    required WalkInCreate walkInCreate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/walk-in-tickets';
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
      _bodyData = jsonEncode(walkInCreate);

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

    WalkInTicketView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WalkInTicketView, WalkInTicketView>(rawData, 'WalkInTicketView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WalkInTicketView>(
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

  /// Sıradan çıkar (\&quot;Sıradan çıkar\&quot;)
  /// Bilet Left olur; arkadakilerin tahmini beklemesi yeniden hesaplanır. Bilet Waiting veya Called değilse 409 invalid_state_transition. İzin: walkin.manage
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [ticketId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [WalkInTicketView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WalkInTicketView>> manageLeaveWalkIn({ 
    required String ifMatch,
    required String ticketId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/walk-in-tickets/{ticketId}/leave'.replaceAll('{' r'ticketId' '}', ticketId.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'If-Match': ifMatch,
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

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    WalkInTicketView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WalkInTicketView, WalkInTicketView>(rawData, 'WalkInTicketView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WalkInTicketView>(
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

  /// Kapıda sıra
  /// KapidaSira ve KapidaSiraBos ekranları, işletme uygulaması Sıra. Şubenin bugünkü (şubenin yerel günü) Waiting ve Called biletleri, numara sırasıyla; numaralar her gün 1&#39;den başlar. Bekleme süreleri istemcide generatedUtc − joinedUtc ile sayılır. Tahmini bekleme &#x3D; öndekilerin kalan hizmet süreleri / müsait personel (WalkInAverageServiceMinutes). İzin: walkin.manage (Owner, BranchManager kendi şubesinde, personel kendi şubelerinde). 
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
  /// Returns a [Future] containing a [Response] with a [WalkInQueue] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WalkInQueue>> manageListWalkIns({ 
    required String branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/walk-in-tickets';
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
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    WalkInQueue? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WalkInQueue, WalkInQueue>(rawData, 'WalkInQueue', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WalkInQueue>(
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

  /// Başlat / Randevuya çevir (\&quot;Randevuya çevir\&quot;)
  /// Bileti randevuya çevirir (A1, kapıda kanal): şimdi başlayan, Channel &#x3D; WalkIn ve Status &#x3D; CheckedIn bir randevu oluşur, takvimde \&quot;Kapıdan\&quot; görünür; bilet Served olur. Müşteri: bilette brandCustomerId varsa o; telefon varsa numarayla bulunur veya açılır (Source &#x3D; WalkIn); telefon yoksa adla telefonsuz müşteri kaydı açılır (docs/acik-sorular.md T-86). Müşteriye bildirim gitmez. Personel \&quot;Fark etmez\&quot; ise veya bilette hizmet yoksa ilgili alan gövdede zorunludur (400 validation_failed). Personel o an meşgulse 409 slot_taken; bilet Waiting veya Called değilse 409 invalid_state_transition. İzin: walkin.manage ve appointment.create (personel yalnız kendine). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [ticketId] 
  /// * [walkInStartRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [WalkInStarted] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WalkInStarted>> manageStartWalkIn({ 
    required String ifMatch,
    required String ticketId,
    WalkInStartRequest? walkInStartRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/walk-in-tickets/{ticketId}/start'.replaceAll('{' r'ticketId' '}', ticketId.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'If-Match': ifMatch,
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
      _bodyData = jsonEncode(walkInStartRequest);

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

    WalkInStarted? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WalkInStarted, WalkInStarted>(rawData, 'WalkInStarted', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WalkInStarted>(
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
