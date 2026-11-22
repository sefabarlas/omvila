import 'package:flutter/widgets.dart';

/// Ekran yüksekliğini dolduran, sığmazsa (klavye, 1.3× yazı) kayan gövde;
/// içindeki `Spacer` ana eylemi alta iter.
class FullHeightScroll extends StatelessWidget {
  /// Gövde.
  const FullHeightScroll({required this.child, super.key});

  /// İçerik (genellikle `Column`).
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) => SingleChildScrollView(
        child: ConstrainedBox(
          constraints: BoxConstraints(minHeight: constraints.maxHeight),
          child: IntrinsicHeight(child: child),
        ),
      ),
    );
  }
}
