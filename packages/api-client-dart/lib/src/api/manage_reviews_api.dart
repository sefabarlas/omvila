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
import 'package:omvila_api/src/model/review_page.dart';
import 'package:omvila_api/src/model/review_reply_request.dart';
import 'package:omvila_api/src/model/review_view.dart';

class ManageReviewsApi {

  final Dio _dio;

  const ManageReviewsApi(this._dio);

  /// Değerlendirmeler
  /// Degerlendirmeler ve DegerlendirmelerBos ekranları. filter: AwaitingReply (\&quot;Yanıt bekleyen\&quot;, düşük puanlılar önce, sonra eskiden yeniye), All (\&quot;Tümü\&quot;, yeniden eskiye), LowRating (\&quot;3 yıldız ve altı\&quot;, yeniden eskiye). overview filtreden bağımsızdır ve şube filtresine uyar. Panelde müşterinin tam adı görünür; rezervasyon sayfasında ad + soyadın baş harfi gösterilir. İzin: review.view (Owner, BranchManager kendi şubesinde); personel 403. 
  ///
  /// Parameters:
  /// * [branchId] 
  /// * [filter] 
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [ReviewPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ReviewPage>> manageListReviews({ 
    String? branchId,
    String? filter,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/reviews';
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
      if (branchId != null) r'branchId': branchId,
      if (filter != null) r'filter': filter,
      if (limit != null) r'limit': limit,
      if (cursor != null) r'cursor': cursor,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    ReviewPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ReviewPage, ReviewPage>(rawData, 'ReviewPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ReviewPage>(
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

  /// Değerlendirmeye yanıt ver (\&quot;Yanıtla\&quot;)
  /// Degerlendirmeler kartındaki yanıt alanı (\&quot;Yanıtınızı yazın, müşteri görür\&quot;). Yanıt varsa yenisiyle değiştirilir (repliedUtc güncellenir). Müşteriye bildirim gitmez; yanıt müşteri uygulamasında ve rezervasyon sayfasında yorumun altında görünür. İzin: review.reply (Owner, BranchManager kendi şubesinde); personel 403. 
  ///
  /// Parameters:
  /// * [reviewId] 
  /// * [reviewReplyRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [ReviewView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ReviewView>> manageReplyReview({ 
    required String reviewId,
    required ReviewReplyRequest reviewReplyRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/reviews/{reviewId}/reply'.replaceAll('{' r'reviewId' '}', reviewId.toString());
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
      _bodyData = jsonEncode(reviewReplyRequest);

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

    ReviewView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ReviewView, ReviewView>(rawData, 'ReviewView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ReviewView>(
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
