import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_isletme/app/app.dart';

void main() {
  runApp(
    // Hatalar ekranda "Tekrar dene" ile gösterilir; otomatik tekrar yok.
    const ProviderScope(retry: noRetry, child: IsletmeApp()),
  );
}

/// Riverpod'un otomatik yeniden denemesini kapatır.
Duration? noRetry(int retryCount, Object error) => null;
