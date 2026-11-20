# PhoneField

+90 önekli telefon alanı; rezervasyonda tek kimlik bilgisi.

- Numara `532 123 45 67` biçiminde gruplanır, başta 0 yazılmaz.
- İpucu SMS geleceğini söyler.

**Flutter:** `TextFormField(keyboardType: TextInputType.phone, autofillHints: [AutofillHints.telephoneNumber])` + `prefixText: '+90 '` + maske biçimlendirici.
