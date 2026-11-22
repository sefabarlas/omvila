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

import 'package:omvila_api/src/model/commission_report.dart';
import 'package:omvila_api/src/model/manage_overview.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/report_overview.dart';

class ManageReportsApi {

  final Dio _dio;

  const ManageReportsApi(this._dio);

  /// Personel ve prim
  /// \&quot;Raporlar ve prim\&quot; ekranındaki \&quot;Personel ve prim\&quot; tablosu (Personel, Randevu, Hizmet cirosu, Oran, Prim) ve toplam (\&quot;Gelmeyen ve iptal edilen randevular prime dahil değil.\&quot;). Aralık manageGetReportOverview ile aynıdır (en fazla 31 gün, şubenin yerel takvimi). Satır: aralıkta tamamlanan randevu sayısı, hizmet primi tabanı (hizmet cirosu), hizmet prim oranı (personelin varsayılan oranı; hizmete özel oran varsa etkin ortalama), hizmet primi, satış primi ve toplam (Accrued, Approved, Paid + ters kayıtlar; eksi olabilir). Prim tutarları CommissionEntry&#39;den okunur, yeniden hesaplanmaz. Kapsam: Owner bütün personel, BranchManager kendi şubelerinin personeli, Staff (commission.view K) yalnız kendi satırı (\&quot;Primim\&quot;); personel kaydı yoksa items boş. İzin: commission.view. 
  ///
  /// Parameters:
  /// * [from] - Aralığın ilk günü (dahil, şubenin yerel takvimi)
  /// * [to] - Aralığın son günü (dahil); from ile arası en fazla 31 gün
  /// * [branchId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [CommissionReport] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<CommissionReport>> manageGetCommissionReport({ 
    required DateTime from,
    required DateTime to,
    String? branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/reports/commissions';
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
      r'from': formatDateOnly(from),
      r'to': formatDateOnly(to),
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    CommissionReport? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<CommissionReport, CommissionReport>(rawData, 'CommissionReport', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<CommissionReport>(
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

  /// Genel bakış (bento kartları ve haftalık doluluk)
  /// Salon paneli Genel bakış (GenelBakis): \&quot;BUGÜNKÜ CİRO 9.840 ₺ · 16 randevu\&quot;, \&quot;ONAY BEKLİYOR 2 · ilki 1 sa 35 dk içinde iptal olur\&quot;, \&quot;KAPIDA SIRADA 3 · ~35 dk bekleme\&quot;, \&quot;BU HAFTA DOLULUK %74 · geçen hafta %68\&quot; ve \&quot;Bu hafta doluluk · personel bazında\&quot;. \&quot;Gün akışı\&quot; manageGetCalendar, \&quot;Dikkat isteyenler\&quot; manageListPendingApprovals ile gelir; bu uç onları tekrarlamaz. Gün ve hafta şubenin saat dilimindedir (hafta Pazartesi–Pazar); kalan süreler generatedUtc&#39;ye göre sayılır. Bugünkü ciro: bugün kaydedilen tahsilatların toplamı (iade düşülmüş; paket satışı ve kapora dahil), appointmentCount bugünün iptal ve taslak dışındaki randevuları. Doluluk: haftanın dolu dakikaları / personelin çalışma dakikaları (izin ve kapalı gün düşülür); Draft ve Cancelled sayılmaz, NoShow sayılır. Çalışma dakikası olmayan personel listede yoktur; hiç çalışma dakikası yoksa percent null. \&quot;Hedefin %72&#39;si\&quot; sözleşmede yoktur (hedef ayarı yok, T-116). branchId verilmezse kullanıcının birincil şubesi. Kapsam: BranchManager yalnız kendi şubeleri (başka şube 404). İzin: report.view (Owner, BranchManager); Staff 403 forbidden (personel özeti manageGetToday&#39;dir). Yetkiye bağlı kartlar: pendingApproval yalnız appointment.approve, walkIns yalnız walkin.manage izninde dolu; aksi hâlde null. 
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
  /// Returns a [Future] containing a [Response] with a [ManageOverview] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ManageOverview>> manageGetOverview({ 
    String? branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/overview';
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

    ManageOverview? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ManageOverview, ManageOverview>(rawData, 'ManageOverview', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ManageOverview>(
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

  /// Raporlar (hak edilen gelir, tahsilat, gelmedi, online pay, günlük tahsilat)
  /// \&quot;Raporlar ve prim\&quot; ekranı (Raporlar, RaporlarBos) üst kartları ve \&quot;Günlük tahsilat\&quot; grafiği. Gün / Hafta / Ay seçimi istemcide from–to aralığına çevrilir (şubenin yerel takvimi, iki uç dahil, en fazla 31 gün; aşılırsa 400 validation_failed, errors.to &#x3D; [out_of_range]). Hak edilen gelir: aralıkta tamamlanan randevuların hizmet tutarları ve paketten düşülen seansların payı (seans kullanıldıkça). Peşin satılan paket hak edilen gelirde satış anında görünmez (KK6). earnedRevenueChangePercent bir önceki eşit uzunluktaki aralığa göre değişimdir; önceki aralıkta gelir yoksa null. Tahsilat: aralıkta kaydedilen bütün tahsilatlar (paket satışları ve kaporalar dahil), iadeler düşülmüş (\&quot;paket satışları ve kaporalar dahil\&quot;). Gelmedi: aralıktaki NoShow sayısı, sonuçlanan randevulara oranı ve kayıp (gelmeyen randevuların hizmet tutarı; alınmış kapora düşülür). Online pay: aralıktaki iptal ve taslak dışı randevuların kanalı Online olanların oranı. Günlük tahsilat: aralıktaki her gün için bir satır; şube o gün kapalıysa isClosed &#x3D; true (\&quot;Pazartesi kapalı\&quot;). Veri yoksa tutarlar 0, oranlar null döner (RaporlarBos). branchId verilmezse kullanıcının erişebildiği bütün şubeler (Owner: marka geneli). Kapsam: BranchManager yalnız kendi şubeleri (başka şube 404). İzin: report.view (Owner, BranchManager); Staff 403 forbidden. 
  ///
  /// Parameters:
  /// * [from] - Aralığın ilk günü (dahil, şubenin yerel takvimi)
  /// * [to] - Aralığın son günü (dahil); from ile arası en fazla 31 gün
  /// * [branchId] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [ReportOverview] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ReportOverview>> manageGetReportOverview({ 
    required DateTime from,
    required DateTime to,
    String? branchId,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/reports/overview';
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
      r'from': formatDateOnly(from),
      r'to': formatDateOnly(to),
    };

    final _response = await _dio.request<Object>(
      _path,
      options: _options,
      queryParameters: _queryParameters,
      cancelToken: cancelToken,
      onSendProgress: onSendProgress,
      onReceiveProgress: onReceiveProgress,
    );

    ReportOverview? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ReportOverview, ReportOverview>(rawData, 'ReportOverview', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ReportOverview>(
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
