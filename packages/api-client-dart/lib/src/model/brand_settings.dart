//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'brand_settings.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BrandSettings {
  /// Returns a new [BrandSettings] instance.
  BrandSettings({

    required  this.requiresApproval,

    required  this.approvalTimeoutMinutes,

    required  this.allowStaffSelection,

    required  this.allowAnyStaffOption,

    required  this.cancellationCutoffHours,

    required  this.rescheduleCutoffHours,

    required  this.noShowAfterMinutes,

    required  this.minNoticeMinutes,

    required  this.maxAdvanceDays,

    required  this.slotIntervalMinutes,

    required  this.defaultBranchTransferMinutes,

    required  this.depositPolicy,

    required  this.riskyCustomerNoShowThreshold,

    required  this.maskCustomerPhoneFromStaff,

    required  this.reminderOffsetsMinutes,

    required  this.reviewInviteEnabled,

     this.depositAvailable,

     this.version,
  });

  @JsonKey(
    
    name: r'requiresApproval',
    required: true,
    includeIfNull: false,
  )


  final bool requiresApproval;



  @JsonKey(
    
    name: r'approvalTimeoutMinutes',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BrandSettingsApprovalTimeoutMinutesEnum.unknownDefaultOpenApi,
  )


  final BrandSettingsApprovalTimeoutMinutesEnum approvalTimeoutMinutes;



  @JsonKey(
    
    name: r'allowStaffSelection',
    required: true,
    includeIfNull: false,
  )


  final bool allowStaffSelection;



  @JsonKey(
    
    name: r'allowAnyStaffOption',
    required: true,
    includeIfNull: false,
  )


  final bool allowAnyStaffOption;



          // minimum: 0
          // maximum: 168
  @JsonKey(
    
    name: r'cancellationCutoffHours',
    required: true,
    includeIfNull: false,
  )


  final int cancellationCutoffHours;



          // minimum: 0
          // maximum: 168
  @JsonKey(
    
    name: r'rescheduleCutoffHours',
    required: true,
    includeIfNull: false,
  )


  final int rescheduleCutoffHours;



          // minimum: 5
          // maximum: 120
  @JsonKey(
    
    name: r'noShowAfterMinutes',
    required: true,
    includeIfNull: false,
  )


  final int noShowAfterMinutes;



          // minimum: 0
          // maximum: 10080
  @JsonKey(
    
    name: r'minNoticeMinutes',
    required: true,
    includeIfNull: false,
  )


  final int minNoticeMinutes;



          // minimum: 1
          // maximum: 365
  @JsonKey(
    
    name: r'maxAdvanceDays',
    required: true,
    includeIfNull: false,
  )


  final int maxAdvanceDays;



  @JsonKey(
    
    name: r'slotIntervalMinutes',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BrandSettingsSlotIntervalMinutesEnum.unknownDefaultOpenApi,
  )


  final BrandSettingsSlotIntervalMinutesEnum slotIntervalMinutes;



          // minimum: 0
          // maximum: 240
  @JsonKey(
    
    name: r'defaultBranchTransferMinutes',
    required: true,
    includeIfNull: false,
  )


  final int defaultBranchTransferMinutes;



  @JsonKey(
    
    name: r'depositPolicy',
    required: true,
    includeIfNull: false,
  unknownEnumValue: BrandSettingsDepositPolicyEnum.unknownDefaultOpenApi,
  )


  final BrandSettingsDepositPolicyEnum depositPolicy;



          // minimum: 1
          // maximum: 10
  @JsonKey(
    
    name: r'riskyCustomerNoShowThreshold',
    required: true,
    includeIfNull: false,
  )


  final int riskyCustomerNoShowThreshold;



  @JsonKey(
    
    name: r'maskCustomerPhoneFromStaff',
    required: true,
    includeIfNull: false,
  )


  final bool maskCustomerPhoneFromStaff;



  @JsonKey(
    
    name: r'reminderOffsetsMinutes',
    required: true,
    includeIfNull: false,
  )


  final List<int> reminderOffsetsMinutes;



  @JsonKey(
    
    name: r'reviewInviteEnabled',
    required: true,
    includeIfNull: false,
  )


  final bool reviewInviteEnabled;



      /// Aktif IBAN var mı (yoksa kapora ayarları kapalı gösterilir)
  @JsonKey(
    
    name: r'depositAvailable',
    required: false,
    includeIfNull: false,
  )


  final bool? depositAvailable;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: false,
    includeIfNull: false,
  )


  final String? version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BrandSettings &&
      other.requiresApproval == requiresApproval &&
      other.approvalTimeoutMinutes == approvalTimeoutMinutes &&
      other.allowStaffSelection == allowStaffSelection &&
      other.allowAnyStaffOption == allowAnyStaffOption &&
      other.cancellationCutoffHours == cancellationCutoffHours &&
      other.rescheduleCutoffHours == rescheduleCutoffHours &&
      other.noShowAfterMinutes == noShowAfterMinutes &&
      other.minNoticeMinutes == minNoticeMinutes &&
      other.maxAdvanceDays == maxAdvanceDays &&
      other.slotIntervalMinutes == slotIntervalMinutes &&
      other.defaultBranchTransferMinutes == defaultBranchTransferMinutes &&
      other.depositPolicy == depositPolicy &&
      other.riskyCustomerNoShowThreshold == riskyCustomerNoShowThreshold &&
      other.maskCustomerPhoneFromStaff == maskCustomerPhoneFromStaff &&
      other.reminderOffsetsMinutes == reminderOffsetsMinutes &&
      other.reviewInviteEnabled == reviewInviteEnabled &&
      other.depositAvailable == depositAvailable &&
      other.version == version;

    @override
    int get hashCode =>
        requiresApproval.hashCode +
        approvalTimeoutMinutes.hashCode +
        allowStaffSelection.hashCode +
        allowAnyStaffOption.hashCode +
        cancellationCutoffHours.hashCode +
        rescheduleCutoffHours.hashCode +
        noShowAfterMinutes.hashCode +
        minNoticeMinutes.hashCode +
        maxAdvanceDays.hashCode +
        slotIntervalMinutes.hashCode +
        defaultBranchTransferMinutes.hashCode +
        depositPolicy.hashCode +
        riskyCustomerNoShowThreshold.hashCode +
        maskCustomerPhoneFromStaff.hashCode +
        reminderOffsetsMinutes.hashCode +
        reviewInviteEnabled.hashCode +
        depositAvailable.hashCode +
        version.hashCode;

  factory BrandSettings.fromJson(Map<String, dynamic> json) => _$BrandSettingsFromJson(json);

  Map<String, dynamic> toJson() => _$BrandSettingsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum BrandSettingsApprovalTimeoutMinutesEnum {
@JsonValue(30)
number30('30'),
@JsonValue(60)
number60('60'),
@JsonValue(120)
number120('120'),
@JsonValue(240)
number240('240'),
@JsonValue(11184809)
unknownDefaultOpenApi('11184809');

const BrandSettingsApprovalTimeoutMinutesEnum(this.value);

final String value;

@override
String toString() => value;
}


enum BrandSettingsSlotIntervalMinutesEnum {
@JsonValue(5)
number5('5'),
@JsonValue(10)
number10('10'),
@JsonValue(15)
number15('15'),
@JsonValue(11184809)
unknownDefaultOpenApi('11184809');

const BrandSettingsSlotIntervalMinutesEnum(this.value);

final String value;

@override
String toString() => value;
}


enum BrandSettingsDepositPolicyEnum {
@JsonValue(r'None')
none(r'None'),
@JsonValue(r'RiskyCustomersOnly')
riskyCustomersOnly(r'RiskyCustomersOnly'),
@JsonValue(r'Always')
always(r'Always'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const BrandSettingsDepositPolicyEnum(this.value);

final String value;

@override
String toString() => value;
}


