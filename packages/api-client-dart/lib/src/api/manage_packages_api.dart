//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/customer_package.dart';
import 'package:omvila_api/src/model/customer_package_page.dart';
import 'package:omvila_api/src/model/package_definition.dart';
import 'package:omvila_api/src/model/package_list.dart';
import 'package:omvila_api/src/model/package_refund_request.dart';
import 'package:omvila_api/src/model/package_write.dart';
import 'package:omvila_api/src/model/problem.dart';

class ManagePackagesApi {

  final Dio _dio;

  const ManagePackagesApi(this._dio);

  /// Paket oluştur (\&quot;Paketi oluştur\&quot;)
  /// PaketOlustur penceresi. Hizmetler markanın aktif hizmetleri olmalı ve tekrar etmemeli (400 validation_failed, errors.items). İzin: package.manage (Owner). 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [packageWrite] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PackageDefinition] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PackageDefinition>> manageCreatePackage({ 
    required String idempotencyKey,
    required PackageWrite packageWrite,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/packages';
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
      _bodyData = jsonEncode(packageWrite);

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

    PackageDefinition? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PackageDefinition, PackageDefinition>(rawData, 'PackageDefinition', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PackageDefinition>(
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

  /// Paket tanımı
  /// Düzenleme penceresi için; ETag döner. İzin: package.manage veya package.sell
  ///
  /// Parameters:
  /// * [packageId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PackageDefinition] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PackageDefinition>> manageGetPackage({ 
    required String packageId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/packages/{packageId}'.replaceAll('{' r'packageId' '}', packageId.toString());
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

    PackageDefinition? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PackageDefinition, PackageDefinition>(rawData, 'PackageDefinition', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PackageDefinition>(
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

  /// Müşteri paketleri (\&quot;Müşteri paketleri\&quot; sekmesi, \&quot;Süresi yaklaşanlar\&quot;)
  /// Paketler ekranı \&quot;Müşteri paketleri\&quot; sekmesi ve \&quot;Süresi yaklaşanlar\&quot; kartı. filter: Active (varsayılan), ExpiringSoon (30 gün içinde biten Active paketler, en yakın önce), Expired, All. branchId verilirse o şubede satılan ve bütün şubelerde geçerli paketler. İzin: package.manage veya package.sell. 
  ///
  /// Parameters:
  /// * [branchId] 
  /// * [filter] 
  /// * [q] 
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
  Future<Response<CustomerPackagePage>> manageListAllCustomerPackages({ 
    String? branchId,
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
    final _path = r'/api/v1/manage/customer-packages';
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

  /// Paket tanımları ve üst kartlar (Paketler)
  /// Paketler ve PaketlerBos ekranları: paket tanımları (\&quot;Paket tanımları\&quot; sekmesi, satışta olanlar önce) ve üst kartlar (overview). branchId verilirse sayılar ve kartlar o şubeye göredir. PaketSat&#39;taki paket listesi için isActive &#x3D; true olanlar kullanılır. İzin: package.manage veya package.sell. 
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
  /// Returns a [Future] containing a [Response] with a [PackageList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PackageList>> manageListPackages({ 
    String? branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/packages';
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
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    PackageList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PackageList, PackageList>(rawData, 'PackageList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PackageList>(
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

  /// Paketi iade et
  /// Salon paketi iade ettiğini işaretler; parayı kendisi öder, OMVILA yalnız kaydeder. Tutarı salon elle girer (S-10); ödenen tutarı aşamaz (aşarsa veya 0 ise 400 validation_failed, errors.amount &#x3D; [out_of_range]). Yan etkiler: CustomerPackage Refunded (kalan seanslar kullanılamaz, ayrılmış seanslı yaklaşan randevular değişmez); satış ödemesi PartiallyRefunded veya Refunded (iade, satışın yöntemiyle kasaya o gün yazılır); satış primi için iade oranında eksi tutarlı ters kayıt. Paket Active veya Expired değilse 409 invalid_state_transition. Para kaydettiği için If-Match ve Idempotency-Key ister. İzin: package.sell (Owner, BranchManager kendi şubesinde). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [customerPackageId] 
  /// * [packageRefundRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CustomerPackage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CustomerPackage>> manageRefundCustomerPackage({ 
    required String ifMatch,
    required String idempotencyKey,
    required String customerPackageId,
    required PackageRefundRequest packageRefundRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/customer-packages/{customerPackageId}/refund'.replaceAll('{' r'customerPackageId' '}', customerPackageId.toString());
    final _options = Options(
      method: r'POST',
      headers: <String, dynamic>{
        r'If-Match': ifMatch,
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
      _bodyData = jsonEncode(packageRefundRequest);

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

  /// Paket tanımını düzenle
  /// Satır menüsü \&quot;Düzenle\&quot; ve \&quot;Satıştan kaldır\&quot; (isActive &#x3D; false). Değişiklik yalnız sonraki satışları etkiler; satılmış paketlerin içeriği, fiyatı ve son kullanım tarihi değişmez. Paketler silinmez. İzin: package.manage (Owner). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [packageId] 
  /// * [packageWrite] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [PackageDefinition] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PackageDefinition>> manageUpdatePackage({ 
    required String ifMatch,
    required String packageId,
    required PackageWrite packageWrite,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/packages/{packageId}'.replaceAll('{' r'packageId' '}', packageId.toString());
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
      _bodyData = jsonEncode(packageWrite);

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

    PackageDefinition? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PackageDefinition, PackageDefinition>(rawData, 'PackageDefinition', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PackageDefinition>(
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
