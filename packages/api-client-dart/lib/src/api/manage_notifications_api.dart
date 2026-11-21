//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/device_registration.dart';
import 'package:omvila_api/src/model/in_app_notification_page.dart';
import 'package:omvila_api/src/model/mark_notifications_read_request.dart';
import 'package:omvila_api/src/model/my_notification_preferences.dart';
import 'package:omvila_api/src/model/notification_settings.dart';
import 'package:omvila_api/src/model/notification_settings_update.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/registered_device.dart';
import 'package:omvila_api/src/model/sms_overview.dart';
import 'package:omvila_api/src/model/sms_sender.dart';
import 'package:omvila_api/src/model/sms_sender_request.dart';
import 'package:omvila_api/src/model/unread_notification_count.dart';

class ManageNotificationsApi {

  final Dio _dio;

  const ManageNotificationsApi(this._dio);

  /// Kendi bildirim tercihlerim (işletme uygulaması \&quot;Bildirim ayarları\&quot;)
  /// \&quot;Yalnızca sizin telefonunuza gelenler.\&quot; Tercihler yalnız push&#39;u etkiler; bildirim paneli kaydı her durumda tutulur. Sahip ve yönetici olayları (owner.credits_low, owner.subscription_payment_failed, staff.deposit_reported) kapatılamaz. Sessiz saat aralığı platform varsayılanıdır (22:00–08:00, kullanıcının şubesinin saat diliminde); v1&#39;de yalnız açılıp kapatılır. İşletme push&#39;ları gönderilmeden önce alıcının bu tercihleriyle süzülür (anahtar → olay eşlemesi MyNotificationPreferences açıklamasında); kapalı anahtarın olayı push olarak gitmez ama bildirim panelinde (manageListInAppNotifications) görünür. Kaydı olmayan kullanıcı için varsayılanlar döner (gün sonu özeti kapalı, bildirimler.md; diğer anahtarlar ve sessiz saatler açık). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MyNotificationPreferences] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MyNotificationPreferences>> manageGetMyNotificationPreferences({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/me/notification-preferences';
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

    MyNotificationPreferences? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MyNotificationPreferences, MyNotificationPreferences>(rawData, 'MyNotificationPreferences', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MyNotificationPreferences>(
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

  /// Bildirim ayarları (hatırlatmalar, değerlendirme daveti, mesaj şablonları)
  /// Ayarlar → Bildirimler ve SMS (AyarlarBildirim) ekranının \&quot;Hatırlatmalar\&quot; ve \&quot;Mesaj şablonları\&quot; bölümleri. Hatırlatma zamanları ve değerlendirme daveti için kanonik uç budur. BrandSettings.reminderOffsetsMinutes ve BrandSettings.reviewInviteEnabled aynı saklanan değerlerin aynasıdır ve geriye uyumluluk için durur (deprecated); hangi uçtan yazılırsa yazılsın iki yanıt da aynı değeri gösterir ve iki kaynağın ETag&#39;i birlikte değişir. templates yalnız salonun ezebildiği SMS şablonlarını listeler; her satırda sistem metni, salon metni (varsa) ve kullanılabilir değişkenler bulunur (\&quot;Düzenle\&quot;). Satır adları (\&quot;Randevu onaylandı\&quot;, \&quot;Hatırlatma\&quot;) istemcide eventCode + variant&#39;tan üretilir. İzin: settings.manage. 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [NotificationSettings] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<NotificationSettings>> manageGetNotificationSettings({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/brand/notification-settings';
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

    NotificationSettings? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<NotificationSettings, NotificationSettings>(rawData, 'NotificationSettings', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<NotificationSettings>(
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

  /// SMS özeti (kontör, bu ay gönderilen, gönderen adı)
  /// AyarlarBildirim üst kartları: \&quot;SMS kontörü 1.240 · bu tempoyla ~18 gün yeter\&quot;, \&quot;Bu ay gönderilen 862 SMS · 1.904 bildirim\&quot;, \&quot;Gönderen adı KUAFORELIF · operatör onaylı\&quot; ve alttaki \&quot;Bakiye 200&#39;ün altına düşünce size haber veririz\&quot; uyarısı. Kontör markaya aittir (şube kırılımı yok). \&quot;Bu ay\&quot; markanın ana şubesinin saat dilimindeki takvim ayıdır. İzin: settings.manage. 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [SmsOverview] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<SmsOverview>> manageGetSmsOverview({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/brand/sms';
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

    SmsOverview? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<SmsOverview, SmsOverview>(rawData, 'SmsOverview', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<SmsOverview>(
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

  /// Bildirim paneli (zil)
  /// Üst bardaki zil ve BildirimPaneli. Oturumdaki kullanıcıya düşen işletme olayları (staff.*, owner.*), yeniden eskiye. Zildeki sayı unreadCount&#39;tur (sahte sayı gösterilmez; 0 ise rozet yok). Panel kaydı push tercihlerinden bağımsız tutulur: kullanıcı push&#39;u kapatsa da olay panelde görünür. title ve body olay anındaki push metniyle aynıdır (docs/standartlar/bildirimler.md), sunucuda üretilir ve müşteri adını \&quot;Ad Soyad\&quot; biçiminde taşır (kilit ekranı kısaltması yalnız push&#39;ta). Not içeriği yazılmaz. target, tıklanınca açılacak ekranı söyler. 
  ///
  /// Parameters:
  /// * [limit] 
  /// * [cursor] 
  /// * [unreadOnly] - Yalnız okunmamışlar
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [InAppNotificationPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<InAppNotificationPage>> manageListInAppNotifications({ 
    int? limit = 50,
    String? cursor,
    bool? unreadOnly = false,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/notifications';
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
      if (unreadOnly != null) r'unreadOnly': unreadOnly,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    InAppNotificationPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<InAppNotificationPage, InAppNotificationPage>(rawData, 'InAppNotificationPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<InAppNotificationPage>(
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

  /// Bildirimleri okundu say (\&quot;Tümünü okundu say\&quot;)
  /// Ya notificationIds (tıklanan bildirim) ya da all &#x3D; true (\&quot;Tümünü okundu say\&quot;) gönderilir; ikisi birden veya hiçbiri 400 validation_failed. all &#x3D; true, isteğin sunucuya ulaştığı ana kadar oluşmuş bildirimleri okundu sayar. İşlem idempotenttir: okunmuş bildirim yeniden okundu sayılmaz (readUtc değişmez), başka kullanıcının bildirimi sessizce yok sayılır. Yanıt güncel okunmamış sayısıdır. 
  ///
  /// Parameters:
  /// * [markNotificationsReadRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [UnreadNotificationCount] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<UnreadNotificationCount>> manageMarkNotificationsRead({ 
    required MarkNotificationsReadRequest markNotificationsReadRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/notifications/read';
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
      _bodyData = jsonEncode(markNotificationsReadRequest);

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

    UnreadNotificationCount? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<UnreadNotificationCount, UnreadNotificationCount>(rawData, 'UnreadNotificationCount', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<UnreadNotificationCount>(
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

  /// İşletme uygulaması push jetonunu kaydet
  /// OMVILA İşletme uygulaması açılışta ve FCM jetonu yenilendiğinde çağırır. Doğal anahtar push jetonudur: aynı jeton tekrar gelirse kayıt güncellenir (sahip, platform, uygulama sürümü, lastSeenUtc) ve aynı deviceId döner. Bu yüzden Idempotency-Key istenmez; istek doğası gereği idempotenttir. Jeton başka bir kullanıcıya kayıtlıysa o kayıt pasifleşir ve jeton oturumdaki kullanıcıya geçer (aynı telefonda hesap değişimi). Sağlayıcı \&quot;geçersiz jeton\&quot; derse kayıt sunucuda pasifleşir. Kaydı silme ucu yol haritasında yoktur (açık soru): şimdilik kayıt geçersiz jeton geri bildirimiyle veya jeton başka kullanıcıya geçince pasifleşir; uygulama oturum kapatırken jetonu yerelde siler. 
  ///
  /// Parameters:
  /// * [deviceRegistration] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [RegisteredDevice] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<RegisteredDevice>> manageRegisterDevice({ 
    required DeviceRegistration deviceRegistration,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/devices';
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
      _bodyData = jsonEncode(deviceRegistration);

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

    RegisteredDevice? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<RegisteredDevice, RegisteredDevice>(rawData, 'RegisteredDevice', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<RegisteredDevice>(
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

  /// Gönderen adı başvurusu yap
  /// SMS&#39;lerin salon adıyla (originator) gitmesi için başvuru. Başvuru Pending olur; platform ekibi operatör onayını alınca Approved yapar (Dilim 6 platform paneli). Onaylanana kadar mesajlar OMVILA adıyla gider (effectiveName). Ad operatör kurallarına uyar: 3–11 karakter, yalnız büyük Latin harfi ve rakam, en az bir harf; Türkçe karakter, boşluk ve noktalama yok (istemci girişi büyük harfe çevirir, Ç→C, Ğ→G, İ→I, Ö→O, Ş→S, Ü→U). Biçim hatası 400 validation_failed (name: invalid_format). Bekleyen başvuru varken yeni başvuru 409 invalid_state_transition. Onaylı ad varken yapılan başvuru onaylanana kadar eski onaylı ad kullanılmaya devam eder. İzin: settings.manage. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [smsSenderRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [SmsSender] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<SmsSender>> manageRequestSmsSender({ 
    required String idempotencyKey,
    required SmsSenderRequest smsSenderRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/brand/sms-sender';
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
      _bodyData = jsonEncode(smsSenderRequest);

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

    SmsSender? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<SmsSender, SmsSender>(rawData, 'SmsSender', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<SmsSender>(
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

  /// Kendi bildirim tercihlerimi kaydet
  /// \&quot;Bildirim ayarları\&quot; (BildirimAyar) anahtarları. Anahtarların hepsi tam gönderilir (salt okunur saat alanları yok sayılır). Değişiklik sonraki push&#39;tan itibaren uygulanır; sessiz saatte ertelenmiş push&#39;lar gönderim anındaki tercihe göre süzülür. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [myNotificationPreferences] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MyNotificationPreferences] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MyNotificationPreferences>> manageUpdateMyNotificationPreferences({ 
    required String ifMatch,
    required MyNotificationPreferences myNotificationPreferences,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/me/notification-preferences';
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
      _bodyData = jsonEncode(myNotificationPreferences);

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

    MyNotificationPreferences? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MyNotificationPreferences, MyNotificationPreferences>(rawData, 'MyNotificationPreferences', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MyNotificationPreferences>(
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

  /// Bildirim ayarlarını kaydet (\&quot;Değişiklikleri kaydet\&quot;)
  /// reminderOffsetsMinutes ve reviewInviteEnabled her istekte tam gönderilir. templates isteğe bağlıdır: gönderilmeyen şablon değişmez; customBody metin ise salon metni olur, null ise sistem şablonuna dönülür. Şablon doğrulaması (400 validation_failed, alan yolu templates[i].customBody): en uzun gerçekçi değerlerle 155 GSM-7 Türkçe karakteri geçerse too_long, listede olmayan değişken invalid_format, zorunlu değişken (ör. {bağlantı}) eksikse required. Ezilemeyen olay kodu (ör. otp.code, deposit.*) templates[i].eventCode için invalid_format. Hatırlatma zamanları tekrarsız olmalıdır (tekrar varsa invalid_format). İzin: settings.manage. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [notificationSettingsUpdate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [NotificationSettings] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<NotificationSettings>> manageUpdateNotificationSettings({ 
    required String ifMatch,
    required NotificationSettingsUpdate notificationSettingsUpdate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/brand/notification-settings';
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
      _bodyData = jsonEncode(notificationSettingsUpdate);

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

    NotificationSettings? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<NotificationSettings, NotificationSettings>(rawData, 'NotificationSettings', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<NotificationSettings>(
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
