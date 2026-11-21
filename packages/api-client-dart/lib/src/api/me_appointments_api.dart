//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/customer_cancel_request.dart';
import 'package:omvila_api/src/model/me_appointment.dart';
import 'package:omvila_api/src/model/me_appointment_page.dart';
import 'package:omvila_api/src/model/me_reschedule_request.dart';
import 'package:omvila_api/src/model/me_reschedule_slot_search.dart';
import 'package:omvila_api/src/model/me_review.dart';
import 'package:omvila_api/src/model/me_review_create.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/slot_search_result.dart';

class MeAppointmentsApi {

  final Dio _dio;

  const MeAppointmentsApi(this._dio);

  /// Randevuyu iptal et (\&quot;Iptal\&quot;, neden zorunlu)
  /// SMS bağlantısındaki iptal (publicCancelAppointment) ile aynı kurallar (A6): neden zorunlu, son süre geçtiyse 409 cutoff_passed (\&quot;Salonu ara\&quot;). Neden çipleri: PlanChanged \&quot;Planım değişti\&quot;, Sick \&quot;Rahatsızlandım\&quot; (tasarımdaki \&quot;Hastayım\&quot; yerine; sağlık bilgisi izlenimi vermemek için), WantsDifferentTime \&quot;Başka saat istiyorum\&quot;, Other (not zorunlu). Alınmış kapora için \&quot;iadesi salonun iade koşullarına bağlıdır\&quot; uyarısı istemcidedir; OMVILA iade yapmaz. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [customerCancelRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeAppointment>> meCancelAppointment({ 
    required String ifMatch,
    required String appointmentId,
    required CustomerCancelRequest customerCancelRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/appointments/{appointmentId}/cancel'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      _bodyData = jsonEncode(customerCancelRequest);

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

    MeAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeAppointment, MeAppointment>(rawData, 'MeAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeAppointment>(
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

  /// Değerlendirmeyi gönder (\&quot;Degerlendir\&quot;)
  /// publicCreateReview ile aynı kurallar (KK8): yalnız Completed randevu ve bir kez; değerlendirilmişse 409 already_reviewed, Completed değilse 409 invalid_state_transition, başka müşterinin randevusu 404 not_found. Etiket yok; puan, isteğe bağlı yorum. Yorumda \&quot;Merve Y.\&quot; görünür. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [meReviewCreate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeReview] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeReview>> meCreateReview({ 
    required String idempotencyKey,
    required MeReviewCreate meReviewCreate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/reviews';
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
      _bodyData = jsonEncode(meReviewCreate);

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

    MeReview? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeReview, MeReview>(rawData, 'MeReview', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeReview>(
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

  /// Randevu detayı (\&quot;RandevuDetay\&quot;)
  /// Başka müşterinin randevusu 404 not_found döner. Kapora isteniyorsa deposit.instructions havale bilgisini taşır (YRHavale). canCancel / canReschedule ve son süreler ErteleKapali ve \&quot;Salonu ara\&quot; gösterimini belirler; \&quot;İptal 24 saat, saat değişikliği 12 saat öncesine kadar\&quot; metni son sürelerin başlangıca farkından üretilir. Degerlendir ekranı başlığı (salon, hizmet, personel, tarih) ve \&quot;Merve Y.\&quot; (meGetProfile.reviewerDisplayName) bu yanıttan gelir. 
  ///
  /// Parameters:
  /// * [appointmentId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeAppointment>> meGetAppointment({ 
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/appointments/{appointmentId}'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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

    MeAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeAppointment, MeAppointment>(rawData, 'MeAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeAppointment>(
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

  /// Randevularım (yaklaşan) ve Geçmiş
  /// Müşterinin bütün markalardaki randevuları (her markada yalnız kendi müşteri kaydının randevuları). scope &#x3D; Upcoming: PendingApproval, Confirmed veya CheckedIn olup bitişi gelmemiş randevular, başlangıca göre artan (Main: ilk kayıt büyük kart, kalanı \&quot;Yaklaşan\&quot;). scope &#x3D; Past: geri kalanlar (Completed, Cancelled, NoShow ve bitişi geçmiş olanlar), başlangıca göre azalan (Gecmis; ay başlıkları istemcide şubenin saat diliminden). notReviewed &#x3D; true yalnız scope &#x3D; Past ile kullanılır ve canReview &#x3D; true olanları döndürür (\&quot;Değerlendirilmedi\&quot; sekmesi); Upcoming ile 400 validation_failed. \&quot;Aynısını tekrar al\&quot; (MainYaklasanYok, KK2): Upcoming boşsa uygulama scope &#x3D; Past, limit &#x3D; 1 ister; satırdaki branch, items (serviceId, staffId) ve canRebook rezervasyon akışını doldurur (publicSearchDays / publicSearchSlots). \&quot;Son randevunuz 12 Kasım&#39;da Kuaför Elif&#39;teydi\&quot; aynı satırdan üretilir. 
  ///
  /// Parameters:
  /// * [scope] - Upcoming = yaklaşan, Past = geçmiş
  /// * [notReviewed] - Yalnız değerlendirilebilir, değerlendirilmemiş geçmiş randevular (\"Değerlendirilmedi\")
  /// * [limit] 
  /// * [cursor] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeAppointmentPage] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeAppointmentPage>> meListAppointments({ 
    required String scope,
    bool? notReviewed,
    int? limit = 50,
    String? cursor,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/appointments';
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
      r'scope': scope,
      if (notReviewed != null) r'notReviewed': notReviewed,
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

    MeAppointmentPage? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeAppointmentPage, MeAppointmentPage>(rawData, 'MeAppointmentPage', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeAppointmentPage>(
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

  /// Havaleyi bildir (\&quot;Havaleyi yaptım, salona bildir\&quot;, YRHavale)
  /// publicReportDepositTransfer ile aynı davranış: deposit.status CustomerReported olur, salona staff.deposit_reported gider, randevu durumu değişmez (\&quot;Salon onayı bekleniyor\&quot;). Kendiliğinden idempotenttir (Idempotency-Key ve If-Match almaz). Kapora beklenmiyorsa 409 invalid_state_transition, tutma süresi dolmuşsa 409 hold_expired. Uygulamada oluşturulan randevuda kimlik publicCreateAppointment yanıtındaki id&#39;dir. 
  ///
  /// Parameters:
  /// * [appointmentId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeAppointment>> meReportDepositTransfer({ 
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/appointments/{appointmentId}/deposit/report'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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

    MeAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeAppointment, MeAppointment>(rawData, 'MeAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeAppointment>(
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

  /// Saati değiştir (müşteri)
  /// publicRescheduleAppointment ile aynı kurallar (A14): erteleme son süresi geçtiyse 409 cutoff_passed, saat dolduysa 409 slot_taken + meta.alternatives. Salon onaylı çalışıyorsa randevu yeniden PendingApproval olur (\&quot;Salon onaylı çalışıyorsa yeni saat onaya düşer\&quot;); alınmış kapora yeni saate aktarılır. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [meRescheduleRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [MeAppointment] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<MeAppointment>> meRescheduleAppointment({ 
    required String ifMatch,
    required String appointmentId,
    required MeRescheduleRequest meRescheduleRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/appointments/{appointmentId}/reschedule'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      _bodyData = jsonEncode(meRescheduleRequest);

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

    MeAppointment? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<MeAppointment, MeAppointment>(rawData, 'MeAppointment', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<MeAppointment>(
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

  /// Saat değişikliği için boş saatler (\&quot;RandevuErtele\&quot;)
  /// Aynı hizmetler ve aynı personelle bir günün boş saatleri; randevunun kendi süresi meşgul sayılmaz. Gün şeridindeki noktalar için uygulama publicSearchDays&#39;i randevunun kalemleriyle (items[].serviceId, staffId) çağırır. Son süre geçtiyse 409 cutoff_passed (ErteleKapali). 
  ///
  /// Parameters:
  /// * [appointmentId] 
  /// * [meRescheduleSlotSearch] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [SlotSearchResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<SlotSearchResult>> meSearchRescheduleSlots({ 
    required String appointmentId,
    required MeRescheduleSlotSearch meRescheduleSlotSearch,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/me/appointments/{appointmentId}/availability/slots'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      _bodyData = jsonEncode(meRescheduleSlotSearch);

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

    SlotSearchResult? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<SlotSearchResult, SlotSearchResult>(rawData, 'SlotSearchResult', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<SlotSearchResult>(
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
