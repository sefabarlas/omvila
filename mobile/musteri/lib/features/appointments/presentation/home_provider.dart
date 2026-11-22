import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:omvila_musteri/app/providers.dart';
import 'package:omvila_musteri/features/appointments/data/appointments_repository.dart';

/// Randevularım ana ekranı; yenilemek için `ref.invalidate(homeProvider)`.
final FutureProvider<HomeData> homeProvider =
    FutureProvider.autoDispose<HomeData>(
      (ref) => ref.watch(appointmentsRepositoryProvider).loadHome(),
    );
