//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'staff_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class StaffUpdate {
  /// Returns a new [StaffUpdate] instance.
  StaffUpdate({

     this.fullName,

     this.displayName,

     this.title,

     this.bio,

     this.role,

     this.calendarColor,

     this.branchIds,

     this.serviceCommissionPercent,

     this.saleCommissionPercent,

     this.isBookableOnline,

     this.isActive,
  });

  @JsonKey(
    
    name: r'fullName',
    required: false,
    includeIfNull: false,
  )


  final String? fullName;



  @JsonKey(
    
    name: r'displayName',
    required: false,
    includeIfNull: false,
  )


  final String? displayName;



  @JsonKey(
    
    name: r'title',
    required: false,
    includeIfNull: false,
  )


  final String? title;



  @JsonKey(
    
    name: r'bio',
    required: false,
    includeIfNull: false,
  )


  final String? bio;



  @JsonKey(
    
    name: r'role',
    required: false,
    includeIfNull: false,
  unknownEnumValue: StaffUpdateRoleEnum.unknownDefaultOpenApi,
  )


  final StaffUpdateRoleEnum? role;



  @JsonKey(
    
    name: r'calendarColor',
    required: false,
    includeIfNull: false,
  )


  final String? calendarColor;



  @JsonKey(
    
    name: r'branchIds',
    required: false,
    includeIfNull: false,
  )


  final List<String>? branchIds;



          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'serviceCommissionPercent',
    required: false,
    includeIfNull: false,
  )


  final num? serviceCommissionPercent;



          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'saleCommissionPercent',
    required: false,
    includeIfNull: false,
  )


  final num? saleCommissionPercent;



  @JsonKey(
    
    name: r'isBookableOnline',
    required: false,
    includeIfNull: false,
  )


  final bool? isBookableOnline;



  @JsonKey(
    
    name: r'isActive',
    required: false,
    includeIfNull: false,
  )


  final bool? isActive;





    @override
    bool operator ==(Object other) => identical(this, other) || other is StaffUpdate &&
      other.fullName == fullName &&
      other.displayName == displayName &&
      other.title == title &&
      other.bio == bio &&
      other.role == role &&
      other.calendarColor == calendarColor &&
      other.branchIds == branchIds &&
      other.serviceCommissionPercent == serviceCommissionPercent &&
      other.saleCommissionPercent == saleCommissionPercent &&
      other.isBookableOnline == isBookableOnline &&
      other.isActive == isActive;

    @override
    int get hashCode =>
        fullName.hashCode +
        (displayName == null ? 0 : displayName.hashCode) +
        (title == null ? 0 : title.hashCode) +
        (bio == null ? 0 : bio.hashCode) +
        role.hashCode +
        calendarColor.hashCode +
        branchIds.hashCode +
        serviceCommissionPercent.hashCode +
        saleCommissionPercent.hashCode +
        isBookableOnline.hashCode +
        isActive.hashCode;

  factory StaffUpdate.fromJson(Map<String, dynamic> json) => _$StaffUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$StaffUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum StaffUpdateRoleEnum {
@JsonValue(r'BranchManager')
branchManager(r'BranchManager'),
@JsonValue(r'Staff')
staff(r'Staff'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const StaffUpdateRoleEnum(this.value);

final String value;

@override
String toString() => value;
}


