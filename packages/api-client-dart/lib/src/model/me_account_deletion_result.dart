//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_account_deletion_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeAccountDeletionResult {
  /// Returns a new [MeAccountDeletionResult] instance.
  MeAccountDeletionResult({

    required  this.deletedUtc,

    required  this.cancelledAppointmentCount,
  });

  @JsonKey(
    
    name: r'deletedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime deletedUtc;



      /// İptal edilen yaklaşan randevular
          // minimum: 0
  @JsonKey(
    
    name: r'cancelledAppointmentCount',
    required: true,
    includeIfNull: false,
  )


  final int cancelledAppointmentCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeAccountDeletionResult &&
      other.deletedUtc == deletedUtc &&
      other.cancelledAppointmentCount == cancelledAppointmentCount;

    @override
    int get hashCode =>
        deletedUtc.hashCode +
        cancelledAppointmentCount.hashCode;

  factory MeAccountDeletionResult.fromJson(Map<String, dynamic> json) => _$MeAccountDeletionResultFromJson(json);

  Map<String, dynamic> toJson() => _$MeAccountDeletionResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

