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

import 'package:omvila_api/src/model/me_brand.dart';
import 'package:omvila_api/src/model/me_brand_list.dart';
import 'package:omvila_api/src/model/me_save_brand_request.dart';
import 'package:omvila_api/src/model/problem.dart';

class MeBrandsApi {

  final Dio _dio;

  const MeBrandsApi(this._dio);

  /// Salonlarım (randevu alınan + kaydedilen)
  /// YRSalon (\&quot;Hangi salon?\&quot;) ve Profil \&quot;Salonlarım\&quot;. Randevu alınan markalar (müşteri kaydı, BrandCustomer) ile QR veya bağlantıyla kaydedilenler (CustomerSavedBrand) birlikte; son ziyarete göre azalan, ziyareti olmayanlar kaydedilme anına göre sonda. Sayfalanmaz (kısa liste). Boşsa YRSalonBos / MainBos. activePackages \&quot;Fön paketiniz var\&quot; rozetini besler (Active ve kalan seansı olan paketler). Uygulamadan randevu akışı (YR*) public uçlarla, müşteri jetonuyla yapılır: publicGetBrand(slug) → publicGetBranchCatalog (YRHizmet, YRPersonel) → publicSearchDays / publicSearchSlots (YRSaat) → publicListMyPackages (\&quot;paketinizden düşülsün\&quot;) → publicCreateAppointment (YROnay; Authorization meBearer, X-Client customer-app, kod adımı yok) → kapora varsa meReportDepositTransfer (YRHavale). Bekleme listesi publicJoinWaitlist (meBearer). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeBrandList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeBrandList>> meListBrands({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/brands';
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
            'name': 'meBearer',
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

    MeBrandList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeBrandList, MeBrandList>(rawData, 'MeBrandList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeBrandList>(
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

  /// Salonu ekle (\&quot;QR kodu okut\&quot;, \&quot;Bağlantıyı yapıştır\&quot;)
  /// MainBos ve derin bağlantı (randevu.omvila.com.tr/{brandSlug}, K3). Uygulama QR veya yapıştırılan bağlantıdan brandSlug&#39;ı çıkarır. CustomerSavedBrand yazılır; BrandCustomer **oluşmaz** ve salon bu kişiyi müşteri listesinde görmez (KK1). Müşteri kaydı ilk randevuda açılır. Eski bağlantı adı yönlendirmesi sürüyorsa güncel markaya kaydedilir. Zaten kayıtlı veya randevu alınmış marka için aynı satır 200 ile döner (kendiliğinden idempotent; Idempotency-Key almaz). Bilinmeyen bağlantı 404 not_found (\&quot;Bu bağlantıda salon bulunamadı\&quot;). 
  ///
  /// Parameters:
  /// * [meSaveBrandRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeBrand] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeBrand>> meSaveBrand({ 
    required MeSaveBrandRequest meSaveBrandRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/brands';
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
            'name': 'meBearer',
          },
        ],
        ...?extra,
      },
      contentType: 'application/json',
      validateStatus: validateStatus,
    );

    dynamic _bodyData;

    try {
      _bodyData = jsonEncode(meSaveBrandRequest);

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

    MeBrand? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeBrand, MeBrand>(rawData, 'MeBrand', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeBrand>(
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
