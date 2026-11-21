//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'appointment_history_entry.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AppointmentHistoryEntry {
  /// Returns a new [AppointmentHistoryEntry] instance.
  AppointmentHistoryEntry({

    required  this.occurredUtc,

    required  this.kind,

     this.actorName,

     this.detail,
  });

  @JsonKey(
    
    name: r'occurredUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime occurredUtc;



      /// Approved = \"Onaylandı\" (A2), Declined = salon reddetti (A4; müşteriye iptal), HoldExpired = onay veya kapora süresi doldu, otomatik iptal (A5)
  @JsonKey(
    
    name: r'kind',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AppointmentHistoryEntryKindEnum.unknownDefaultOpenApi,
  )


  final AppointmentHistoryEntryKindEnum kind;



  @JsonKey(
    
    name: r'actorName',
    required: false,
    includeIfNull: false,
  )


  final String? actorName;



  @JsonKey(
    
    name: r'detail',
    required: false,
    includeIfNull: false,
  )


  final String? detail;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AppointmentHistoryEntry &&
      other.occurredUtc == occurredUtc &&
      other.kind == kind &&
      other.actorName == actorName &&
      other.detail == detail;

    @override
    int get hashCode =>
        occurredUtc.hashCode +
        kind.hashCode +
        (actorName == null ? 0 : actorName.hashCode) +
        (detail == null ? 0 : detail.hashCode);

  factory AppointmentHistoryEntry.fromJson(Map<String, dynamic> json) => _$AppointmentHistoryEntryFromJson(json);

  Map<String, dynamic> toJson() => _$AppointmentHistoryEntryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Approved = \"Onaylandı\" (A2), Declined = salon reddetti (A4; müşteriye iptal), HoldExpired = onay veya kapora süresi doldu, otomatik iptal (A5)
enum AppointmentHistoryEntryKindEnum {
@JsonValue(r'Created')
created(r'Created'),
@JsonValue(r'Approved')
approved(r'Approved'),
@JsonValue(r'Declined')
declined(r'Declined'),
@JsonValue(r'HoldExpired')
holdExpired(r'HoldExpired'),
@JsonValue(r'Rescheduled')
rescheduled(r'Rescheduled'),
@JsonValue(r'Cancelled')
cancelled(r'Cancelled'),
@JsonValue(r'CheckedIn')
checkedIn(r'CheckedIn'),
@JsonValue(r'NoShow')
noShow(r'NoShow'),
@JsonValue(r'NoShowUndone')
noShowUndone(r'NoShowUndone'),
@JsonValue(r'Completed')
completed(r'Completed'),
@JsonValue(r'DepositReported')
depositReported(r'DepositReported'),
@JsonValue(r'DepositReceived')
depositReceived(r'DepositReceived'),
@JsonValue(r'NotificationSent')
notificationSent(r'NotificationSent'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AppointmentHistoryEntryKindEnum(this.value);

final String value;

@override
String toString() => value;
}


