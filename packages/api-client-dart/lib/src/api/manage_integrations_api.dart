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

import 'package:omvila_api/src/model/google_calendar_callback_request.dart';
import 'package:omvila_api/src/model/google_calendar_connect_url.dart';
import 'package:omvila_api/src/model/google_calendar_connection.dart';
import 'package:omvila_api/src/model/problem.dart';

class ManageIntegrationsApi {

  final Dio _dio;

  const ManageIntegrationsApi(this._dio);

  /// Google Takvim bağlantısını tamamla
  /// Sağlayıcıdan dönen code ve state ile bağlantıyı kurar (CalendarLink Active); varsa eski bağlantının yerini alır ve gelecekteki randevular aktarılmaya başlar. state geçersiz, süresi dolmuş veya başka kullanıcınınsa 400 validation_failed (errors.state &#x3D; [invalid_format]); sağlayıcı code&#39;u reddederse errors.code &#x3D; [invalid_format]. Kullanıcının personel kaydı yoksa 404. İzin: calendar.view. 
  ///
  /// Parameters:
  /// * [googleCalendarCallbackRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [GoogleCalendarConnection] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<GoogleCalendarConnection>> manageCompleteGoogleCalendarConnect({ 
    required GoogleCalendarCallbackRequest googleCalendarCallbackRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/integrations/google/callback';
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
      _bodyData = jsonEncode(googleCalendarCallbackRequest);

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

    GoogleCalendarConnection? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<GoogleCalendarConnection, GoogleCalendarConnection>(rawData, 'GoogleCalendarConnection', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<GoogleCalendarConnection>(
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

  /// Google Takvim bağlantısını kes
  /// Bağlantı Revoked olur, saklanan jetonlar silinir, sağlayıcıdaki erişim iptal edilir. Google&#39;a aktarılmış etkinlikler silinmez. Bağlantı yoksa da 204 (idempotent). Kullanıcının personel kaydı yoksa 404. İzin: calendar.view. 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future]
  /// Throws [DioException] if API call or serialization fails
  Future<Response<void>> manageDisconnectGoogleCalendar({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/integrations/google';
    final _options = Options(
      method: r'DELETE',
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

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    return _response;
  }

  /// Google Takvim bağlantı adresi
  /// Sağlayıcının izin sayfası adresi ve tek kullanımlık state değeri (10 dk geçerli, kullanıcıya bağlı). İstemci kullanıcıyı url&#39;e yönlendirir; sağlayıcı code ve state ile panelin geri dönüş sayfasına döner, panel bunları manageCompleteGoogleCalendarConnect&#39;e gönderir. Sahte adaptörde url doğrudan geri dönüş sayfasını gösterir (K7). Kullanıcının personel kaydı yoksa 404. İzin: calendar.view. 
  ///
  /// Parameters:
  /// * [client] - Geri dönüş adresinin seçimi (web paneli veya işletme uygulaması)
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [GoogleCalendarConnectUrl] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<GoogleCalendarConnectUrl>> manageGetGoogleCalendarConnectUrl({ 
    String? client,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/integrations/google/connect-url';
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
      if (client != null) r'client': client,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    GoogleCalendarConnectUrl? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<GoogleCalendarConnectUrl, GoogleCalendarConnectUrl>(rawData, 'GoogleCalendarConnectUrl', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<GoogleCalendarConnectUrl>(
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

  /// Google Takvim bağlantı durumu (oturumdaki personel)
  /// Oturumdaki kullanıcının kendi personel kaydının Google Takvim bağlantısı (CalendarLink). Aktarım tek yönlüdür (OMVILA → Google); Google&#39;daki değişiklikler geri gelmez. Bağlantı yoksa status &#x3D; NotConnected. Diğer personelin durumu StaffDetail.googleCalendarStatus&#39;tadır. v1&#39;de sağlayıcı sahte adaptörle çalışır, gerçek OAuth istemcisi mağaza hazırlığıyla açılır (K7). Kullanıcının bu markada personel kaydı yoksa 404. İzin: calendar.view. 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [GoogleCalendarConnection] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<GoogleCalendarConnection>> manageGetGoogleCalendarStatus({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/integrations/google';
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

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    GoogleCalendarConnection? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<GoogleCalendarConnection, GoogleCalendarConnection>(rawData, 'GoogleCalendarConnection', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<GoogleCalendarConnection>(
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
