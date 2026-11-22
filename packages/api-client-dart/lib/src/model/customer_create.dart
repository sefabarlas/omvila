//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerCreate {
  /// Returns a new [CustomerCreate] instance.
  CustomerCreate({

    required  this.fullName,

    required  this.phone,

     this.birthYear,

     this.notes,

     this.sendAppointmentSms = true,
  });

  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// Serbest biçimli telefon girişi; sunucu E.164'e çevirir (\"0532 418 77 05\", \"5324187705\", \"+90 532 418 7705\")
  @JsonKey(
    
    name: r'phone',
    required: true,
    includeIfNull: false,
  )


  final String phone;



          // minimum: 1900
          // maximum: 2026
  @JsonKey(
    
    name: r'birthYear',
    required: false,
    includeIfNull: false,
  )


  final int? birthYear;



      /// Sağlık bilgisi yazılmaz
  @JsonKey(
    
    name: r'notes',
    required: false,
    includeIfNull: false,
  )


  final String? notes;



      /// \"Randevu bilgilendirme SMS'leri gönderilsin\"
  @JsonKey(
    defaultValue: true,
    name: r'sendAppointmentSms',
    required: false,
    includeIfNull: false,
  )


  final bool? sendAppointmentSms;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerCreate &&
      other.fullName == fullName &&
      other.phone == phone &&
      other.birthYear == birthYear &&
      other.notes == notes &&
      other.sendAppointmentSms == sendAppointmentSms;

    @override
    int get hashCode =>
        fullName.hashCode +
        phone.hashCode +
        (birthYear == null ? 0 : birthYear.hashCode) +
        (notes == null ? 0 : notes.hashCode) +
        sendAppointmentSms.hashCode;

  factory CustomerCreate.fromJson(Map<String, dynamic> json) => _$CustomerCreateFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

