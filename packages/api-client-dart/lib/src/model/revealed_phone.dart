//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'revealed_phone.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class RevealedPhone {
  /// Returns a new [RevealedPhone] instance.
  RevealedPhone({

    required  this.phone,
  });

      /// E.164 telefon
  @JsonKey(
    
    name: r'phone',
    required: true,
    includeIfNull: false,
  )


  final String phone;





    @override
    bool operator ==(Object other) => identical(this, other) || other is RevealedPhone &&
      other.phone == phone;

    @override
    int get hashCode =>
        phone.hashCode;

  factory RevealedPhone.fromJson(Map<String, dynamic> json) => _$RevealedPhoneFromJson(json);

  Map<String, dynamic> toJson() => _$RevealedPhoneToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

