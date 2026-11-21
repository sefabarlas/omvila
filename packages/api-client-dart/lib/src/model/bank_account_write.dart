//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'bank_account_write.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankAccountWrite {
  /// Returns a new [BankAccountWrite] instance.
  BankAccountWrite({

    required  this.accountHolderName,

    required  this.bankName,

     this.iban,

    required  this.transferCodePrefix,

    required  this.transferWindowMinutes,
  });

      /// Alıcı adı; hesap sahibiyle aynı olmalı
  @JsonKey(
    
    name: r'accountHolderName',
    required: true,
    includeIfNull: false,
  )


  final String accountHolderName;



      /// Banka
  @JsonKey(
    
    name: r'bankName',
    required: true,
    includeIfNull: false,
  )


  final String bankName;



      /// Tam IBAN girişi; boşluk ve küçük harf kabul edilir. Sunucu boşlukları atıp TR biçimini (TR + 24 hane) ve mod-97 denetim hanesini doğrular; geçersizse 400 validation_failed, errors.iban = [invalid_format]. 
  @JsonKey(
    
    name: r'iban',
    required: false,
    includeIfNull: false,
  )


  final String? iban;



      /// Açıklama kodu öneki (2–5 büyük Latin harfi)
  @JsonKey(
    
    name: r'transferCodePrefix',
    required: true,
    includeIfNull: false,
  )


  final String transferCodePrefix;



      /// Havale süresi (1, 2 veya 4 saat)
  @JsonKey(
    
    name: r'transferWindowMinutes',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BankAccountWriteTransferWindowMinutesEnum.unknownDefaultOpenApi,
  )


  final BankAccountWriteTransferWindowMinutesEnum transferWindowMinutes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankAccountWrite &&
      other.accountHolderName == accountHolderName &&
      other.bankName == bankName &&
      other.iban == iban &&
      other.transferCodePrefix == transferCodePrefix &&
      other.transferWindowMinutes == transferWindowMinutes;

    @override
    int get hashCode =>
        accountHolderName.hashCode +
        bankName.hashCode +
        iban.hashCode +
        transferCodePrefix.hashCode +
        transferWindowMinutes.hashCode;

  factory BankAccountWrite.fromJson(Map<String, dynamic> json) => _$BankAccountWriteFromJson(json);

  Map<String, dynamic> toJson() => _$BankAccountWriteToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Havale süresi (1, 2 veya 4 saat)
enum BankAccountWriteTransferWindowMinutesEnum {
@JsonValue(60)
number60('60'),
@JsonValue(120)
number120('120'),
@JsonValue(240)
number240('240'),
@JsonValue(11184809)
unknownDefaultOpenApi('11184809');

const BankAccountWriteTransferWindowMinutesEnum(this.value);

final String value;

@override
String toString() => value;
}


