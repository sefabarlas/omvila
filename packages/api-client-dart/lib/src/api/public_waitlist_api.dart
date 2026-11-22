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
import 'package:omvila_api/src/model/public_appointment.dart';
import 'package:omvila_api/src/model/public_waitlist_entry.dart';
import 'package:omvila_api/src/model/public_waitlist_join.dart';
import 'package:omvila_api/src/model/public_waitlist_offer.dart';

class PublicWaitlistApi {

  final Dio _dio;

  const PublicWaitlistApi(this._dio);

  /// Teklifi kabul et (\&quot;Randevuyu al\&quot;)
  /// Teklif Accepted, bekleme kaydı Converted olur ve randevu online kurallarıyla oluşur (A1, docs/acik-sorular.md T-86): salon onay veya kapora istiyorsa randevu PendingApproval oluşur ve yanıtta depositInstructions bulunur; değilse Confirmed. Müşteriye appointment.confirmed veya appointment.requested gider. Kendiliğinden idempotenttir: kabul edilmiş teklif tekrar gelirse aynı randevu 200 ile döner. Süre dolmuşsa 409 hold_expired; reddedilmiş veya geri alınmışsa 409 invalid_state_transition; engellenmiş müşteri 403 customer_blocked. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
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
  Future<Response<PublicAppointment>> publicAcceptWaitlistOffer({ 
    required String ifMatch,
    required String accessCode,
    String? xClient,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/waitlist-offers/{accessCode}/accept'.replaceAll('{' r'accessCode' '}', accessCode.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'If-Match': ifMatch,
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

  /// Teklifi reddet (\&quot;Bu saat olmaz\&quot;)
  /// Teklif Declined olur, saat sıradaki uygun kişiye teklif edilir. Kişi listede kalır (Waiting). Zaten reddedilmişse aynı sonuç 200 ile döner; kabul edilmiş veya geri alınmışsa 409 invalid_state_transition; süresi dolmuşsa 409 hold_expired. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [accessCode] - SMS bağlantısındaki kod (omvila.com/r/{code} randevu, /b/{code} bekleme teklifi veya bekleme kaydı, /d/{code} değerlendirme)
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PublicWaitlistOffer] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicWaitlistOffer>> publicDeclineWaitlistOffer({ 
    required String ifMatch,
    required String accessCode,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/waitlist-offers/{accessCode}/decline'.replaceAll('{' r'accessCode' '}', accessCode.toString());
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
      validateStatus: validateStatus,
    );

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    PublicWaitlistOffer? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicWaitlistOffer, PublicWaitlistOffer>(rawData, 'PublicWaitlistOffer', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicWaitlistOffer>(
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

  /// Bekleme teklifi (\&quot;Beklediğiniz saat boşaldı\&quot;)
  /// Teklif bağlantısı (omvila.com/b/{accessCode}; BeklemeTeklif, BeklemeDoldu). Bağlantı telefonu doğrular, kod istenmez. Süresi dolmuş, reddedilmiş veya geri alınmış teklif de 200 ile döner (status Expired, Declined, Withdrawn); ekran BeklemeDoldu gösterir. Kod geçersizse 404 access_code_invalid. 
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
  /// Returns a [Future] containing a [Response] with a [PublicWaitlistOffer] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicWaitlistOffer>> publicGetWaitlistOffer({ 
    required String accessCode,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/waitlist-offers/{accessCode}'.replaceAll('{' r'accessCode' '}', accessCode.toString());
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

    PublicWaitlistOffer? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicWaitlistOffer, PublicWaitlistOffer>(rawData, 'PublicWaitlistOffer', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicWaitlistOffer>(
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

  /// Bekleme listesine katıl (\&quot;Bekleme listesine katıl\&quot;)
  /// Rezervasyon sayfası DoluGun ve müşteri uygulaması BeklemeKatil. Telefon doğrulamasından sonra çağrılır (publicBookingBearer veya meBearer). Kayıt WaitlistEntry(Waiting, Source &#x3D; Online) olur; müşteri kaydı yoksa açılır (aydınlatma metni onayı randevudaki gibi kaydedilir). Aynı müşterinin aynı şubede aynı hizmetler için Waiting kaydı varsa yenisi açılmaz; tercihleri güncellenir ve 200 ile döner. Uygun saat boşalınca waitlist.offer gider (push → SMS) ve 15 dk tutulur. Engellenmiş müşteri 403 customer_blocked; online randevu kapalıysa 403 booking_disabled. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [branchId] 
  /// * [publicWaitlistJoin] 
  /// * [xClient] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PublicWaitlistEntry] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicWaitlistEntry>> publicJoinWaitlist({ 
    required String idempotencyKey,
    required String branchId,
    required PublicWaitlistJoin publicWaitlistJoin,
    String? xClient,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/branches/{branchId}/waitlist'.replaceAll('{' r'branchId' '}', branchId.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'Idempotency-Key': idempotencyKey,
        if (xClient != null) r'X-Client': xClient,
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
            'type': 'http',
            'scheme': 'bearer',
            'name': 'publicBookingBearer',
          },{
            'type': 'http',
            'scheme': 'bearer',
            'name': 'meBearer',
          },
        ],
        ...?extra,
      },
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(publicWaitlistJoin);

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

    PublicWaitlistEntry? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicWaitlistEntry, PublicWaitlistEntry>(rawData, 'PublicWaitlistEntry', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicWaitlistEntry>(
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

  /// Listeden çık (\&quot;Listeden çık\&quot;)
  /// BeklemeDoldu \&quot;Listeden çık\&quot;. accessCode bekleme kaydının kodudur (PublicWaitlistOffer.waitlistAccessCode, PublicWaitlistEntry.accessCode). Kayıt Cancelled olur; bekleyen teklif varsa Declined olur ve saat sıradakine geçer. Kendiliğinden idempotenttir (zaten çıkmışsa 200). Kayıt Converted veya Expired ise 409 invalid_state_transition. Kod geçersizse 404 access_code_invalid. 
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
  /// Returns a [Future] containing a [Response] with a [PublicWaitlistEntry] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicWaitlistEntry>> publicLeaveWaitlist({ 
    required String accessCode,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/waitlist/{accessCode}/leave'.replaceAll('{' r'accessCode' '}', accessCode.toString());
    final _options = Options(
      method: r'POST',
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

    PublicWaitlistEntry? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicWaitlistEntry, PublicWaitlistEntry>(rawData, 'PublicWaitlistEntry', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicWaitlistEntry>(
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
