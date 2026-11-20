# OtpInput

6 haneli SMS doğrulama kodu.

- SMS'ten otomatik doldurma açık; kod tamamlanınca kendiliğinden doğrulanır.
- Hata: "Kod hatalı. Tekrar deneyin."; 60 sn sonra "Kodu yeniden gönder".

**Flutter:** `pinput` paketi veya 6 `TextField`; iOS'ta `autofillHints: [AutofillHints.oneTimeCode]`, Android'de SMS Retriever.
