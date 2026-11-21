//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/customer_create.dart';
import 'package:omvila_api/src/model/customer_deletion_preview.dart';
import 'package:omvila_api/src/model/customer_deletion_request.dart';
import 'package:omvila_api/src/model/customer_import_commit_request.dart';
import 'package:omvila_api/src/model/customer_import_preview.dart';
import 'package:omvila_api/src/model/customer_import_result.dart';
import 'package:omvila_api/src/model/customer_merge_request.dart';
import 'package:omvila_api/src/model/customer_package.dart';
import 'package:omvila_api/src/model/customer_package_page.dart';
import 'package:omvila_api/src/model/customer_payment_page.dart';
import 'package:omvila_api/src/model/customer_review_page.dart';
import 'package:omvila_api/src/model/customer_summary.dart';
import 'package:omvila_api/src/model/data_deletion_request_page.dart';
import 'package:omvila_api/src/model/data_deletion_request_view.dart';
import 'package:omvila_api/src/model/manage_list_customer_appointments200_response.dart';
import 'package:omvila_api/src/model/manage_list_customers200_response.dart';
import 'package:omvila_api/src/model/package_sale_request.dart';
import 'package:omvila_api/src/model/problem.dart';

class ManageCustomersApi {

  final Dio _dio;

  const ManageCustomersApi(this._dio);

  /// İçe aktarmayı uygula (\&quot;1.242 kaydı içe aktar\&quot;)
  /// Önizlemedeki geçerli satırları uygular: yeni müşteriler Source &#x3D; Import ile eklenir, kayıtlı numaralar birleşir, hatalı satırlar atlanır. sendInfoSms açıksa içe aktarılanların sonraki randevu bilgilendirme SMS&#39;leri açık olur (kampanya izni verilmez). Kendiliğinden idempotenttir: uygulanmış önizleme tekrar gönderilirse aynı sonuç 200 ile döner. Önizlemenin süresi dolmuşsa 409 hold_expired. İzin: customer.import (Owner). 
  ///
  /// Parameters:
  /// * [importId] 
  /// * [customerImportCommitRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerImportResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerImportResult>> manageCommitImport({ 
    required String importId,
    CustomerImportCommitRequest? customerImportCommitRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/imports/{importId}/commit'.replaceAll('{' r'importId' '}', importId.toString());
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
      _bodyData = jsonEncode(customerImportCommitRequest);

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

    CustomerImportResult? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerImportResult, CustomerImportResult>(rawData, 'CustomerImportResult', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerImportResult>(
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

  /// Müşteri ekle
  /// Numara markada kayıtlıysa 409 phone_already_exists + meta.brandCustomerId (\&quot;Mevcut kartı aç\&quot;).
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [customerCreate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerSummary] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerSummary>> manageCreateCustomer({ 
    required String idempotencyKey,
    required CustomerCreate customerCreate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers';
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
      _bodyData = jsonEncode(customerCreate);

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

    CustomerSummary? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerSummary, CustomerSummary>(rawData, 'CustomerSummary', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerSummary>(
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

  /// Müşterileri CSV olarak dışa aktar (\&quot;Dışa aktar\&quot;)
  /// Musteriler ekranı \&quot;Dışa aktar\&quot;. Markanın müşterileri (anonimleştirilmiş ve birleşmiş kayıtlar hariç), filter verilirse yalnız o filtredekiler. Sütunlar: Ad, Telefon, Doğum yılı, İlk kanal, İlk ziyaret, Son ziyaret, Ziyaret, Gelmedi, Toplam harcama (TL), Etiketler, Not. Her dışa aktarma denetim kaydına yazılır. \&quot;Tüm verileri dışa aktar\&quot; (AyarlarKvkk, e-postayla) Dilim 6&#39;dadır. İzin: customer.export (Owner). 
  ///
  /// Parameters:
  /// * [filter] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [String] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<String>> manageExportCustomers({ 
    String? filter,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/export';
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
      if (filter != null) r'filter': filter,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    String? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<String, String>(rawData, 'String', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<String>(
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

  /// Müşteri kartı (sekmeler ayrı uçlarda; customers/{brandCustomerId}/appointments, packages, payments, reviews)
  /// 
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerSummary] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerSummary>> manageGetCustomer({ 
    required String brandCustomerId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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

    CustomerSummary? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerSummary, CustomerSummary>(rawData, 'CustomerSummary', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerSummary>(
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

  /// KVKK silme talebi önizlemesi
  /// MusteriSil penceresindeki \&quot;Korunacak (anonim)\&quot; ve \&quot;Aktif paket\&quot; bilgileri. İzin: customer.kvkk (Owner)
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerDeletionPreview] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerDeletionPreview>> manageGetCustomerDeletionPreview({ 
    required String brandCustomerId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/deletion'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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

    CustomerDeletionPreview? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerDeletionPreview, CustomerDeletionPreview>(rawData, 'CustomerDeletionPreview', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerDeletionPreview>(
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

  /// Müşteri kartı · Randevular sekmesi
  /// MusteriKarti ekranı. Yeniden eskiye; tutar randevuya kopyalanan kalem fiyatlarının toplamıdır. Personel rolünde yalnız kendi randevuları döner. İzin: customer.view
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [ManageListCustomerAppointments200Response] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ManageListCustomerAppointments200Response>> manageListCustomerAppointments({ 
    required String brandCustomerId,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/appointments'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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

    ManageListCustomerAppointments200Response? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ManageListCustomerAppointments200Response, ManageListCustomerAppointments200Response>(rawData, 'ManageListCustomerAppointments200Response', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ManageListCustomerAppointments200Response>(
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

  /// Müşteri kartı · Paketler sekmesi
  /// MusteriKarti \&quot;Paketler\&quot; sekmesi ve üstteki \&quot;Fön paketi 2/5\&quot; kartı. Bütün durumlar (Active önce, sonra yeniden eskiye); usages alanı seans kullanımlarını gösterir (iptal edilen randevunun seansı revertedUtc ile \&quot;geri döndü\&quot;). İzin: customer.view 
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerPackagePage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerPackagePage>> manageListCustomerPackages({ 
    required String brandCustomerId,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/packages'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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

    CustomerPackagePage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerPackagePage, CustomerPackagePage>(rawData, 'CustomerPackagePage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerPackagePage>(
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

  /// Müşteri kartı · Ödemeler sekmesi
  /// MusteriKarti \&quot;Ödemeler\&quot; sekmesi: randevu tahsilatları, kaporalar ve paket satışları, yeniden eskiye. İzin: payment.collect (personel 403)
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerPaymentPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerPaymentPage>> manageListCustomerPayments({ 
    required String brandCustomerId,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/payments'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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

    CustomerPaymentPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerPaymentPage, CustomerPaymentPage>(rawData, 'CustomerPaymentPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerPaymentPage>(
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

  /// Müşteri kartı · Değerlendirmeler sekmesi
  /// MusteriKarti \&quot;Değerlendirmeler\&quot; sekmesi, yeniden eskiye. İzin: review.view (personel 403)
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerReviewPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerReviewPage>> manageListCustomerReviews({ 
    required String brandCustomerId,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/reviews'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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

    CustomerReviewPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerReviewPage, CustomerReviewPage>(rawData, 'CustomerReviewPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerReviewPage>(
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

  /// Müşteri ara ve listele
  /// q ad veya telefonla arar; telefonun son 4 hanesiyle de çalışır. Personel rolünde telefon maskeli döner.
  ///
  /// Parameters:
  /// * [q] 
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
  /// Returns a [Future] containing a [Response] with a [ManageListCustomers200Response] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ManageListCustomers200Response>> manageListCustomers({ 
    String? q,
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
    final _path = r'/api/v1/manage/customers';
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
      if (q != null) r'q': q,
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

    ManageListCustomers200Response? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ManageListCustomers200Response, ManageListCustomers200Response>(rawData, 'ManageListCustomers200Response', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ManageListCustomers200Response>(
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

  /// KVKK silme talepleri (\&quot;Silme talepleri\&quot;)
  /// AyarlarKvkk \&quot;Silme talepleri\&quot; tablosu (\&quot;30 gün içinde yanıtlanmalı\&quot;): müşteri uygulamasından gelen ve panelden yapılan talepler, yeniden eskiye. Bekleyen talep MusteriSil ile işlenir. Aydınlatma metni, saklama süresi ve \&quot;Tüm verileri dışa aktar\&quot; Dilim 6&#39;dadır. İzin: customer.kvkk (Owner). 
  ///
  /// Parameters:
  /// * [status] 
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [DataDeletionRequestPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<DataDeletionRequestPage>> manageListDeletionRequests({ 
    String? status,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customer-deletion-requests';
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
      if (status != null) r'status': status,
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

    DataDeletionRequestPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<DataDeletionRequestPage, DataDeletionRequestPage>(rawData, 'DataDeletionRequestPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<DataDeletionRequestPage>(
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

  /// Kayıtları birleştir
  /// MusteriBirlestir penceresi. Yoldaki müşteri \&quot;Kalacak kayıt\&quot;, sourceBrandCustomerId \&quot;Birleşecek kayıt\&quot;tır. Randevular, paketler, ödemeler, değerlendirmeler ve bekleme kayıtları hedefe taşınır; sayaçlar yeniden hesaplanır; mergeNotes açıksa notlar tarih sırasıyla eklenir. Kaynak kayıt arşivlenir (BrandCustomer.MergedIntoId); farklı telefonlu iki global Customer birleştirilmez, yalnız markadaki görünüm birleşir. Aynı kayıt verilirse 400 validation_failed; kaynak zaten birleştirilmiş veya anonimleştirilmişse 409 invalid_state_transition. Yanıt hedef müşterinin güncel kartıdır. İzin: customer.merge (Owner, BranchManager). 
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [customerMergeRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerSummary] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerSummary>> manageMergeCustomers({ 
    required String brandCustomerId,
    required CustomerMergeRequest customerMergeRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/merge'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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
      _bodyData = jsonEncode(customerMergeRequest);

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

    CustomerSummary? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerSummary, CustomerSummary>(rawData, 'CustomerSummary', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerSummary>(
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

  /// CSV içe aktarma önizlemesi
  /// MusteriIceAktar penceresi. CSV yüklenir, satırlar ayrıştırılır, hiçbir kayıt değişmez. Sütunlar başlıktan eşlenir (Ad, Telefon, Not, Doğum yılı, E-posta). Telefonlar E.164&#39;e çevrilir (\&quot;0532 123 45 67\&quot;, \&quot;5321234567\&quot; ve \&quot;+90 532 123 4567\&quot; aynı kişidir); markada kayıtlı numara \&quot;Birleşecek\&quot; sayılır (mevcut notlar korunur). Kampanya izni içe aktarılmaz. Boyut veya satır sınırı aşılırsa ya da dosya CSV değilse 400 validation_failed (errors.file &#x3D; [too_large] / [invalid_format]); sınırlar yapılandırmadadır. Önizleme kaydı (CustomerImport, Previewed) oluşturduğu için Idempotency-Key ister. İzin: customer.import (Owner). 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [file] - UTF-8 CSV (virgül veya noktalı virgül ayraçlı), ilk satır başlık
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerImportPreview] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerImportPreview>> managePreviewImport({ 
    required String idempotencyKey,
    required MultipartFile file,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/imports';
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
        r'file': file,
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

    CustomerImportPreview? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerImportPreview, CustomerImportPreview>(rawData, 'CustomerImportPreview', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerImportPreview>(
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

  /// Kalıcı olarak anonimleştir (KVKK silme talebi)
  /// MusteriSil \&quot;Kalıcı olarak anonimleştir\&quot;. Geri alınamaz. Bu markadaki müşteri kaydı hemen anonimleştirilir: ad, telefon, e-posta ve notlar maskelenir veya silinir; randevu, ödeme ve prim kayıtları anonim olarak kalır; yaklaşan randevular iptal edilir (neden AccountDeleted, müşteriye bildirim gitmez); bekleme kayıtları kapanır. Global Customer kaydı ve başka markalardaki kayıtlar etkilenmez. DataDeletionRequest(RequestedVia &#x3D; Panel, Completed) yazılır; bu müşterinin bekleyen talebi varsa o da Completed olur. confirmFullName müşterinin adıyla eşleşmezse 400 validation_failed (errors.confirmFullName &#x3D; [mismatch]). Zaten anonimleştirilmişse 409 invalid_state_transition. İzin: customer.kvkk (Owner). 
  ///
  /// Parameters:
  /// * [brandCustomerId] 
  /// * [customerDeletionRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [DataDeletionRequestView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<DataDeletionRequestView>> manageRequestCustomerDeletion({ 
    required String brandCustomerId,
    required CustomerDeletionRequest customerDeletionRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/deletion'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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
      _bodyData = jsonEncode(customerDeletionRequest);

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

    DataDeletionRequestView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<DataDeletionRequestView, DataDeletionRequestView>(rawData, 'DataDeletionRequestView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<DataDeletionRequestView>(
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

  /// Müşteriye paket sat (\&quot;1.700 ₺ al ve paketi tanımla\&quot;)
  /// PaketSat penceresi. Satışta paketin adı, içeriği ve fiyatı kopyalanır; expiresOn &#x3D; satış günü + validityDays (şubenin yerel takvimi). Yan etkiler: CustomerPackage(Active) + kalemleri; Payment(method &#x3D; paymentMethod, tutar &#x3D; paket fiyatı) ve CustomerPackage tahsisi (Kasa&#39;da \&quot;Fön paketi · 5 seans\&quot;); satan personele CommissionEntry(Sale, Accrued) (yüzde: paketin saleCommissionPercent&#39;i, yoksa personelin SaleCommissionPercent&#39;i). Paket satışı tahsilattır; seanslar kullanıldıkça gelir sayılır. Paket satışta değilse (isActive &#x3D; false) 409 invalid_state_transition. Satan personel satış şubesinde çalışmıyorsa 400 validation_failed (errors.soldByStaffId). isAvailableAllBranches &#x3D; false olan paket yalnız satıldığı şubede kullanılır. Para kaydettiği için Idempotency-Key ister. İzin: package.sell (Owner, BranchManager kendi şubesinde). 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [brandCustomerId] 
  /// * [packageSaleRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerPackage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerPackage>> manageSellPackage({ 
    required String idempotencyKey,
    required String brandCustomerId,
    required PackageSaleRequest packageSaleRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customers/{brandCustomerId}/packages'.replaceAll('{' r'brandCustomerId' '}', brandCustomerId.toString());
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
      _bodyData = jsonEncode(packageSaleRequest);

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

    CustomerPackage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CustomerPackage, CustomerPackage>(rawData, 'CustomerPackage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CustomerPackage>(
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
