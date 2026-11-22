//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'omvila_bank_account.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class OmvilaBankAccount {
  /// Returns a new [OmvilaBankAccount] instance.
  OmvilaBankAccount({

    required  this.accountHolderName,

    required  this.iban,

     this.bankName,
  });

      /// Alıcı (OMVILA şirket unvanı)
  @JsonKey(
    
    name: r'accountHolderName',
    required: true,
    includeIfNull: false,
  )


  final String accountHolderName;



      /// Tam IBAN (TR ile başlar, boşluksuz)
  @JsonKey(
    
    name: r'iban',
    required: true,
    includeIfNull: false,
  )


  final String iban;



      /// Banka adı
  @JsonKey(
    
    name: r'bankName',
    required: false,
    includeIfNull: false,
  )


  final String? bankName;





    @override
    bool operator ==(Object other) => identical(this, other) || other is OmvilaBankAccount &&
      other.accountHolderName == accountHolderName &&
      other.iban == iban &&
      other.bankName == bankName;

    @override
    int get hashCode =>
        accountHolderName.hashCode +
        iban.hashCode +
        (bankName == null ? 0 : bankName.hashCode);

  factory OmvilaBankAccount.fromJson(Map<String, dynamic> json) => _$OmvilaBankAccountFromJson(json);

  Map<String, dynamic> toJson() => _$OmvilaBankAccountToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

