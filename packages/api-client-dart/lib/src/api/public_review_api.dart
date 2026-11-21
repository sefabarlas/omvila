//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/public_review_create.dart';
import 'package:omvila_api/src/model/public_review_invite.dart';

class PublicReviewApi {

  final Dio _dio;

  const PublicReviewApi(this._dio);

  /// Değerlendirmeyi gönder (\&quot;Gönder\&quot;)
  /// Yalnız Completed randevu ve bir kez değerlendirilir: değerlendirilmişse 409 already_reviewed; randevu Completed değilse 409 invalid_state_transition. Değerlendirme yayınlanmış olarak kaydedilir (yorumda ad + soyadın baş harfi görünür); salonun Owner ve BranchManager kullanıcılarına staff.review_received gider (personele gitmez, docs/acik-sorular.md T-86). Rezervasyon sayfasındaki puan, yayınlanmış değerlendirme sayısı ReviewPublicThreshold&#39;u geçince ve salon gösteriyorsa (showReviews) PublicBrand.rating ile döner. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [accessCode] - SMS bağlantısındaki kod (omvila.com/r/{code} randevu, /b/{code} bekleme teklifi veya bekleme kaydı, /d/{code} değerlendirme)
  /// * [publicReviewCreate] 
  /// * [xClient] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PublicReviewInvite] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicReviewInvite>> publicCreateReview({ 
    required String idempotencyKey,
    required String accessCode,
    required PublicReviewCreate publicReviewCreate,
    String? xClient,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/reviews/{accessCode}'.replaceAll('{' r'accessCode' '}', accessCode.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'Idempotency-Key': idempotencyKey,
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
      _bodyData = jsonEncode(publicReviewCreate);

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

    PublicReviewInvite? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicReviewInvite, PublicReviewInvite>(rawData, 'PublicReviewInvite', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicReviewInvite>(
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

  /// Değerlendirme bağlantısı (\&quot;Kuaför Elif&#39;te nasıldı?\&quot;)
  /// review.invite bağlantısı (omvila.com/d/{accessCode}); müşteri uygulamasındaki Degerlendir ekranının web karşılığı. Giriş istemez. Değerlendirilmişse alreadyReviewed &#x3D; true ve review dolu döner. Kod geçersiz veya süresi dolmuşsa 404 access_code_invalid. 
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
  /// Returns a [Future] containing a [Response] with a [PublicReviewInvite] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PublicReviewInvite>> publicGetReviewInvite({ 
    required String accessCode,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/public/reviews/{accessCode}'.replaceAll('{' r'accessCode' '}', accessCode.toString());
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

    PublicReviewInvite? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PublicReviewInvite, PublicReviewInvite>(rawData, 'PublicReviewInvite', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PublicReviewInvite>(
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
