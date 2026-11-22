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

import 'package:omvila_api/src/model/admin_brand_detail.dart';
import 'package:omvila_api/src/model/admin_brand_page.dart';
import 'package:omvila_api/src/model/admin_brand_status_change.dart';
import 'package:omvila_api/src/model/admin_credit_grant.dart';
import 'package:omvila_api/src/model/admin_credit_grant_result.dart';
import 'package:omvila_api/src/model/admin_credit_transaction_page.dart';
import 'package:omvila_api/src/model/admin_payment_notice_page.dart';
import 'package:omvila_api/src/model/admin_plan_change.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/subscription_invoice_page.dart';

class AdminBrandsApi {

  final Dio _dio;

  const AdminBrandsApi(this._dio);

  /// Markayı askıya al veya yeniden aç
  /// Brand.Status Active ↔ Suspended. Askıdaki markanın paneli ve rezervasyon sayfası kapanır (public uçlar 403 booking_disabled), veriler silinmez. Onboarding veya Archived markada ya da durum zaten istenen değerdeyse 409 invalid_state_transition. Identity modülünün Contracts arayüzü üzerinden yazılır (K4). Neden zorunludur; denetime yazılır (Update, Brand). İzin: platform.brands.manage (yalnız PlatformAdmin); diğer roller 403. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [brandId] - Marka kimliği
  /// * [adminBrandStatusChange] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminBrandDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminBrandDetail>> adminChangeBrandStatus({ 
    required String ifMatch,
    required String brandId,
    required AdminBrandStatusChange adminBrandStatusChange,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands/{brandId}/status'.replaceAll('{' r'brandId' '}', brandId.toString());
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
      _bodyData = jsonEncode(adminBrandStatusChange);

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

    AdminBrandDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminBrandDetail, AdminBrandDetail>(rawData, 'AdminBrandDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminBrandDetail>(
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

  /// Planı değiştir (\&quot;Planı değiştir\&quot;)
  /// Markanın planı (ve isteğe bağlı fatura dönemi) hemen değişir; yeni fiyat bir sonraki faturadan uygulanır, açık fatura değişmez. Markanın etkin personel veya şube sayısı yeni planın sınırını aşıyorsa 409 plan_limit_exceeded (meta.staffCount, meta.staffLimit, meta.branchCount, meta.includedBranchCount); ek şube ücretli plana geçişte şube sınırı yerine ek şube ücreti uygulanır. Etkin olmayan plan 400 (errors.planCode &#x3D; [invalid_format]). Denetime yazılır (Update, Subscription). İzin: platform.brands.manage (yalnız PlatformAdmin); diğer roller 403. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [brandId] - Marka kimliği
  /// * [adminPlanChange] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminBrandDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminBrandDetail>> adminChangePlan({ 
    required String ifMatch,
    required String brandId,
    required AdminPlanChange adminPlanChange,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands/{brandId}/plan'.replaceAll('{' r'brandId' '}', brandId.toString());
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
      _bodyData = jsonEncode(adminPlanChange);

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

    AdminBrandDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminBrandDetail, AdminBrandDetail>(rawData, 'AdminBrandDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminBrandDetail>(
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

  /// Marka detayı (Özet)
  /// PMarkaDetay \&quot;Özet\&quot; sekmesi: abonelik, kontör, bu ay randevu ve abonelik ödemesi kartları, \&quot;Marka bilgileri\&quot; ve son 30 günün \&quot;Kontör hareketleri\&quot;. Her açılış denetime yazılır (AuditLog Action &#x3D; View, EntityType &#x3D; Brand, changes.section &#x3D; Summary): \&quot;Bu görüntüleme denetim kaydına yazıldı.\&quot; Telefonlar her zaman maskelidir; platform kullanıcısı marka verisini değiştiremez, yalnız plan, kontör, durum ve eşleştirme işlemleri yapar. Diğer sekmeler: adminListBrandInvoices (Abonelik ve fatura), adminListBrandCreditTransactions (Kontör), adminListBrandPaymentNotices (Ödemeler), adminListAuditLog?brandId&#x3D; (Denetim kaydı). ETag, durum ve plan değişikliklerinde If-Match ile gönderilir. İzin: platform.brands.view. 
  ///
  /// Parameters:
  /// * [brandId] - Marka kimliği
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminBrandDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminBrandDetail>> adminGetBrand({ 
    required String brandId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands/{brandId}'.replaceAll('{' r'brandId' '}', brandId.toString());
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

    AdminBrandDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminBrandDetail, AdminBrandDetail>(rawData, 'AdminBrandDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminBrandDetail>(
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

  /// Kontör ekle (\&quot;Kontör ekle\&quot;)
  /// Hediye veya düzeltme kontörü (CreditTransaction, Grant; eksi değer düzeltmedir ve bakiyeyi sıfırın altına indiremez: 409 insufficient_credits). Messaging modülünün Contracts arayüzü üzerinden yazılır (K4). Denetime yazılır (Create, CreditTransaction; neden ve miktar). İzin: platform.credits.grant (yalnız PlatformAdmin); Destek ve Finans 403 forbidden. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [brandId] - Marka kimliği
  /// * [adminCreditGrant] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminCreditGrantResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminCreditGrantResult>> adminGrantCredits({ 
    required String idempotencyKey,
    required String brandId,
    required AdminCreditGrant adminCreditGrant,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands/{brandId}/credits'.replaceAll('{' r'brandId' '}', brandId.toString());
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
      _bodyData = jsonEncode(adminCreditGrant);

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

    AdminCreditGrantResult? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminCreditGrantResult, AdminCreditGrantResult>(rawData, 'AdminCreditGrantResult', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminCreditGrantResult>(
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

  /// Marka kontör hareketleri (\&quot;Kontör\&quot; sekmesi)
  /// Markanın kontör defteri (CreditTransaction), yeniden eskiye. Günlük gönderim tüketimi gün başına tek satırda toplanır (\&quot;Hatırlatma SMS&#39;leri (günlük)\&quot;). Sekmenin açılışı denetime yazılır (View, changes.section &#x3D; Credits). İzin: platform.brands.view. 
  ///
  /// Parameters:
  /// * [brandId] - Marka kimliği
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminCreditTransactionPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminCreditTransactionPage>> adminListBrandCreditTransactions({ 
    required String brandId,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands/{brandId}/credit-transactions'.replaceAll('{' r'brandId' '}', brandId.toString());
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

    AdminCreditTransactionPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminCreditTransactionPage, AdminCreditTransactionPage>(rawData, 'AdminCreditTransactionPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminCreditTransactionPage>(
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

  /// Marka faturaları (\&quot;Abonelik ve fatura\&quot; sekmesi)
  /// Markanın abonelik faturaları, yeniden eskiye. Sekmenin açılışı denetime yazılır (View, changes.section &#x3D; Invoices). İzin: platform.brands.view. 
  ///
  /// Parameters:
  /// * [brandId] - Marka kimliği
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [SubscriptionInvoicePage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<SubscriptionInvoicePage>> adminListBrandInvoices({ 
    required String brandId,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands/{brandId}/invoices'.replaceAll('{' r'brandId' '}', brandId.toString());
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

    SubscriptionInvoicePage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<SubscriptionInvoicePage, SubscriptionInvoicePage>(rawData, 'SubscriptionInvoicePage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<SubscriptionInvoicePage>(
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

  /// Marka havale bildirimleri (\&quot;Ödemeler\&quot; sekmesi)
  /// Markanın abonelik ve kontör havale bildirimleri (bütün durumlar), yeniden eskiye. Sekmenin açılışı denetime yazılır (View, changes.section &#x3D; Payments). İzin: platform.brands.view. 
  ///
  /// Parameters:
  /// * [brandId] - Marka kimliği
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminPaymentNoticePage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminPaymentNoticePage>> adminListBrandPaymentNotices({ 
    required String brandId,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands/{brandId}/payment-notices'.replaceAll('{' r'brandId' '}', brandId.toString());
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

    AdminPaymentNoticePage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminPaymentNoticePage, AdminPaymentNoticePage>(rawData, 'AdminPaymentNoticePage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminPaymentNoticePage>(
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

  /// Markalar
  /// Platform paneli Markalar (PMarkalar): \&quot;412 aktif · 64 denemede\&quot;, filtreler (Tümü, Denemede, Ödeme gecikti, Salt okunur, Kapora açık) ve tablo (Marka, Plan, Durum, Şube, Bu ay randevu). \&quot;Kapora açık\&quot; &#x3D; etkin kapora IBAN hesabı olan marka (BrandBankAccount.IsActive &#x3D; true). q marka adı, bağlantı adı (slug) veya sahibin telefonunun son 4 hanesiyle arar. Ada göre sıralı. Liste görüntülemesi denetime yazılmaz; tek markanın açılışı (adminGetBrand) yazılır. İzin: platform.brands.view (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [filter] - Tümü, Denemede, Ödeme gecikti, Salt okunur, Kapora açık
  /// * [q] - Marka adı, bağlantı adı veya sahip telefonunun son 4 hanesi
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminBrandPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminBrandPage>> adminListBrands({ 
    String? filter,
    String? q,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/brands';
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
      if (filter != null) r'filter': filter,
      if (q != null) r'q': q,
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

    AdminBrandPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminBrandPage, AdminBrandPage>(rawData, 'AdminBrandPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminBrandPage>(
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
