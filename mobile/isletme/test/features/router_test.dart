import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:omvila_isletme/app/router.dart';
import 'package:omvila_isletme/features/push/data/push_prompt_controller.dart';
import 'package:omvila_isletme/features/push/data/push_service.dart';
import 'package:omvila_isletme/features/shell/presentation/tabs.dart';

import '../helpers/fixtures.dart';

// Kural: ürün kuralı 7 (giriş yalnız telefon + SMS), kodlama-flutter.md §
// Bildirimler (izin önce kendi ekranımızla), roller-yetkiler.md § Menü
// görünürlüğü ve teslim-notlari.md § Rol farkı (personel: Takvimim,
// Müşteriler, Kapıda sıra, Primim).
void main() {
  const asked = AsyncData(
    PushPromptState(
      asked: true,
      bannerDismissed: false,
      permission: PushPermission.granted,
    ),
  );
  const notAsked = AsyncData(
    PushPromptState(
      asked: false,
      bannerDismissed: false,
      permission: PushPermission.notDetermined,
    ),
  );
  final owner = AsyncData(session(ownerSessionJson));
  final staff = AsyncData(session(staffSessionJson));

  test('oturum okunurken açılış', () {
    expect(
      redirectFor(
        location: Routes.today,
        auth: const AsyncLoading(),
        prompt: asked,
      ),
      Routes.splash,
    );
    expect(
      redirectFor(
        location: Routes.splash,
        auth: AsyncError(Exception(), StackTrace.empty),
        prompt: asked,
      ),
      isNull,
    );
  });

  test('oturum yoksa giriş; kod adımı serbest', () {
    expect(
      redirectFor(
        location: Routes.today,
        auth: const AsyncData(null),
        prompt: asked,
      ),
      Routes.login,
    );
    expect(
      redirectFor(
        location: Routes.loginCode,
        auth: const AsyncData(null),
        prompt: asked,
      ),
      isNull,
    );
  });

  test('girişten sonra izin ekranı bir kez, sonra Bugün', () {
    expect(
      redirectFor(location: Routes.loginCode, auth: owner, prompt: notAsked),
      Routes.notificationPermission,
    );
    expect(
      redirectFor(location: Routes.splash, auth: owner, prompt: asked),
      Routes.today,
    );
    expect(
      redirectFor(location: Routes.today, auth: owner, prompt: asked),
      isNull,
    );
  });

  test('yetkisiz sekme Bugün’e döner; derin bağlantı serbest', () {
    expect(
      redirectFor(location: Routes.notifications, auth: staff, prompt: asked),
      Routes.today,
    );
    expect(
      redirectFor(location: Routes.walkIns, auth: owner, prompt: asked),
      isNull,
    );
    expect(
      redirectFor(
        location: Routes.appointment(pendingId),
        auth: staff,
        prompt: asked,
      ),
      isNull,
    );
  });

  test('sekmeler rol ve izne göre', () {
    expect(tabsFor(session(ownerSessionJson)).map((t) => t.label), [
      'Bugün',
      'Takvim',
      'Müşteriler',
      'Bildirimler',
      'Profil',
    ]);
    expect(tabsFor(session(staffSessionJson)).map((t) => t.label), [
      'Bugün',
      'Takvimim',
      'Müşteriler',
      'Kapıda sıra',
      'Profil',
    ]);
  });
}
