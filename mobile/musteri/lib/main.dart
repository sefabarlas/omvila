import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_musteri/app/app.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  // Riverpod'un kendiliğinden yeniden denemesi kapalı: hata ekranında
  // "Tekrar dene" kullanıcıdadır.
  runApp(
    ProviderScope(
      retry: (retryCount, error) => null,
      child: const OmvilaMusteriApp(),
    ),
  );
}
