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

import 'dart:typed_data';
import 'package:omvila_api/src/model/admin_approval_queue.dart';
import 'package:omvila_api/src/model/admin_payment_notice_detail.dart';
import 'package:omvila_api/src/model/admin_payment_notice_match.dart';
import 'package:omvila_api/src/model/admin_payment_notice_resolution.dart';
import 'package:omvila_api/src/model/admin_sms_sender_decision.dart';
import 'package:omvila_api/src/model/admin_sms_sender_rejection.dart';
import 'package:omvila_api/src/model/problem.dart';

class AdminApprovalsApi {

  final Dio _dio;

  const AdminApprovalsApi(this._dio);

  /// SMS gönderen adını onayla (\&quot;Onayla\&quot;)
  /// Operatör başvurusu onaylanınca çağrılır: BrandSmsSender Approved olur, markanın SMS&#39;leri bu adla gitmeye başlar (effectiveName). Messaging modülünün Contracts arayüzü üzerinden yazılır (K4). Başvuru Pending değilse 409 invalid_state_transition. Denetime yazılır (Approve, BrandSmsSender). İzin: platform.sms-senders.decide (PlatformAdmin, PlatformSupport); Finans 403. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [senderId] - SMS gönderen adı başvurusu (BrandSmsSender) kimliği
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminSmsSenderDecision] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminSmsSenderDecision>> adminApproveSmsSender({ 
    required String ifMatch,
    required String senderId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/sms-senders/{senderId}/approve'.replaceAll('{' r'senderId' '}', senderId.toString());
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
            'name': 'adminBearer',
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

    AdminSmsSenderDecision? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminSmsSenderDecision, AdminSmsSenderDecision>(rawData, 'AdminSmsSenderDecision', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminSmsSenderDecision>(
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

  /// Havale bildirimi ve eşleşme adayları
  /// POnayDetay: \&quot;Markanın bildirimi\&quot; (Ne için, Beklenen tutar, Açıklama kodu, Gönderen, Bildirim), \&quot;Dekont\&quot; ve \&quot;OMVILA hesabına gelen havaleler\&quot;. Adaylar: eşleşmemiş banka hareketlerinden değer tarihi bildirimden 7 gün önce ile 3 gün sonrası arasında olanlar (candidateWindow); sıralama: kodu bulunan, sonra tutarı tutan, sonra tarihe yakın olan. codeFound: açıklamada kod var (büyük/küçük harf, boşluk ve tire farkı yok sayılır); amountMatches: tutar beklenen tutara eşit. verdict CodeMatches (\&quot;Kod tutuyor\&quot;) ikisi de doğruysa, aksi hâlde CheckRequired (\&quot;Kontrol edin\&quot;). Beklenen tutar tanımlı değilse amountMatches false. Açılış denetime yazılır (View, PaymentNotice). Dekont adminGetPaymentNoticeReceipt ile indirilir. İzin: platform.payments.match (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [noticeId] - Havale bildirimi (PaymentNotice) kimliği
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminPaymentNoticeDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminPaymentNoticeDetail>> adminGetPaymentNotice({ 
    required String noticeId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/payment-notices/{noticeId}'.replaceAll('{' r'noticeId' '}', noticeId.toString());
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
            'name': 'adminBearer',
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

    AdminPaymentNoticeDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminPaymentNoticeDetail, AdminPaymentNoticeDetail>(rawData, 'AdminPaymentNoticeDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminPaymentNoticeDetail>(
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

  /// Dekontu indir (\&quot;Aç\&quot;)
  /// Dekont dosyası (PDF, JPG veya PNG). Dekont yoksa 404. Cache-Control: private, no-store; indirme denetime yazılır (View, changes.section &#x3D; Receipt). İzin: platform.payments.match. 
  ///
  /// Parameters:
  /// * [noticeId] - Havale bildirimi (PaymentNotice) kimliği
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [Uint8List] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<Uint8List>> adminGetPaymentNoticeReceipt({ 
    required String noticeId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/payment-notices/{noticeId}/receipt'.replaceAll('{' r'noticeId' '}', noticeId.toString());
    final _options = Options(
      method: r'GET',
      responseType: ResponseType.bytes,
      headers: <String, dynamic>{
        ...?headers,
      },
      extra: <String, dynamic>{
        'secure': <Map<String, String>>[
          {
            'type': 'http',
            'scheme': 'bearer',
            'name': 'adminBearer',
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

    Uint8List? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : rawData as Uint8List;

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<Uint8List>(
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

  /// Onay kuyrukları (havale bildirimi, SMS başlığı)
  /// POnay / POnayBos: \&quot;7 bekleyen · en eskisi 2 gün\&quot;, sekmeler \&quot;Havale bildirimi 5\&quot; ve \&quot;SMS başlığı 2\&quot;. Yalnız bekleyen kayıtlar (PaymentNotice Pending, BrandSmsSender Pending), en eskiden yeniye. Müşteri kaporaları bu kuyruğa düşmez; onları salon işaretler. Havale satırındaki candidate en iyi eşleşme adayıdır (adminGetPaymentNotice&#39;taki ilk aday): verdict CodeMatches &#x3D; açıklamada markanın kodu var ve tutar tutuyor (\&quot;Bankada eşleşen hareket var\&quot;, \&quot;tutar ve açıklama tutuyor\&quot;); CheckRequired &#x3D; kod yok veya tutar farklı (\&quot;Kontrol edin: açıklama kodu yok; tutar ve gönderen adıyla eşleştirin\&quot;); aday yoksa candidate null. SMS başlığı satırları yalnız platform.sms-senders.decide izninde döner (Finans rolünde smsSenders sayısı 0 ve satır yok). İzin: platform.payments.match (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [type] - Sekme; verilmezse ikisi birden
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminApprovalQueue] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminApprovalQueue>> adminListApprovalQueue({ 
    String? type,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/approval-queue';
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      validateStatus: validateStatus,
    );

    final _queryParameters = <String, dynamic>{
      if (type != null) r'type': type,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    AdminApprovalQueue? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminApprovalQueue, AdminApprovalQueue>(rawData, 'AdminApprovalQueue', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminApprovalQueue>(
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

  /// Bulunamadı, markaya yaz (\&quot;Bulunamadı, markaya yaz\&quot;)
  /// Bildirim NotFound olur; markanın Owner kullanıcılarına owner.payment_notice_not_found gider (message varsa bildirime eklenir). Salon yeniden bildirebilir. Abonelik durumu değişmez. Bildirim Pending değilse 409 invalid_state_transition. Denetime yazılır (Update, PaymentNotice). İzin: platform.payments.match (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [noticeId] - Havale bildirimi (PaymentNotice) kimliği
  /// * [adminPaymentNoticeResolution] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminPaymentNoticeDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminPaymentNoticeDetail>> adminMarkPaymentNoticeNotFound({ 
    required String ifMatch,
    required String noticeId,
    AdminPaymentNoticeResolution? adminPaymentNoticeResolution,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/payment-notices/{noticeId}/not-found'.replaceAll('{' r'noticeId' '}', noticeId.toString());
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(adminPaymentNoticeResolution);

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

    AdminPaymentNoticeDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminPaymentNoticeDetail, AdminPaymentNoticeDetail>(rawData, 'AdminPaymentNoticeDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminPaymentNoticeDetail>(
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

  /// Seçili hareketle eşleştir (\&quot;Seçili hareketle eşleştir\&quot;, \&quot;Eşleştir\&quot;)
  /// Bildirim Matched olur, banka hareketi bildirime bağlanır (PlatformBankTransaction.MatchedNoticeId). Abonelikte bildirimin dönemi ödendi sayılır: fatura kesilir veya açık fatura Paid olur (PaidVia BankTransfer), abonelik Active&#39;e döner. Kontörde paketin kontörü eklenir (CreditTransaction Purchase, Messaging Contracts arayüzü üzerinden, K4). Markanın Owner kullanıcılarına owner.payment_notice_matched gider. Denetime yazılır (Match, PaymentNotice). Tutar uyuşmasa da operatör eşleştirebilir (uyarı istemcidedir). Bildirim Pending değilse 409 invalid_state_transition; hareket başka bildirime bağlıysa 409 bank_transaction_already_matched. İzin: platform.payments.match (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [noticeId] - Havale bildirimi (PaymentNotice) kimliği
  /// * [adminPaymentNoticeMatch] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminPaymentNoticeDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminPaymentNoticeDetail>> adminMatchPaymentNotice({ 
    required String ifMatch,
    required String noticeId,
    required AdminPaymentNoticeMatch adminPaymentNoticeMatch,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/payment-notices/{noticeId}/match'.replaceAll('{' r'noticeId' '}', noticeId.toString());
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(adminPaymentNoticeMatch);

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

    AdminPaymentNoticeDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminPaymentNoticeDetail, AdminPaymentNoticeDetail>(rawData, 'AdminPaymentNoticeDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminPaymentNoticeDetail>(
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

  /// Havale bildirimini reddet
  /// Geçersiz veya yinelenen bildirim için: bildirim Rejected olur, markaya owner.payment_notice_rejected gider. message zorunludur (yoksa 400 validation_failed, errors.message &#x3D; [required]). Bildirim Pending değilse 409 invalid_state_transition. Denetime yazılır (Update, PaymentNotice). İzin: platform.payments.match (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [noticeId] - Havale bildirimi (PaymentNotice) kimliği
  /// * [adminPaymentNoticeResolution] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminPaymentNoticeDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminPaymentNoticeDetail>> adminRejectPaymentNotice({ 
    required String ifMatch,
    required String noticeId,
    required AdminPaymentNoticeResolution adminPaymentNoticeResolution,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/payment-notices/{noticeId}/reject'.replaceAll('{' r'noticeId' '}', noticeId.toString());
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(adminPaymentNoticeResolution);

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

    AdminPaymentNoticeDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminPaymentNoticeDetail, AdminPaymentNoticeDetail>(rawData, 'AdminPaymentNoticeDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminPaymentNoticeDetail>(
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

  /// SMS gönderen adını reddet (\&quot;Reddet\&quot;)
  /// BrandSmsSender Rejected olur; reason salonun AyarlarBildirim ekranında rejectionReason olarak görünür. Önceki onaylı ad (yoksa OMVILA) sürer. Başvuru Pending değilse 409 invalid_state_transition. Denetime yazılır (Update, BrandSmsSender). İzin: platform.sms-senders.decide (PlatformAdmin, PlatformSupport). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [senderId] - SMS gönderen adı başvurusu (BrandSmsSender) kimliği
  /// * [adminSmsSenderRejection] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminSmsSenderDecision] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminSmsSenderDecision>> adminRejectSmsSender({ 
    required String ifMatch,
    required String senderId,
    required AdminSmsSenderRejection adminSmsSenderRejection,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/sms-senders/{senderId}/reject'.replaceAll('{' r'senderId' '}', senderId.toString());
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(adminSmsSenderRejection);

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

    AdminSmsSenderDecision? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminSmsSenderDecision, AdminSmsSenderDecision>(rawData, 'AdminSmsSenderDecision', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminSmsSenderDecision>(
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
