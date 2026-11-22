import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:omvila_ui/omvila_ui.dart';

/// Açılış, giriş ve bildirim izni ekranlarının petrol zemini ve eğik
/// yuvarlak kareleri (Acilis, Giris, BildirimIzni). Süsleme; ekran okuyucu
/// okumaz.
class BrandBackdrop extends StatelessWidget {
  /// Zemin.
  const BrandBackdrop({
    required this.child,
    this.borderRadius = BorderRadius.zero,
    this.secondShape = true,
    super.key,
  });

  /// İçerik.
  final Widget child;

  /// Alt köşeler (giriş ekranında yuvarlak).
  final BorderRadius borderRadius;

  /// Sol alttaki ikinci şekil.
  final bool secondShape;

  @override
  Widget build(BuildContext context) {
    final r = context.omvilaRenkler;
    return ClipRRect(
      borderRadius: borderRadius,
      child: ColoredBox(
        color: r.zeminMarka,
        child: Stack(
          fit: StackFit.passthrough,
          children: [
            Positioned(
              right: -80,
              top: 110,
              child: _Shape(color: r.kapakMercan, size: 230, angle: 0.33),
            ),
            if (secondShape)
              Positioned(
                left: -60,
                bottom: 160,
                child: _Shape(
                  color: r.marka.withValues(alpha: 0.7),
                  size: 160,
                  angle: -0.12,
                ),
              ),
            child,
          ],
        ),
      ),
    );
  }
}

class _Shape extends StatelessWidget {
  const _Shape({required this.color, required this.size, required this.angle});

  final Color color;
  final double size;
  final double angle;

  @override
  Widget build(BuildContext context) {
    return ExcludeSemantics(
      child: Transform.rotate(
        angle: angle * math.pi / 2,
        child: Container(
          width: size,
          height: size,
          decoration: BoxDecoration(
            color: color,
            borderRadius: BorderRadius.circular(size * 0.28),
          ),
        ),
      ),
    );
  }
}

/// OMVILA amblemi, koyu zemin için (design-tokens omvila-amblem-ters.svg).
/// Uygulama ikonunun kare zemini ve iç boşluğu çizilmez: amblem verilen
/// boyutu doldurur, koyu temada arkasında kare görünmez.
class AppIcon extends StatelessWidget {
  /// İkon.
  const AppIcon({this.size = 56, super.key});

  /// Kenar.
  final double size;

  @override
  Widget build(BuildContext context) {
    return SvgPicture.asset(
      'assets/omvila-amblem-ters.svg',
      width: size,
      height: size,
      semanticsLabel: 'OMVILA',
    );
  }
}
