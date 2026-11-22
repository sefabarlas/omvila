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

import 'package:omvila_api/src/model/admin_subscription_page.dart';
import 'package:omvila_api/src/model/bank_transaction.dart';
import 'package:omvila_api/src/model/bank_transaction_create.dart';
import 'package:omvila_api/src/model/bank_transaction_import_result.dart';
import 'package:omvila_api/src/model/bank_transaction_page.dart';
import 'package:omvila_api/src/model/problem.dart';

class AdminFinanceApi {

  final Dio _dio;

  const AdminFinanceApi(this._dio);

  /// Banka hareketini elle ekle
  /// CSV&#39;de olmayan bir havaleyi elle girer (source &#x3D; Manual). bankReference markalar arası benzersizdir; aynı referans varsa 409 bank_transaction_duplicate (meta.bankTransactionId). Denetime yazılır (Create, PlatformBankTransaction). İzin: platform.billing.manage (PlatformAdmin, PlatformFinance); Destek 403. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [bankTransactionCreate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [BankTransaction] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<BankTransaction>> adminCreateBankTransaction({ 
    required String idempotencyKey,
    required BankTransactionCreate bankTransactionCreate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/bank-transactions';
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
      _bodyData = jsonEncode(bankTransactionCreate);

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

    BankTransaction? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<BankTransaction, BankTransaction>(rawData, 'BankTransaction', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<BankTransaction>(
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

  /// Banka hareketlerini CSV&#39;den içe aktar
  /// Tek uç, iki kip: dryRun &#x3D; true önizlemedir (hiçbir kayıt yazılmaz), dryRun &#x3D; false aynı dosyayı uygular. Sütunlar başlıktan eşlenir (Tarih, Tutar, Gönderen, Açıklama, Referans); yalnız gelen (artı) tutarlar alınır. Kendiliğinden idempotenttir: bankReference&#39;ı zaten kayıtlı satır duplicate sayılıp atlanır, aynı dosya ikinci kez gönderilirse yeni kayıt oluşmaz. Hatalı satırlar atlanır ve errors listesinde döner. Dosya CSV değilse veya boyut/satır sınırı aşılırsa 400 validation_failed (errors.file &#x3D; [invalid_format] / [too_large]); sınırlar yapılandırmadadır. Uygulama denetime yazılır (Create, PlatformBankTransaction; satır sayıları). İzin: platform.billing.manage (PlatformAdmin, PlatformFinance); Destek 403. 
  ///
  /// Parameters:
  /// * [file] - UTF-8 CSV (virgül veya noktalı virgül ayraçlı), ilk satır başlık
  /// * [dryRun] - true = önizleme (kayıt yazılmaz), false = uygula
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [BankTransactionImportResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<BankTransactionImportResult>> adminImportBankTransactions({ 
    required MultipartFile file,
    bool? dryRun,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/bank-transactions/import';
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
            'name': 'adminBearer',
          },
        ],
        ...?extra,
      },
      contentType: 'multipart/form-data',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = FormData.fromMap(<String, dynamic>{
        r'file': file,
        if (dryRun != null) r'dryRun': dryRun,
      });

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

    BankTransactionImportResult? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<BankTransactionImportResult, BankTransactionImportResult>(rawData, 'BankTransactionImportResult', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<BankTransactionImportResult>(
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

  /// OMVILA hesabına gelen havaleler (\&quot;Banka hareketlerinde ara\&quot;)
  /// İçe aktarılan ve elle girilen banka hareketleri (PlatformBankTransaction), değer tarihine göre yeniden eskiye. q gönderen adında, açıklamada ve banka referansında arar; unmatchedOnly eşleşmemişleri süzer. Tarih aralığı en fazla 92 gün. İzin: platform.payments.match (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [q] - Gönderen adı, açıklama (ör. ABN-KE-1125) veya banka referansı
  /// * [from] - Değer tarihi alt sınırı (dahil)
  /// * [to] - Değer tarihi üst sınırı (dahil)
  /// * [unmatchedOnly] - Yalnız eşleşmemiş hareketler
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [BankTransactionPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<BankTransactionPage>> adminListBankTransactions({ 
    String? q,
    DateTime? from,
    DateTime? to,
    bool? unmatchedOnly = false,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/bank-transactions';
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
      if (q != null) r'q': q,
      if (from != null) r'from': formatDateOnly(from),
      if (to != null) r'to': formatDateOnly(to),
      if (unmatchedOnly != null) r'unmatchedOnly': unmatchedOnly,
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

    BankTransactionPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<BankTransactionPage, BankTransactionPage>(rawData, 'BankTransactionPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<BankTransactionPage>(
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

  /// Abonelikler
  /// PAbonelikler: \&quot;AKTİF 348 · ödeme düzenli\&quot;, \&quot;DENEMEDE 64 · 9&#39;u bu hafta bitiyor\&quot;, \&quot;ÖDEME GECİKTİ 11 · 7 günlük hatırlatma süresinde\&quot;, \&quot;SALT OKUNUR 3 · online randevu kapalı\&quot;, filtreler (Tümü, Denemede, Aktif, Ödeme gecikti, Salt okunur, İptal edildi, Kontörü azalan) ve tablo (Marka, Plan, Durum, Sonraki adım, Kontör). CreditsLow &#x3D; kontör LowCreditThreshold&#39;un altında. nextStep satırın \&quot;Sonraki adım\&quot; sütunudur: TrialEnds (deneme biter, date), ReadOnlyOn (salt okunura geçer, date), Renews (yenileme, date), ReactivatesOnPayment (\&quot;Ödeme gelince açılır\&quot;), SmsStopped (kontör bitti, \&quot;SMS gönderilmiyor\&quot;), ArchivesOn (iptal; arşiv günü). Sıralama: aciliyete göre (ödeme gecikti ve salt okunur önce). İzin: platform.brands.view (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [filter] - Tümü, Denemede, Aktif, Ödeme gecikti, Salt okunur, İptal edildi, Kontörü azalan
  /// * [q] - Marka adı
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminSubscriptionPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminSubscriptionPage>> adminListSubscriptions({ 
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
    final _path = r'/api/v1/admin/subscriptions';
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

    AdminSubscriptionPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminSubscriptionPage, AdminSubscriptionPage>(rawData, 'AdminSubscriptionPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminSubscriptionPage>(
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
