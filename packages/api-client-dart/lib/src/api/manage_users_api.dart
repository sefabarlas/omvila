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

import 'package:omvila_api/src/model/manage_user.dart';
import 'package:omvila_api/src/model/manage_user_list.dart';
import 'package:omvila_api/src/model/manage_user_update.dart';
import 'package:omvila_api/src/model/problem.dart';
import 'package:omvila_api/src/model/role_matrix.dart';

class ManageUsersApi {

  final Dio _dio;

  const ManageUsersApi(this._dio);

  /// Rol izinleri (salt okunur matris)
  /// AyarlarRoller \&quot;Rol izinleri\&quot; tablosu (\&quot;İşletme sahibi her şeyi görür\&quot;). Satırlar docs/alan/roller-yetkiler.md izin kataloğunun ekran gruplarıdır; her satır için rol başına erişim düzeyi döner (All &#x3D; tam, Branch &#x3D; kendi şube(ler)i, Own &#x3D; yalnız kendi kayıtları \&quot;Kendi\&quot;, None &#x3D; yok \&quot;—\&quot;). Matris v1&#39;de düzenlenemez (markaya özel rol yok, K6). Satır adları istemcide group kodundan üretilir. İzin: settings.roles (yalnız Owner). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [RoleMatrix] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<RoleMatrix>> manageGetRoleMatrix({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/roles';
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

    RoleMatrix? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<RoleMatrix, RoleMatrix>(rawData, 'RoleMatrix', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<RoleMatrix>(
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

  /// Kullanıcılar (Ayarlar → Roller ve yetkiler)
  /// AyarlarRoller \&quot;Kullanıcılar\&quot; tablosu (Ad, Rol, Şube, Durum: Aktif / Davet gönderildi). Markadaki bütün üyelikler (Disabled dahil), ada göre sıralı. \&quot;Kullanıcı ekle\&quot; ayrı uç değildir: personel davet akışı (manageCreateStaff, telefon + \&quot;Davet SMS&#39;i gönder\&quot;) kullanılır; markaya özel rol yoktur (K6). İzin: settings.roles (yalnız Owner). 
  ///
  /// Parameters:
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [ManageUserList] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ManageUserList>> manageListUsers({ 
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/users';
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

    ManageUserList? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ManageUserList, ManageUserList>(rawData, 'ManageUserList', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ManageUserList>(
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

  /// Kullanıcının rolünü ve şubelerini düzenle (\&quot;Düzenle\&quot;)
  /// Kullanıcının markadaki rolü ve şubeleri değişir (K6). Owner rolünde branchIds yok sayılır (tüm şubeler); BranchManager ve Staff için en az bir şube gerekir (yoksa 400 validation_failed, errors.branchIds &#x3D; [required]). Markadaki son Owner başka role alınamaz: 409 last_owner_required. Rol değişince kullanıcının açık oturumlarındaki izinler bir sonraki jeton yenilemede güncellenir. Değişiklik denetime yazılır. Başka markanın kullanıcısı 404. İzin: settings.roles (yalnız Owner). 
  ///
  /// Parameters:
  /// * [ifMatch] - GET yanıtındaki ETag değeri
  /// * [userId] - Salon kullanıcısı kimliği
  /// * [manageUserUpdate] 
  /// * [cancelToken] - A [CancelToken] that can be used to cancel the operation
  /// * [headers] - Can be used to add additional headers to the request
  /// * [extras] - Can be used to add flags to the request
  /// * [validateStatus] - A [ValidateStatus] callback that can be used to determine request success based on the HTTP status of the response
  /// * [onSendProgress] - A [ProgressCallback] that can be used to get the send progress
  /// * [onReceiveProgress] - A [ProgressCallback] that can be used to get the receive progress
  ///
  /// Returns a [Future] containing a [Response] with a [ManageUser] as data
  /// Throws [DioException] if API call or serialization fails
  Future<Response<ManageUser>> manageUpdateUser({ 
    required String ifMatch,
    required String userId,
    required ManageUserUpdate manageUserUpdate,
    CancelToken? cancelToken,
    Map<String, dynamic>? headers,
    Map<String, dynamic>? extra,
    ValidateStatus? validateStatus,
    ProgressCallback? onSendProgress,
    ProgressCallback? onReceiveProgress,
  }) async {
    final _path = r'/api/v1/manage/users/{userId}'.replaceAll('{' r'userId' '}', userId.toString());
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
      _bodyData = jsonEncode(manageUserUpdate);

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

    ManageUser? _responseData;

    try {
final rawData = _response.data;
_responseData = rawData == null ? null : deserialize<ManageUser, ManageUser>(rawData, 'ManageUser', growable: true);

    } catch (error, stackTrace) {
      throw DioException(
        requestOptions: _response.requestOptions,
        response: _response,
        type: DioExceptionType.unknown,
        error: error,
        stackTrace: stackTrace,
      );
    }

    return Response<ManageUser>(
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
