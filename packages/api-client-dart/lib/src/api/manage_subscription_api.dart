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
import 'package:omvila_api/src/model/credit_package_list.dart';
import 'package:omvila_api/src/model/credit_purchase_instructions.dart';
import 'package:omvila_api/src/model/credit_purchase_request.dart';
import 'package:omvila_api/src/model/payment_notice_purpose.dart';
import 'package:omvila_api/src/model/payment_notice_summary.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/subscription_detail.dart';
import 'package:omvila_api/src/model/subscription_invoice_page.dart';

class ManageSubscriptionApi {

  final Dio _dio;

  const ManageSubscriptionApi(this._dio);

  /// Fatura PDF&#39;i (\&quot;PDF\&quot;)
  /// Faturanın bilgi amaçlı PDF&#39;i; sunucuda üretilir (e-Arşiv değil). Başka markanın faturası veya olmayan fatura 404 not_found. Yanıt Cache-Control: private, no-store taşır. İzin: subscription.manage (Owner). 
  ///
  /// Parameters:
  /// * [invoiceId] - Abonelik faturası kimliği
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [Uint8List] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<Uint8List>> manageGetInvoicePdf({ 
    required String invoiceId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/subscription/invoices/{invoiceId}/pdf'.replaceAll('{' r'invoiceId' '}', invoiceId.toString());
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

  /// OMVILA aboneliği (Ayarlar → Abonelik ve fatura)
  /// AyarlarAbonelik ekranı: deneme/gecikme şeridi, \&quot;PLAN Salon · aylık · deneme\&quot;, \&quot;ŞUBE 2 · 1 dahil + 1 ek şube\&quot;, \&quot;PERSONEL 5 · … dahil\&quot; kartları ve \&quot;Havale / EFT ile öde\&quot; kutusu (Alıcı, IBAN, Açıklama ABN-…, Tutar). Faturalar ayrı uçtadır (manageListInvoices). Fiyat ve OMVILA hesap bilgisi yapılandırmadan okunur (S-14): plan fiyatı tanımlı değilse amountDue null, OMVILA hesabı tanımlı değilse omvilaBankAccount null döner; istemci bu alanları boş gösterir, köşeli parantezli yer tutucu yazmaz. Boş sınır (includedBranchCount, staffLimit) sınırsız demektir. transferCode markanın sabit abonelik kodudur (ABN-…); her dönem aynıdır. Kartla ödeme (T-06) sözleşmede yoktur; kart alanı eklenince geriye uyumlu gelir. Salt okunur abonelikte de çağrılabilir. İzin: subscription.manage (yalnız Owner); diğer roller 403 forbidden. 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [SubscriptionDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<SubscriptionDetail>> manageGetSubscription({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/subscription';
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

    SubscriptionDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<SubscriptionDetail, SubscriptionDetail>(rawData, 'SubscriptionDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<SubscriptionDetail>(
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

  /// SMS kontör paketleri
  /// Kontör satın alma penceresindeki paketler (Başlangıç 1.000, Orta 5.000, Büyük 20.000 SMS). Yalnız etkin paketler; fiyat tanımlı değilse price null (S-14). İzin: subscription.manage (Owner). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CreditPackageList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CreditPackageList>> manageListCreditPackages({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/credit-packages';
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

    CreditPackageList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CreditPackageList, CreditPackageList>(rawData, 'CreditPackageList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CreditPackageList>(
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

  /// Faturalar
  /// AyarlarAbonelik \&quot;Faturalar\&quot; tablosu (No, Dönem, Tutar, Durum, PDF). Yeniden eskiye. Faturalar bilgi amaçlıdır, e-Arşiv değildir (\&quot;e-Arşiv değil, bilgi amaçlı\&quot;). Fatura numarası boşluksuz sıradır (OMV-2026-000184). Liste boşsa items boş dizi döner (deneme süresinde fatura yoktur). İzin: subscription.manage (Owner). 
  ///
  /// Parameters:
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
  Future<Response<SubscriptionInvoicePage>> manageListInvoices({ 
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/subscription/invoices';
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

  /// Havaleyi bildir (\&quot;Havaleyi bildir (dekont isteğe bağlı)\&quot;)
  /// Salonun OMVILA hesabına yaptığı havaleyi bildirmesi (PaymentNotice, Pending). Bildirim platform onay kuyruğuna düşer; ekip banka hareketiyle eşleştirince abonelikte dönem ödendi sayılır (fatura Paid, durum Active), kontörde paket bakiyeye eklenir ve owner.payment_notice_matched gider. \&quot;Havale ekibimiz tarafından 1 iş günü içinde eşleştirilir. Gönderdiyseniz bildirin, beklerken panel açık kalır.\&quot; purpose &#x3D; Subscription: beklenen tutar o anki amountDue (fiyat tanımlı değilse boş), açıklama kodu markanın ABN- kodudur. purpose &#x3D; Credits: creditPackageId zorunludur (managePurchaseCredits ile seçilen paket); beklenen tutar paket fiyatı, açıklama kodu markanın KNT- kodudur. Dekont isteğe bağlıdır: en fazla 5 MB, PDF, JPG veya PNG (guvenlik-kvkk.md); aşılırsa 400 validation_failed (errors.receipt &#x3D; [too_large] / [invalid_format]). Aynı amaç için (kontörde aynı paket için) bekleyen bildirim varsa 409 payment_notice_already_pending (meta.paymentNoticeId). Salt okunur ve ödeme gecikti durumunda da çağrılabilir (ödemenin yolu budur). İzin: subscription.manage (Owner). 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [purpose] 
  /// * [creditPackageId] - purpose = Credits ise zorunlu; seçilen kontör paketi
  /// * [senderName] - Havaleyi gönderen ad (dekonttaki ad); verilmezse boş
  /// * [receipt] - Dekont (isteğe bağlı): PDF, JPG veya PNG, en fazla 5 MB
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PaymentNoticeSummary] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PaymentNoticeSummary>> manageNotifyTransfer({ 
    required String idempotencyKey,
    required PaymentNoticePurpose purpose,
    String? creditPackageId,
    String? senderName,
    MultipartFile? receipt,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/subscription/payment-notices';
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
      contentType: 'multipart/form-data',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = FormData.fromMap(<String, dynamic>{
        r'purpose': purpose,
        if (creditPackageId != null) r'creditPackageId': creditPackageId,
        if (senderName != null) r'senderName': senderName,
        if (receipt != null) r'receipt': receipt,
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

    PaymentNoticeSummary? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PaymentNoticeSummary, PaymentNoticeSummary>(rawData, 'PaymentNoticeSummary', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PaymentNoticeSummary>(
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

  /// Kontör satın al (havale bilgisi)
  /// v1&#39;de kontör yalnız havaleyle alınır (K1; kartla kontör yok). Salon paketi seçer, yanıt havale bilgisini döner: alıcı, IBAN, markanın sabit KNT- açıklama kodu ve paket tutarı. Bu uç kayıt oluşturmaz; salon havaleyi yapınca manageNotifyTransfer (purpose &#x3D; Credits, creditPackageId) ile bildirir, kontör eşleştirmeden sonra eklenir. Paket etkin değilse veya yoksa 404 not_found. OMVILA hesabı yapılandırılmamışsa omvilaBankAccount, fiyat tanımlı değilse amount null döner. Salt okunur abonelikte de çağrılabilir. İzin: subscription.manage (Owner). 
  ///
  /// Parameters:
  /// * [creditPurchaseRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CreditPurchaseInstructions] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CreditPurchaseInstructions>> managePurchaseCredits({ 
    required CreditPurchaseRequest creditPurchaseRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/credits/purchase';
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
      _bodyData = jsonEncode(creditPurchaseRequest);

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

    CreditPurchaseInstructions? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CreditPurchaseInstructions, CreditPurchaseInstructions>(rawData, 'CreditPurchaseInstructions', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CreditPurchaseInstructions>(
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
