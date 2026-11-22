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

import 'package:omvila_api/src/model/admin_credit_package_catalog.dart';
import 'package:omvila_api/src/model/admin_credit_package_catalog_update.dart';
import 'package:omvila_api/src/model/admin_plan_catalog.dart';
import 'package:omvila_api/src/model/admin_plan_catalog_update.dart';
import 'package:omvila_api/src/model/problem.dart';

class AdminPlansApi {

  final Dio _dio;

  const AdminPlansApi(this._dio);

  /// Kontör paketleri
  /// PPlanlar \&quot;Kontör paketleri\&quot; (Paket, SMS, Fiyat). Fiyat tanımlı değilse price null (S-14). İzin: platform.plans.view (bütün platform rolleri). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminCreditPackageCatalog] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminCreditPackageCatalog>> adminListCreditPackages({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/credit-packages';
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

    AdminCreditPackageCatalog? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminCreditPackageCatalog, AdminCreditPackageCatalog>(rawData, 'AdminCreditPackageCatalog', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminCreditPackageCatalog>(
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

  /// Planlar ve fiyat
  /// PPlanlar: plan kartları (Tekil, Salon, Salon Plus; \&quot;96 marka\&quot;), \&quot;Ortak kurallar\&quot; (Deneme süresi, Ödeme gecikmesinde, Deneme hediyesi, Kanallar). Fiyat ve sınırlar S-14 çözülene kadar boş olabilir: boş fiyat \&quot;belirlenmedi\&quot;, boş sınır \&quot;sınırsız\&quot; demektir; istemci köşeli parantezli yer tutucu yazmaz. Ortak kurallar yapılandırmadan okunur ve bu uçtan değiştirilmez (trialCredits tanımlı değilse null). Özellik bayraklarının ekran metinleri istemcidedir. İzin: platform.plans.view (bütün platform rolleri; Destek görüntüler, \&quot;Planı düzenle\&quot; kilitli). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminPlanCatalog] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminPlanCatalog>> adminListPlans({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/plans';
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

    AdminPlanCatalog? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminPlanCatalog, AdminPlanCatalog>(rawData, 'AdminPlanCatalog', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminPlanCatalog>(
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

  /// Kontör paketlerini kaydet
  /// Paketlerin ad, kontör, fiyat ve etkinlik değerleri; code ile eşleşir, olmayan code 400. Paket eklenmez ve silinmez; isActive &#x3D; false paketi satıştan kaldırır. Bekleyen havale bildirimlerinin beklenen tutarı değişmez. Denetime yazılır (Update, CreditPackage). İzin: platform.plans.manage (PlatformAdmin, PlatformFinance); Destek 403. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [adminCreditPackageCatalogUpdate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminCreditPackageCatalog] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminCreditPackageCatalog>> adminUpdateCreditPackages({ 
    required String ifMatch,
    required AdminCreditPackageCatalogUpdate adminCreditPackageCatalogUpdate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/credit-packages';
    final _options = Options(
      method: r'PUT',
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
      _bodyData = jsonEncode(adminCreditPackageCatalogUpdate);

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

    AdminCreditPackageCatalog? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminCreditPackageCatalog, AdminCreditPackageCatalog>(rawData, 'AdminCreditPackageCatalog', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminCreditPackageCatalog>(
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

  /// Planları kaydet (\&quot;Planı düzenle\&quot;)
  /// Planların ad, açıklama, fiyat, sınır ve özellik değerleri. Gönderilen her plan code ile eşleşir; olmayan code 400 (errors.plans[i].code &#x3D; [invalid_format]). Plan eklenmez ve silinmez; isActive &#x3D; false yeni kayıtlarda planı gizler. Değişiklik yeni kayıtlar ve sonraki faturalar için geçerlidir, mevcut aboneler korunur (\&quot;Yeni kayıtlar için geçerli · mevcut aboneler korunur\&quot;). Denetime yazılır (Update, Plan). İzin: platform.plans.manage (PlatformAdmin, PlatformFinance); Destek 403. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [adminPlanCatalogUpdate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AdminPlanCatalog] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AdminPlanCatalog>> adminUpdatePlans({ 
    required String ifMatch,
    required AdminPlanCatalogUpdate adminPlanCatalogUpdate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/admin/plans';
    final _options = Options(
      method: r'PUT',
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
      _bodyData = jsonEncode(adminPlanCatalogUpdate);

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

    AdminPlanCatalog? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AdminPlanCatalog, AdminPlanCatalog>(rawData, 'AdminPlanCatalog', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AdminPlanCatalog>(
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
