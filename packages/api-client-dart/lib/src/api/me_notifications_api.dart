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

import 'package:omvila_api/src/model/device_registration.dart';
import 'package:omvila_api/src/model/mark_notifications_read_request.dart';
import 'package:omvila_api/src/model/me_notification_page.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/registered_device.dart';
import 'package:omvila_api/src/model/unread_notification_count.dart';

class MeNotificationsApi {

  final Dio _dio;

  const MeNotificationsApi(this._dio);

  /// Bildirimler (müşteri)
  /// Müşteriye giden push&#39;lu olayların uygulama içi kaydı, yeniden eskiye (Bildirimler, BildirimlerBos). Kayıt push tercihinden ve teslimden bağımsız yazılır (bildirim izni kapalı olsa da görünür). title ve body olay anındaki push metniyle aynıdır (docs/standartlar/bildirimler.md § Müşteri olayları); kod, IBAN ve not içeriği yazılmaz. Zildeki nokta unreadCount &gt; 0 iken gösterilir (sahte sayı yok). target dokununca açılacak ekrandır. 
  ///
  /// Parameters:
  /// * [limit] 
  /// * [cursor] 
  /// * [unreadOnly] - Yalnız okunmamışlar
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeNotificationPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeNotificationPage>> meListNotifications({ 
    int? limit = 50,
    String? cursor,
    bool? unreadOnly,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/notifications';
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
            'name': 'meBearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _queryParameters = <String, dynamic>{
      if (limit != null) r'limit': limit,
      if (cursor != null) r'cursor': cursor,
      if (unreadOnly != null) r'unreadOnly': unreadOnly,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    MeNotificationPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeNotificationPage, MeNotificationPage>(rawData, 'MeNotificationPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeNotificationPage>(
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

  /// Bildirimleri okundu say
  /// manageMarkNotificationsRead ile aynı kurallar: notificationIds (dokunulan bildirim) veya all &#x3D; true; ikisi birden veya hiçbiri 400 validation_failed. İdempotenttir; başka müşterinin bildirimi sessizce yok sayılır. Yanıt güncel okunmamış sayısıdır. 
  ///
  /// Parameters:
  /// * [markNotificationsReadRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [UnreadNotificationCount] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<UnreadNotificationCount>> meMarkNotificationsRead({ 
    required MarkNotificationsReadRequest markNotificationsReadRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/notifications/read';
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
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
      _bodyData = jsonEncode(markNotificationsReadRequest);

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

    UnreadNotificationCount? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<UnreadNotificationCount, UnreadNotificationCount>(rawData, 'UnreadNotificationCount', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<UnreadNotificationCount>(
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

  /// Müşteri uygulaması push jetonunu kaydet (\&quot;BildirimIzni\&quot;)
  /// Müşteri kendi izin ekranımızda \&quot;Bildirimlere izin ver\&quot; dedikten sonra, açılışta ve FCM jetonu yenilendiğinde çağrılır (Device: OwnerType Customer, App Customer). manageRegisterDevice ile aynı kurallar: doğal anahtar push jetonudur, aynı jeton güncellenir ve aynı deviceId döner (Idempotency-Key istenmez); jeton başka hesaba kayıtlıysa o kayıt pasifleşir. Aktif cihaz varken müşteri bildirimleri önce push denenir, ulaşmazsa SMS&#39;e düşer. \&quot;Şimdi değil\&quot; diyen kullanıcıda cihaz kaydı yoktur, hatırlatmalar SMS ile gelir (MainBildirimKapali şeridi istemcidedir). 
  ///
  /// Parameters:
  /// * [deviceRegistration] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [RegisteredDevice] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<RegisteredDevice>> meRegisterDevice({ 
    required DeviceRegistration deviceRegistration,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/devices';
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
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
      _bodyData = jsonEncode(deviceRegistration);

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

    RegisteredDevice? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<RegisteredDevice, RegisteredDevice>(rawData, 'RegisteredDevice', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<RegisteredDevice>(
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
