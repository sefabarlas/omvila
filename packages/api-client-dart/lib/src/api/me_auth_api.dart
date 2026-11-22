//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/me_auth_result.dart';
import 'package:omvila_api/src/model/me_logout_request.dart';
import 'package:omvila_api/src/model/me_otp_request.dart';
import 'package:omvila_api/src/model/me_refresh_request.dart';
import 'package:omvila_api/src/model/otp_challenge_response.dart';
import 'package:omvila_api/src/model/otp_verify_request.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/token_pair.dart';

class MeAuthApi {

  final Dio _dio;

  const MeAuthApi(this._dio);

  /// Oturumu kapat (\&quot;Çıkış yap\&quot;)
  /// Gönderilen yenileme jetonunun zinciri iptal edilir. pushToken gönderilirse o cihaz kaydı pasifleşir (çıkış yapan telefona push gitmez). Uygulama jetonları yerelde de siler. İstek kendiliğinden idempotenttir. 
  ///
  /// Parameters:
  /// * [meLogoutRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future]
  /// Throws [DioException] if API call or serialization fails
  Future<Response<void>> meLogout({ 
    MeLogoutRequest? meLogoutRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/auth/logout';
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
      _bodyData = jsonEncode(meLogoutRequest);

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

  /// Erişim jetonunu yenile (müşteri)
  /// Yenileme jetonu gövdede gönderilir ve her kullanımda döndürülür (rotation): yanıttaki yeni refreshToken saklanır, eskisi geçersizdir. Kullanılmış bir jeton yeniden gelirse o zincirdeki bütün jetonlar iptal edilir ve 401 unauthorized döner (uygulama girişe döner). 
  ///
  /// Parameters:
  /// * [meRefreshRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [TokenPair] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<TokenPair>> meRefreshToken({ 
    required MeRefreshRequest meRefreshRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/auth/refresh';
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
      _bodyData = jsonEncode(meRefreshRequest);

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

    TokenPair? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<TokenPair, TokenPair>(rawData, 'TokenPair', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<TokenPair>(
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

  /// SMS kodu iste (müşteri uygulaması girişi)
  /// Müşteri uygulaması \&quot;Giriş\&quot; ekranındaki \&quot;Kod gönder\&quot;. Kod her zaman OMVILA adıyla SMS&#39;tir (otp.code, OtpChallenge.Purpose CustomerLogin). Numaranın kayıtlı olup olmadığı yanıtta açık edilmez (her durumda 202). Yeniden gönderim ve deneme sınırları salon girişiyle aynıdır (429 otp_resend_too_soon, otp_attempts_exceeded). 
  ///
  /// Parameters:
  /// * [meOtpRequest] 
  /// * [xClient] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [OtpChallengeResponse] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<OtpChallengeResponse>> meRequestOtp({ 
    required MeOtpRequest meOtpRequest,
    String? xClient,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/auth/otp/request';
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
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(meOtpRequest);

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

    OtpChallengeResponse? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<OtpChallengeResponse, OtpChallengeResponse>(rawData, 'OtpChallengeResponse', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<OtpChallengeResponse>(
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

  /// SMS kodunu doğrula, oturum aç (\&quot;GirisKod\&quot;)
  /// Kod doğruysa müşteri oturumu açılır (CustomerSession). Numaraya ait global Customer kaydı yoksa açılır; kayıt IsVerified ve HasAppAccount olur. Bu işlem hiçbir markada BrandCustomer açmaz. Jetonlar gövdede döner (yalnız mobil istemci); uygulama yenileme jetonunu flutter_secure_storage&#39;da saklar. rememberDevice alanı bu uçta yok sayılır. Kod hatalıysa 422 otp_invalid (meta.attemptsLeft), süresi dolmuşsa 422 otp_expired. 
  ///
  /// Parameters:
  /// * [otpVerifyRequest] 
  /// * [xClient] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeAuthResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeAuthResult>> meVerifyOtp({ 
    required OtpVerifyRequest otpVerifyRequest,
    String? xClient,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/auth/otp/verify';
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
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(otpVerifyRequest);

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

    MeAuthResult? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeAuthResult, MeAuthResult>(rawData, 'MeAuthResult', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeAuthResult>(
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
