//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/staff_service_assignment.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'staff_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class StaffCreate {
  /// Returns a new [StaffCreate] instance.
  StaffCreate({

    required  this.fullName,

     this.phone,

     this.title,

    required  this.role,

    required  this.calendarColor,

    required  this.branchIds,

     this.services,

     this.copyBranchWorkingHours = true,

     this.serviceCommissionPercent = 0,

     this.saleCommissionPercent = 0,

     this.isBookableOnline = true,

     this.sendInvite = true,
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
    required: false,
    includeIfNull: false,
  )


  final String? phone;



  @JsonKey(
    
    name: r'title',
    required: false,
    includeIfNull: false,
  )


  final String? title;



  @JsonKey(
    
    name: r'role',
    required: true,
    includeIfNull: false,
  unknownEnumValue: StaffCreateRoleEnum.unknownDefaultOpenApi,
  )


  final StaffCreateRoleEnum role;



  @JsonKey(
    
    name: r'calendarColor',
    required: true,
    includeIfNull: false,
  )


  final String calendarColor;



  @JsonKey(
    
    name: r'branchIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> branchIds;



  @JsonKey(
    
    name: r'services',
    required: false,
    includeIfNull: false,
  )


  final List<StaffServiceAssignment>? services;



      /// \"İşletme saatleriyle aynı\"
  @JsonKey(
    defaultValue: true,
    name: r'copyBranchWorkingHours',
    required: false,
    includeIfNull: false,
  )


  final bool? copyBranchWorkingHours;



          // minimum: 0
          // maximum: 100
  @JsonKey(
    defaultValue: 0,
    name: r'serviceCommissionPercent',
    required: false,
    includeIfNull: false,
  )


  final num? serviceCommissionPercent;



          // minimum: 0
          // maximum: 100
  @JsonKey(
    defaultValue: 0,
    name: r'saleCommissionPercent',
    required: false,
    includeIfNull: false,
  )


  final num? saleCommissionPercent;



  @JsonKey(
    defaultValue: true,
    name: r'isBookableOnline',
    required: false,
    includeIfNull: false,
  )


  final bool? isBookableOnline;



      /// \"Davet SMS'i gönder\"
  @JsonKey(
    defaultValue: true,
    name: r'sendInvite',
    required: false,
    includeIfNull: false,
  )


  final bool? sendInvite;





    @override
    bool operator ==(Object other) => identical(this, other) || other is StaffCreate &&
      other.fullName == fullName &&
      other.phone == phone &&
      other.title == title &&
      other.role == role &&
      other.calendarColor == calendarColor &&
      other.branchIds == branchIds &&
      other.services == services &&
      other.copyBranchWorkingHours == copyBranchWorkingHours &&
      other.serviceCommissionPercent == serviceCommissionPercent &&
      other.saleCommissionPercent == saleCommissionPercent &&
      other.isBookableOnline == isBookableOnline &&
      other.sendInvite == sendInvite;

    @override
    int get hashCode =>
        fullName.hashCode +
        phone.hashCode +
        (title == null ? 0 : title.hashCode) +
        role.hashCode +
        calendarColor.hashCode +
        branchIds.hashCode +
        services.hashCode +
        copyBranchWorkingHours.hashCode +
        serviceCommissionPercent.hashCode +
        saleCommissionPercent.hashCode +
        isBookableOnline.hashCode +
        sendInvite.hashCode;

  factory StaffCreate.fromJson(Map<String, dynamic> json) => _$StaffCreateFromJson(json);

  Map<String, dynamic> toJson() => _$StaffCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum StaffCreateRoleEnum {
@JsonValue(r'BranchManager')
branchManager(r'BranchManager'),
@JsonValue(r'Staff')
staff(r'Staff'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const StaffCreateRoleEnum(this.value);

final String value;

@override
String toString() => value;
}


