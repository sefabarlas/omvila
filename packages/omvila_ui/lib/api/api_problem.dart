import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:omvila_ui/errors/tr.dart';

/// Sunucunun döndürdüğü RFC 9457 hatası. Kullanıcıya `title`/`detail` değil,
/// `code`'a göre metin gösterilir ([message]). Bilinmeyen kod da gelebilir.
final class ApiProblem implements Exception {
  /// Ham alanlardan.
  const ApiProblem({
    required this.status,
    required this.code,
    this.errors = const {},
    this.meta = const {},
    this.traceId,
  });

  /// Dio hatasından. Bağlantı hatasında `status` 0 ve kod `network_error`.
  factory ApiProblem.fromDioException(DioException error) {
    final response = error.response;
    if (response == null) {
      return const ApiProblem(status: 0, code: networkErrorCode);
    }

    final status = response.statusCode ?? 0;
    final body = _decode(response.data);
    final code = body?['code'];
    return ApiProblem(
      status: status,
      code: code is String ? code : (status >= 500 ? 'try_again' : 'unknown'),
      errors: _errors(body?['errors']),
      meta: body?['meta'] is Map<String, dynamic>
          ? body!['meta'] as Map<String, dynamic>
          : const {},
      traceId: body?['traceId'] is String ? body!['traceId'] as String : null,
    );
  }

  /// İstemcinin kendi kodu: sunucuya ulaşılamadı ("Bağlantı yok" deseni).
  static const networkErrorCode = 'network_error';

  /// HTTP durum kodu; bağlantı hatasında 0.
  final int status;

  /// Sözleşmedeki `ErrorCode` değeri (snake_case) veya bilinmeyen kod.
  final String code;

  /// Alan hataları (`validation_failed`): alan → kodlar.
  final Map<String, List<String>> errors;

  /// Koda özgü ek bilgi.
  final Map<String, dynamic> meta;

  /// Sunucu iz kimliği (destek için).
  final String? traceId;

  /// Kullanıcıya gösterilecek Türkçe metin.
  String get message => errorMessage(code);

  /// Sunucuya ulaşılamadı.
  bool get isNetworkError => code == networkErrorCode;

  static Map<String, dynamic>? _decode(Object? data) {
    if (data is Map<String, dynamic>) {
      return data;
    }

    if (data is String && data.isNotEmpty) {
      try {
        final decoded = jsonDecode(data);
        return decoded is Map<String, dynamic> ? decoded : null;
      } on FormatException {
        return null;
      }
    }

    return null;
  }

  static Map<String, List<String>> _errors(Object? value) {
    if (value is! Map<String, dynamic>) {
      return const {};
    }

    return {
      for (final entry in value.entries)
        if (entry.value is List)
          entry.key: [
            for (final item in entry.value as List<dynamic>)
              if (item is String) item,
          ],
    };
  }

  @override
  String toString() => 'ApiProblem($status, $code)';
}
