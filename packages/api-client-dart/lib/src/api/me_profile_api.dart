//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/me_account_deletion_confirm.dart';
import 'package:omvila_api/src/model/me_account_deletion_result.dart';
import 'package:omvila_api/src/model/me_consent_list.dart';
import 'package:omvila_api/src/model/me_marketing_consent_update.dart';
import 'package:omvila_api/src/model/me_profile.dart';
import 'package:omvila_api/src/model/me_profile_update.dart';
import 'package:omvila_api/src/model/otp_challenge_response.dart';
import 'package:omvila_api/src/model/problem.dart';

class MeProfileApi {

  final Dio _dio;

  const MeProfileApi(this._dio);

  /// Profil (\&quot;Profil\&quot;, \&quot;Profil bilgilerim\&quot;)
  /// Global Customer kaydının müşteriye görünen alanları. Markalar bu kaydın tamamını görmez (ürün kuralı 5). Telefon giriş anahtarıdır, salt okunurdur (\&quot;Değiştirmek için destek\&quot;). fullName, müşteri henüz ad vermediyse (ilk girişte, randevusuz) null&#39;dır; uygulama selamlamayı adsız gösterir ve randevu alırken adı ister. Profilde \&quot;Salonlarım\&quot; meListBrands&#39;ten gelir. 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeProfile] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeProfile>> meGetProfile({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/profile';
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

    MeProfile? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeProfile, MeProfile>(rawData, 'MeProfile', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeProfile>(
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

  /// İzinlerim (salon bazında kampanya izni)
  /// \&quot;İzinlerim (KVKK)\&quot; ekranı. Randevu mesajları işlem mesajıdır, rızaya bağlı değildir ve kapatılamaz (ekrandaki açıklama metni; listede satırı yoktur). Liste, müşterinin müşteri kaydı (BrandCustomer) olan markalardır; yalnız kaydedilen (randevu alınmamış) salon listede yoktur. marketingGranted, o markadaki son Marketing* rıza kaydıdır (ConsentRecord; kayıt yoksa false). İYS&#39;ye bildirim v1&#39;de yapılmaz, rıza yalnız toplanır (T-09). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeConsentList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeConsentList>> meListConsents({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/consents';
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

    MeConsentList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeConsentList, MeConsentList>(rawData, 'MeConsentList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeConsentList>(
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

  /// Hesabı ve verileri sil (SMS koduyla onay, 2. adım)
  /// Kod doğruysa silme hemen uygulanır (KK9, docs/acik-sorular.md W7): yaklaşan randevular iptal edilir (CancellationReason AccountDeleted; salonlara staff.appointment_changed bildirimi gider, seanslar geri döner), global kayıt ve bütün markalardaki müşteri kayıtları anonimleşir (ad, telefon, e-posta, notlar), rıza kayıtları ve cihazlar pasifleşir, bütün oturumlar kapanır. Randevu, ödeme ve prim kayıtları salonlarda anonim olarak kalır (\&quot;Salonlarda kalanlar\&quot;); kalan paket seansları kullanılamaz hâle gelir. DataDeletionRequest (BrandId boş, RequestedVia CustomerApp, Completed) yazılır. Yanıttan sonra jetonlar geçersizdir; uygulama HesapSilindi ekranını gösterir. Kod hatalıysa 422 otp_invalid, süresi dolmuşsa 422 otp_expired. 
  ///
  /// Parameters:
  /// * [meAccountDeletionConfirm] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeAccountDeletionResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeAccountDeletionResult>> meRequestAccountDeletion({ 
    required MeAccountDeletionConfirm meAccountDeletionConfirm,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/account/deletion';
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
      _bodyData = jsonEncode(meAccountDeletionConfirm);

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

    MeAccountDeletionResult? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeAccountDeletionResult, MeAccountDeletionResult>(rawData, 'MeAccountDeletionResult', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeAccountDeletionResult>(
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

  /// Hesap silme için SMS kodu iste (\&quot;SMS koduyla onayla ve sil\&quot;, 1. adım)
  /// HesapSil ekranı. Oturumdaki müşterinin kendi numarasına kod gönderir (OtpChallenge.Purpose AccountDeletion, OMVILA adıyla SMS). Ekrandaki önizleme (\&quot;2 yaklaşan randevunuz iptal edilecek\&quot;, \&quot;Kalan paket seansları\&quot;) ayrı uçtan gelmez: uygulama meListAppointments(scope &#x3D; Upcoming) ve meListPackages yanıtlarından üretir. Sınırlar giriş koduyla aynıdır (429). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [OtpChallengeResponse] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<OtpChallengeResponse>> meRequestAccountDeletionOtp({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/account/deletion/otp';
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
      validateStatus: validateStatus,
    );

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    OtpChallengeResponse? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<OtpChallengeResponse, OtpChallengeResponse>(rawData, 'OtpChallengeResponse', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<OtpChallengeResponse>(
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

  /// Bir salonun kampanya iznini aç veya kapat
  /// Anahtar tek bir \&quot;Kampanya ve indirim mesajları\&quot; iznidir: sunucu o marka için MarketingSms, MarketingEmail ve MarketingPush rıza kayıtlarını (ConsentRecord, Channel CustomerApp) aynı değerle ekler. Rıza defteri yalnız eklenir, güncellenmez; değer zaten aynıysa yeni satır yazılmaz. Bu yüzden istek kendiliğinden idempotenttir ve If-Match almaz (son yazılan geçerlidir). Müşteri kaydı olmayan marka 404 not_found. Yanıt güncel listedir. 
  ///
  /// Parameters:
  /// * [meMarketingConsentUpdate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeConsentList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeConsentList>> meUpdateMarketingConsent({ 
    required MeMarketingConsentUpdate meMarketingConsentUpdate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/consents';
    final _options = Options(
      method: r'PUT',
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
      _bodyData = jsonEncode(meMarketingConsentUpdate);

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

    MeConsentList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeConsentList, MeConsentList>(rawData, 'MeConsentList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeConsentList>(
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

  /// Profil bilgilerini kaydet (\&quot;Kaydet\&quot;)
  /// Gönderilmeyen alan değişmez; email için açıkça null gönderilirse e-posta silinir. Telefon bu uçla değişmez. Dil v1&#39;de yalnız tr&#39;dir (uygulama arayüzü ve bildirim metinleri Türkçe; English seçeneği docs/acik-sorular.md T-101&#39;de açık). Biçim hatası 400 validation_failed (fullName too_short / too_long, email invalid_format). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [meProfileUpdate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeProfile] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeProfile>> meUpdateProfile({ 
    required String ifMatch,
    required MeProfileUpdate meProfileUpdate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/profile';
    final _options = Options(
      method: r'PATCH',
      headers: <String, dynamic>{
        r'If-Match': ifMatch,
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
      _bodyData = jsonEncode(meProfileUpdate);

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

    MeProfile? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeProfile, MeProfile>(rawData, 'MeProfile', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeProfile>(
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
