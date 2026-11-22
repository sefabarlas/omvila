//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/sms_sender_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'sms_sender.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SmsSender {
  /// Returns a new [SmsSender] instance.
  SmsSender({

    required  this.name,

    required  this.status,

    required  this.effectiveName,

     this.submittedUtc,

     this.decidedUtc,

     this.rejectionReason,
  });

      /// Son başvurulan ad; hiç başvuru yoksa null
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: true,
  )


  final String? name;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SmsSenderStatus.unknownDefaultOpenApi,
  )


  final SmsSenderStatus status;



      /// SMS'lerin şu an gittiği ad; onaylı ad yoksa platform varsayılanı OMVILA. Yeni başvuru beklerken önceki onaylı ad sürer
  @JsonKey(
    
    name: r'effectiveName',
    required: true,
    includeIfNull: false,
  )


  final String effectiveName;



      /// Başvuru anı
  @JsonKey(
    
    name: r'submittedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? submittedUtc;



      /// Onay veya ret anı
  @JsonKey(
    
    name: r'decidedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? decidedUtc;



      /// Ret nedeni (platform ekibinin yazdığı metin)
  @JsonKey(
    
    name: r'rejectionReason',
    required: false,
    includeIfNull: false,
  )


  final String? rejectionReason;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SmsSender &&
      other.name == name &&
      other.status == status &&
      other.effectiveName == effectiveName &&
      other.submittedUtc == submittedUtc &&
      other.decidedUtc == decidedUtc &&
      other.rejectionReason == rejectionReason;

    @override
    int get hashCode =>
        (name == null ? 0 : name.hashCode) +
        status.hashCode +
        effectiveName.hashCode +
        (submittedUtc == null ? 0 : submittedUtc.hashCode) +
        (decidedUtc == null ? 0 : decidedUtc.hashCode) +
        (rejectionReason == null ? 0 : rejectionReason.hashCode);

  factory SmsSender.fromJson(Map<String, dynamic> json) => _$SmsSenderFromJson(json);

  Map<String, dynamic> toJson() => _$SmsSenderToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

