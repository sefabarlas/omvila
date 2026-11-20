# Skeleton

Liste yüklenirken iskelet; 300 ms'den kısa yüklemede gösterilmez.

- Hareket azaltma açıksa nabız animasyonu durur.

**Flutter:** `shimmer` paketi yerine düz `r.iskelet` blokları + `AnimatedOpacity`; `MediaQuery.disableAnimations` kontrolü.
