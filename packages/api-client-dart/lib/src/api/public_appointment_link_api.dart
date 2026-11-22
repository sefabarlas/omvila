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

import 'package:omvila_api/src/model/customer_cancel_request.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/public_appointment.dart';
import 'package:omvila_api/src/model/public_reschedule_appointment_request.dart';
import 'package:omvila_api/src/model/public_search_reschedule_slots_request.dart';
import 'package:omvila_api/src/model/slot_search_result.dart';

class PublicAppointmentLinkApi {

  final Dio _dio;

  const PublicAppointmentLinkApi(this._dio);

  /// Randevuyu iptal et (müşteri)
  /// İptal nedeni zorunludur. Son süre geçtiyse 409 cutoff_passed.
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [accessCode] - SMS bağlantısındaki kod (omvila.com/r/{code} randevu, /b/{code} bekleme teklifi veya bekleme kaydı, /d/{code} değerlendirme)
  /// * [customerCancelRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PublicAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicAppointment>> publicCancelAppointment({ 
    required String ifMatch,
    required String accessCode,
    required CustomerCancelRequest customerCancelRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/appointments/{accessCode}/cancel'.replaceAll('{' r'accessCode' '}', accessCode.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'If-Match': ifMatch,
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
      _bodyData = jsonEncode(customerCancelRequest);

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

    PublicAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicAppointment, PublicAppointment>(rawData, 'PublicAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicAppointment>(
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

  /// SMS bağlantısından randevu (\&quot;Randevum\&quot;)
  /// 
  ///
  /// Parameters:
  /// * [accessCode] - SMS bağlantısındaki kod (omvila.com/r/{code} randevu, /b/{code} bekleme teklifi veya bekleme kaydı, /d/{code} değerlendirme)
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PublicAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicAppointment>> publicGetAppointmentByCode({ 
    required String accessCode,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/appointments/{accessCode}'.replaceAll('{' r'accessCode' '}', accessCode.toString());
    final _options = Options(
      method: r'GET',
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

    PublicAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicAppointment, PublicAppointment>(rawData, 'PublicAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicAppointment>(
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

  /// Havaleyi bildir (\&quot;Havaleyi yaptım, salona bildir\&quot;)
  /// Rezervasyon sayfası Havale ekranı. Müşteri havaleyi yaptığını salona bildirir: deposit.status CustomerReported, customerReportedUtc dolar, geçmişe DepositReported yazılır, salonun Owner ve BranchManager kullanıcılarına staff.deposit_reported push&#39;u ve panel bildirimi gider (\&quot;Havale bildirildi · kontrol edin\&quot;). Randevu durumu değişmez (PendingApproval, displayStatus DepositPending); ekran \&quot;Salon onayı bekleniyor\&quot; gösterir. Kendiliğinden idempotenttir: zaten CustomerReported ise 200 ile aynı sonuç döner, bildirim ikinci kez gitmez. Bu yüzden Idempotency-Key ve If-Match almaz. Kapora beklenmiyorsa (kapora yok, Received, Waived, Refunded veya randevu PendingApproval değil) 409 invalid_state_transition; tutma süresi dolmuş ama otomatik iptal işi henüz çalışmamışsa 409 hold_expired. Kod geçersizse 404 access_code_invalid. 
  ///
  /// Parameters:
  /// * [accessCode] - SMS bağlantısındaki kod (omvila.com/r/{code} randevu, /b/{code} bekleme teklifi veya bekleme kaydı, /d/{code} değerlendirme)
  /// * [xClient] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PublicAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicAppointment>> publicReportDepositTransfer({ 
    required String accessCode,
    String? xClient,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/appointments/{accessCode}/deposit/report'.replaceAll('{' r'accessCode' '}', accessCode.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        if (xClient != null) r'X-Client': xClient,
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

    PublicAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicAppointment, PublicAppointment>(rawData, 'PublicAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicAppointment>(
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

  /// Saati değiştir (müşteri)
  /// Erteleme son süresi geçtiyse 409 cutoff_passed. Salon onaylı çalışıyorsa randevu yeniden PendingApproval olur. Alınmış kapora yeni saate aktarılır. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [accessCode] - SMS bağlantısındaki kod (omvila.com/r/{code} randevu, /b/{code} bekleme teklifi veya bekleme kaydı, /d/{code} değerlendirme)
  /// * [publicRescheduleAppointmentRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PublicAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicAppointment>> publicRescheduleAppointment({ 
    required String ifMatch,
    required String accessCode,
    required PublicRescheduleAppointmentRequest publicRescheduleAppointmentRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/appointments/{accessCode}/reschedule'.replaceAll('{' r'accessCode' '}', accessCode.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'If-Match': ifMatch,
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
      _bodyData = jsonEncode(publicRescheduleAppointmentRequest);

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

    PublicAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicAppointment, PublicAppointment>(rawData, 'PublicAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicAppointment>(
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

  /// Saat değişikliği için boş saatler (aynı hizmetler ve personel)
  /// 
  ///
  /// Parameters:
  /// * [accessCode] - SMS bağlantısındaki kod (omvila.com/r/{code} randevu, /b/{code} bekleme teklifi veya bekleme kaydı, /d/{code} değerlendirme)
  /// * [publicSearchRescheduleSlotsRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [SlotSearchResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<SlotSearchResult>> publicSearchRescheduleSlots({ 
    required String accessCode,
    required PublicSearchRescheduleSlotsRequest publicSearchRescheduleSlotsRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/appointments/{accessCode}/availability/slots'.replaceAll('{' r'accessCode' '}', accessCode.toString());
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
      _bodyData = jsonEncode(publicSearchRescheduleSlotsRequest);

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

    SlotSearchResult? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<SlotSearchResult, SlotSearchResult>(rawData, 'SlotSearchResult', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<SlotSearchResult>(
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
