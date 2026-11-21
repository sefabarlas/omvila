//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'dart:async';

// ignore: unused_import
import 'dart:convert';
import 'package:omvila_api/src/deserialize.dart';
import 'package:dio/dio.dart';

import 'package:omvila_api/src/model/appointment_created.dart';
import 'package:omvila_api/src/model/appointment_detail.dart';
import 'package:omvila_api/src/model/calendar_view.dart';
import 'package:omvila_api/src/model/deposit_refund_request.dart';
import 'package:omvila_api/src/model/manage_appointment_create.dart';
import 'package:omvila_api/src/model/manage_approve_request.dart';
import 'package:omvila_api/src/model/manage_cancel_request.dart';
import 'package:omvila_api/src/model/manage_reschedule_request.dart';
import 'package:omvila_api/src/model/pending_approval_list.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/slot_search_request.dart';
import 'package:omvila_api/src/model/slot_search_result.dart';

class ManageCalendarApi {

  final Dio _dio;

  const ManageCalendarApi(this._dio);

  /// Onay bekleyen talebi onayla (\&quot;Onayla\&quot;)
  /// Randevu yan paneli (RandevuPanel) \&quot;Onayla\&quot; butonu. Geçiş A2: PendingApproval → Confirmed; holdExpiresUtc temizlenir, müşteriye appointment.approved bildirimi gider, hatırlatmalar planlanır. Gövde isteğe bağlıdır. Kapora istenmiş ve gelmemişse (deposit.status Awaiting veya CustomerReported) onay için waiveDeposit &#x3D; true gerekir (\&quot;Kaporasız onayla\&quot;); gönderilmezse 409 deposit_required (istemci \&quot;Kapora geldi\&quot; veya \&quot;Kaporasız onayla\&quot; seçeneğini sunar). waiveDeposit &#x3D; true ile onaylanınca deposit.status Waived olur, geçmişe DepositWaived yazılır, Payment oluşmaz ve \&quot;Salonda ödenecek\&quot; (amountDue) toplam tutara döner. Kapora yoksa veya alınmışsa waiveDeposit yok sayılır. Durum PendingApproval değilse 409 invalid_state_transition. Tutma süresi dolmuş ama otomatik iptal işi henüz çalışmamışsa 409 hold_expired. İzin: appointment.approve (Owner, BranchManager kendi şubesinde). Personel rolü 403 forbidden alır; kapsam dışı randevu 404. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [manageApproveRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageApproveAppointment({ 
    required String ifMatch,
    required String appointmentId,
    ManageApproveRequest? manageApproveRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/approve'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      _bodyData = jsonEncode(manageApproveRequest);

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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Randevuyu iptal et
  /// Tekrarlayan randevuda scope zorunludur. notifyCustomer false ise müşteriye mesaj gitmez. İzin: appointment.cancel
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [manageCancelRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageCancelAppointment({ 
    required String ifMatch,
    required String appointmentId,
    required ManageCancelRequest manageCancelRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/cancel'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      _bodyData = jsonEncode(manageCancelRequest);

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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Geldi olarak işaretle
  /// 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageCheckInAppointment({ 
    required String ifMatch,
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/check-in'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Randevu oluştur (\&quot;Yeni randevu\&quot;)
  /// Panel/telefon kanalında randevu Confirmed oluşur, kapora istenmez. Çalışma saati dışı veya izinli personel için önce 409 outside_working_hours / staff_on_time_off döner; kullanıcı onaylarsa allowOutsideWorkingHours &#x3D; true ile tekrar gönderilir. Çakışma asla geçilemez: 409 slot_taken. recurrence verilirse ilk 3 aylık tekrarlar oluşur; çakışan tekrarlar skippedOccurrences içinde döner. 
  ///
  /// Parameters:
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [manageAppointmentCreate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentCreated] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentCreated>> manageCreateAppointment({ 
    required String idempotencyKey,
    required ManageAppointmentCreate manageAppointmentCreate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments';
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
      _bodyData = jsonEncode(manageAppointmentCreate);

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

    AppointmentCreated? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentCreated, AppointmentCreated>(rawData, 'AppointmentCreated', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentCreated>(
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

  /// Onay bekleyen talebi reddet (\&quot;Reddet\&quot;)
  /// Randevu yan paneli (RandevuPanel) \&quot;Reddet\&quot; butonu. Geçiş A4: PendingApproval → Cancelled, neden sabit DeclinedBySalon (cancellation.cancelledBy &#x3D; Salon). Segmentler pasifleşir, slot boşalır, paket seansı geri verilir, bekleme listesi tetiklenir. Müşteriye appointment.declined bildirimi gider; müşteri metninde ve Randevum sayfasında \&quot;iptal\&quot; yazar, \&quot;reddedildi\&quot; geçmez. Gövde yoktur: tasarımda neden veya not alanı bulunmaz, müşteriye bildirim her zaman gider (talep yanıtsız kalmamalı). Durum PendingApproval değilse 409 invalid_state_transition; tutma süresi dolmuşsa 409 hold_expired. İzin: appointment.approve. Personel rolü 403 forbidden alır; kapsam dışı randevu 404. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageDeclineAppointment({ 
    required String ifMatch,
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/decline'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Randevu ayrıntısı (yan panel)
  /// 
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
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageGetAppointment({ 
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Takvim (gün veya hafta)
  /// Günlük görünüm: from &#x3D; to, personel kolonları. Haftalık görünüm: 7 gün, staffIds tek personel. Personel rolünde yalnız kendi kolonu döner. Aralık en fazla 31 gün. 
  ///
  /// Parameters:
  /// * [branchId] 
  /// * [from] 
  /// * [to] 
  /// * [staffIds] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CalendarView] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CalendarView>> manageGetCalendar({ 
    required String branchId,
    required DateTime from,
    required DateTime to,
    List<String>? staffIds,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/calendar';
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
      r'branchId': branchId,
      r'from': from,
      r'to': to,
      if (staffIds != null) r'staffIds': staffIds,
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    CalendarView? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CalendarView, CalendarView>(rawData, 'CalendarView', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CalendarView>(
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

  /// Onay bekleyen talepler (\&quot;Dikkat isteyenler\&quot;, \&quot;Onay bekliyor\&quot; bentosu)
  /// Genel bakış \&quot;Onay bekliyor\&quot; bentosu (\&quot;ilki 1 sa 35 dk içinde iptal olur\&quot;), \&quot;Dikkat isteyenler\&quot; kartındaki \&quot;2 online talep onay bekliyor\&quot; satırı ve takvim üst bandı. Yalnız PendingApproval randevular, holdExpiresUtc&#39;ye göre artan sıralı (en acil önce). Kapora bekleyen talepler (displayStatus DepositPending) de listededir; depositStatus ve depositAmount ile \&quot;Kapora bekleniyor\&quot; ve \&quot;Havale bildirildi · kontrol edin\&quot; gösterilir. Sayfalanmaz: talepler en geç onay süresi (en fazla 240 dk) içinde düştüğü için liste kısadır. Kalan süre istemcide holdExpiresUtc ile generatedUtc farkından sayılır; cihaz saati kullanılmaz (generatedUtc sunucunun \&quot;şimdi\&quot;sidir). branchId verilirse yalnız o şube; verilmezse kullanıcının kapsamındaki bütün şubeler. İzin: appointment.approve. Personel rolü 403 forbidden alır (onay butonlarını görmez). 
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
  /// Returns a [Future] containing a [Response] with a [PendingApprovalList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<PendingApprovalList>> manageListPendingApprovals({ 
    String? branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/approval-queue';
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

    PendingApprovalList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<PendingApprovalList, PendingApprovalList>(rawData, 'PendingApprovalList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<PendingApprovalList>(
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

  /// Kapora geldi olarak işaretle (\&quot;Kapora geldi\&quot;)
  /// Randevu yan paneli ve işletme uygulaması \&quot;Kapora geldi mi?\&quot; (KaporaOnay) \&quot;Kapora geldi\&quot; butonu. Geçiş A3: PendingApproval → Confirmed. Salon havaleyi kendi bankasında görür ve işaretler; OMVILA para almaz, yalnız kaydeder. Salonun işaretlemesi onay sayılır (onay da gerekiyorsa ayrıca \&quot;Onayla\&quot; gerekmez). Yan etkiler: deposit.status Received, receivedUtc; Payment(Method &#x3D; Transfer, IsDeposit &#x3D; true, tutar &#x3D; deposit.required, Reference &#x3D; açıklama kodu) ve randevu tahsis kaydı; amountDue kapora kadar düşer; holdExpiresUtc temizlenir; geçmişe DepositReceived yazılır; müşteriye deposit.received SMS&#39;i gider, hatırlatmalar planlanır. Gövde yoktur: tutar istenen kaporadır. Payment oluşturduğu için Idempotency-Key ister; aynı anahtarla tekrar gelen istek ilk yanıtı alır. Randevu PendingApproval değilse veya kapora beklenmiyorsa (kapora yok, Received, Waived, Refunded) 409 invalid_state_transition. Tutma süresi dolmuş ama otomatik iptal işi henüz çalışmamışsa 409 hold_expired. İzin: appointment.deposit.mark (Owner, BranchManager kendi şubesinde). Personel rolü v1&#39;de 403 forbidden alır; kapsam dışı randevu 404. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [appointmentId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageMarkDepositReceived({ 
    required String ifMatch,
    required String idempotencyKey,
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/deposit/mark-received'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      validateStatus: validateStatus,
    );

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Kapora iadesini işaretle (\&quot;Salon iade etti\&quot;)
  /// Randevu yan paneli \&quot;Salon iade etti\&quot;. Salon iadeyi kendi bankasından yapar; OMVILA para göndermez, yalnız kaydeder. Tutar elle girilir ve alınan kaporayı aşamaz (aşarsa veya 0 ise 400 validation_failed, errors.amount &#x3D; [out_of_range]); kısmi iade olabilir. Yan etkiler: deposit.status Refunded, refundedUtc, refundedAmount; kapora Payment kaydı Refunded veya PartiallyRefunded olur (RefundedAmountMinor); geçmişe DepositRefunded yazılır (not varsa detail&#39;de, yalnız panelde görünür). Müşteriye bildirim gitmez; müşteri uygulamasında Ödemelerim \&quot;Salon iade etti\&quot; gösterir (Dilim 5). Randevu durumu değişmez (iptal, tamamlanmış veya onaylı olabilir). Kapora alınmamışsa (deposit.status Received değil) veya zaten iade işaretlendiyse 409 invalid_state_transition. Para kaydettiği için Idempotency-Key ister. İzin: appointment.deposit.mark (Owner, BranchManager kendi şubesinde); personel 403, kapsam dışı randevu 404. 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [idempotencyKey] - İstemcinin ürettiği UUID; aynı anahtarla tekrar gelen istek ilk yanıtı alır (24 saat)
  /// * [appointmentId] 
  /// * [depositRefundRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageMarkDepositRefunded({ 
    required String ifMatch,
    required String idempotencyKey,
    required String appointmentId,
    required DepositRefundRequest depositRefundRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/deposit/refund'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      _bodyData = jsonEncode(depositRefundRequest);

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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Kapora hatırlatması gönder (\&quot;Henüz gelmedi, hatırlat\&quot;)
  /// KaporaOnay ve randevu yan paneli \&quot;Hatırlat\&quot;. Müşteriye deposit.awaiting SMS&#39;i kalan süreyle yeniden gönderilir (her zaman SMS; sessiz saate kaydırılmaz, 1 kontör). Durum değişmez; deposit.remindedUtc dolar ve geçmişe DepositReminded yazılır. Randevu başına en fazla bir kez: ikinci istek 409 deposit_reminder_already_sent (istemci butonu remindedUtc doluysa pasif gösterir). Kapora beklenmiyorsa (randevu PendingApproval değil veya deposit.status Awaiting/CustomerReported değil) 409 invalid_state_transition; tutma süresi dolmuşsa 409 hold_expired; kontör yoksa 409 insufficient_credits (hatırlatma hakkı harcanmaz). İzin: appointment.deposit.mark (Owner, BranchManager kendi şubesinde); personel 403, kapsam dışı randevu 404. 
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
  /// Returns a [Future]
  /// Throws [DioException] if API call or serialization fails
  Future<Response<void>> manageRemindDeposit({ 
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/deposit/remind'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      validateStatus: validateStatus,
    );

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    return _response;
  }

  /// Saati veya personeli değiştir (sürükle-bırak dahil)
  /// Salon son süreye tabi değildir. Tekrarlayan randevuda scope zorunludur.
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [manageRescheduleRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageRescheduleAppointment({ 
    required String ifMatch,
    required String appointmentId,
    required ManageRescheduleRequest manageRescheduleRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/reschedule'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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
      _bodyData = jsonEncode(manageRescheduleRequest);

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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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

  /// Panel için boş saatler (\&quot;Elif için boş saatler\&quot;)
  /// Public motorun aynısıdır; MinNotice ve MaxAdvance sınırları panelde uygulanmaz, online kapalı personel ve hizmetler dahildir.
  ///
  /// Parameters:
  /// * [branchId] 
  /// * [slotSearchRequest] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [SlotSearchResult] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<SlotSearchResult>> manageSearchSlots({ 
    required String branchId,
    required SlotSearchRequest slotSearchRequest,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/branches/{branchId}/availability/slots'.replaceAll('{' r'branchId' '}', branchId.toString());
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
      _bodyData = jsonEncode(slotSearchRequest);

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

  /// Geldiyse geri al
  /// Yalnız aynı yerel gün içinde; randevu CheckedIn olur, gelmedi sayacı düşer.
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [appointmentId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [AppointmentDetail] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<AppointmentDetail>> manageUndoNoShow({ 
    required String ifMatch,
    required String appointmentId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/appointments/{appointmentId}/undo-no-show'.replaceAll('{' r'appointmentId' '}', appointmentId.toString());
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

    AppointmentDetail? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<AppointmentDetail, AppointmentDetail>(rawData, 'AppointmentDetail', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<AppointmentDetail>(
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
