import 'package:dio/dio.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// API çağrısını çalıştırır; Dio hatasını [ApiProblem]'e çevirir. Ekranlar
/// yalnız [ApiProblem] görür (kod → Türkçe metin).
Future<T> apiCall<T>(Future<T> Function() body) async {
  try {
    return await body();
  } on DioException catch (e) {
    throw ApiProblem.fromDioException(e);
  }
}
