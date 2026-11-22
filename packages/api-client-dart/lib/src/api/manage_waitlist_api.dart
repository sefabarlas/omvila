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

import 'package:omvila_api/src/model/manage_waitlist_create.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/waitlist_entry_view.dart';
import 'package:omvila_api/src/model/waitlist_list.dart';
import 'package:omvila_api/src/model/waitlist_offer_request.dart';

class ManageWaitlistApi {

  final Dio _dio;

  const ManageWaitlistApi(this._dio);

  /// Bekleme listesine ekle (\&quot;Listeye ekle\&quot;)
  /// Panelden bekleme kaydı (Source &#x3D; Panel). newCustomer verilirse müşteri kaydı açılır (numara kayıtlıysa 409 phone_already_exists + meta.brandCustomerId). Müşteriye bildirim gitmez. Uygun saat boşalınca teklif motoru bu kaydı da sıraya alır. İzin: waitlist.manage (Owner, BranchManager kendi şubesinde). 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [manageWaitlistCreate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [WaitlistEntryView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WaitlistEntryView>> manageAddToWaitlist({ 
    required String idempotencyKey,
    required ManageWaitlistCreate manageWaitlistCreate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/waitlist';
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
      _bodyData = jsonEncode(manageWaitlistCreate);

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

    WaitlistEntryView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WaitlistEntryView, WaitlistEntryView>(rawData, 'WaitlistEntryView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WaitlistEntryView>(
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

  /// Bekleme listesi
  /// BeklemeListesi ve BeklemeListesiBos ekranları. Şubenin Waiting kayıtları, öncelik ve kayıt sırasıyla (teklifin gideceği sıra); bekleyen teklifi olanlarda pendingOffer dolu (\&quot;Teklif gönderildi\&quot;, \&quot;11 dakika içinde yanıt bekleniyor\&quot;). Sayfalanmaz: liste kısadır. Kalan süre istemcide pendingOffer.holdExpiresUtc − generatedUtc ile sayılır (cihaz saati kullanılmaz). İzin: waitlist.manage (Owner, BranchManager kendi şubesinde); personel 403. 
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
  /// Returns a [Future] containing a [Response] with a [WaitlistList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WaitlistList>> manageListWaitlist({ 
    required String branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/waitlist';
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

    WaitlistList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WaitlistList, WaitlistList>(rawData, 'WaitlistList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WaitlistList>(
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

  /// Saat öner (\&quot;Saat öner\&quot;)
  /// BeklemeListesi \&quot;Saat öner\&quot;: salon bekleyen kişiye elle bir saat teklif eder. Teklif motorundaki teklifle aynıdır: WaitlistOffer(Pending), holdExpiresUtc &#x3D; şimdi + WaitlistOfferHoldMinutes, saat bu süre boyunca online müşteriye gösterilmez, müşteriye waitlist.offer gider (push → SMS, sessiz saate kaydırılmaz). Saat dolu veya personel müsait değilse 409 slot_taken; kayıt Waiting değilse veya bekleyen teklifi varsa 409 invalid_state_transition; SMS gerekip kontör yoksa 409 insufficient_credits (teklif oluşmaz). İzin: waitlist.manage (Owner, BranchManager kendi şubesinde). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [entryId] 
  /// * [waitlistOfferRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [WaitlistEntryView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WaitlistEntryView>> manageOfferSlot({ 
    required String ifMatch,
    required String entryId,
    required WaitlistOfferRequest waitlistOfferRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/waitlist/{entryId}/offer'.replaceAll('{' r'entryId' '}', entryId.toString());
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
      _bodyData = jsonEncode(waitlistOfferRequest);

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

    WaitlistEntryView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WaitlistEntryView, WaitlistEntryView>(rawData, 'WaitlistEntryView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WaitlistEntryView>(
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

  /// Teklifi geri al (\&quot;Teklifi geri al\&quot;)
  /// BeklemeListesi \&quot;Teklifi geri al\&quot;: bekleyen teklif Withdrawn olur, saatin tutması kalkar (saat yeniden online görünür, otomatik olarak sıradakine teklif edilmez). Kişi listede kalır (Waiting). Müşteriye bildirim gitmez; teklif bağlantısı \&quot;Bu saatin süresi doldu\&quot; gösterir. Bekleyen teklif yoksa (kabul, ret veya süre dolması daha önce olduysa) 409 invalid_state_transition. İzin: waitlist.manage (Owner, BranchManager kendi şubesinde). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [entryId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [WaitlistEntryView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<WaitlistEntryView>> manageWithdrawWaitlistOffer({ 
    required String ifMatch,
    required String entryId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/waitlist/{entryId}/offer/withdraw'.replaceAll('{' r'entryId' '}', entryId.toString());
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

    WaitlistEntryView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<WaitlistEntryView, WaitlistEntryView>(rawData, 'WaitlistEntryView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<WaitlistEntryView>(
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
