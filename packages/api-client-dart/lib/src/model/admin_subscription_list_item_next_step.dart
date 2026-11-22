//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_subscription_list_item_next_step.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSubscriptionListItemNextStep {
  /// Returns a new [AdminSubscriptionListItemNextStep] instance.
  AdminSubscriptionListItemNextStep({

    required  this.type,

    required  this.date,
  });

      /// Adım türü
  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminSubscriptionListItemNextStepTypeEnum.unknownDefaultOpenApi,
  )


  final AdminSubscriptionListItemNextStepTypeEnum type;



      /// Adımın günü (ReactivatesOnPayment ve SmsStopped'da null)
  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: true,
  )
  @DateOnlyConverter()


  final DateTime? date;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSubscriptionListItemNextStep &&
      other.type == type &&
      other.date == date;

    @override
    int get hashCode =>
        type.hashCode +
        (date == null ? 0 : date.hashCode);

  factory AdminSubscriptionListItemNextStep.fromJson(Map<String, dynamic> json) => _$AdminSubscriptionListItemNextStepFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSubscriptionListItemNextStepToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Adım türü
enum AdminSubscriptionListItemNextStepTypeEnum {
@JsonValue(r'TrialEnds')
trialEnds(r'TrialEnds'),
@JsonValue(r'ReadOnlyOn')
readOnlyOn(r'ReadOnlyOn'),
@JsonValue(r'Renews')
renews(r'Renews'),
@JsonValue(r'ReactivatesOnPayment')
reactivatesOnPayment(r'ReactivatesOnPayment'),
@JsonValue(r'SmsStopped')
smsStopped(r'SmsStopped'),
@JsonValue(r'ArchivesOn')
archivesOn(r'ArchivesOn'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminSubscriptionListItemNextStepTypeEnum(this.value);

final String value;

@override
String toString() => value;
}


