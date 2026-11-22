//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/me_package_list.dart';
import 'package:omvila_api/src/model/me_payment_page.dart';
import 'package:omvila_api/src/model/problem.dart';

class MeWalletApi {

  final Dio _dio;

  const MeWalletApi(this._dio);

  /// Paketlerim
  /// Bütün markalardaki paketler. Önce kullanılabilir paketler (Active ve kalan seansı olan; son kullanım gününe göre artan), sonra \&quot;Biten paketler\&quot; (seansı biten, süresi geçen, iade edilen; bitişe göre azalan). Başlıktaki \&quot;2 aktif paket\&quot; activeCount&#39;tur. Boşsa PaketlerBos. Paketler salondan satın alınır; uygulamada satın alma yoktur. 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MePackageList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MePackageList>> meListPackages({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/packages';
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

    MePackageList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MePackageList, MePackageList>(rawData, 'MePackageList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MePackageList>(
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

  /// Ödemelerim (salt okunur)
  /// \&quot;Uygulamadan ödeme alınmaz.\&quot; Kapora havaleleri ve salonda kaydedilen tahsilatlar, yeniden eskiye (ay başlıkları istemcide). Satırlar: kind &#x3D; Deposit (kapora havalesi), AppointmentPayment (salonda alınan randevu ödemesi), PackagePurchase (paket satın alma). Kapora durumu (Dilim 4 KK9): status AwaitingConfirmation &#x3D; müşteri \&quot;Havaleyi yaptım\&quot; dedi, salon henüz \&quot;Kapora geldi\&quot; demedi (\&quot;Salon onayı bekleniyor\&quot;; satır tahsilat kaydı değil, randevudan gelir, id randevunun kimliğidir); Recorded &#x3D; \&quot;Alındı · 26 Kasım randevusu\&quot;; Refunded veya PartiallyRefunded &#x3D; \&quot;Salon iade etti\&quot;. Bildirilmemiş (Awaiting) kapora, iptal edilmiş (Voided) kayıt ve paketten düşülen kalemlerin 0 TL kaydı listelenmez. Yöntem metni istemcide: Cash \&quot;Nakit\&quot;, Card \&quot;Kartla, salonda\&quot; (salonun kendi POS&#39;u), Transfer \&quot;Havale\&quot;. Boşsa OdemelerimBos. 
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
  /// Returns a [Future] containing a [Response] with a [MePaymentPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MePaymentPage>> meListPayments({ 
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/payments';
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

    MePaymentPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MePaymentPage, MePaymentPage>(rawData, 'MePaymentPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MePaymentPage>(
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
