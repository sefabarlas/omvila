//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'staff_summary.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class StaffSummary {
  /// Returns a new [StaffSummary] instance.
  StaffSummary({

    required  this.id,

    required  this.fullName,

     this.displayName,

     this.title,

     this.photoUrl,

    required  this.calendarColor,

    required  this.isBookableOnline,

    required  this.isActive,

    required  this.branchIds,

     this.role,

     this.membershipStatus,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



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
    
    name: r'photoUrl',
    required: false,
    includeIfNull: false,
  )


  final String? photoUrl;



  @JsonKey(
    
    name: r'calendarColor',
    required: true,
    includeIfNull: false,
  )


  final String calendarColor;



  @JsonKey(
    
    name: r'isBookableOnline',
    required: true,
    includeIfNull: false,
  )


  final bool isBookableOnline;



  @JsonKey(
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;



  @JsonKey(
    
    name: r'branchIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> branchIds;



  @JsonKey(
    
    name: r'role',
    required: false,
    includeIfNull: false,
  unknownEnumValue: StaffSummaryRoleEnum.unknownDefaultOpenApi,
  )


  final StaffSummaryRoleEnum? role;



  @JsonKey(
    
    name: r'membershipStatus',
    required: false,
    includeIfNull: false,
  unknownEnumValue: StaffSummaryMembershipStatusEnum.unknownDefaultOpenApi,
  )


  final StaffSummaryMembershipStatusEnum? membershipStatus;





    @override
    bool operator ==(Object other) => identical(this, other) || other is StaffSummary &&
      other.id == id &&
      other.fullName == fullName &&
      other.displayName == displayName &&
      other.title == title &&
      other.photoUrl == photoUrl &&
      other.calendarColor == calendarColor &&
      other.isBookableOnline == isBookableOnline &&
      other.isActive == isActive &&
      other.branchIds == branchIds &&
      other.role == role &&
      other.membershipStatus == membershipStatus;

    @override
    int get hashCode =>
        id.hashCode +
        fullName.hashCode +
        (displayName == null ? 0 : displayName.hashCode) +
        (title == null ? 0 : title.hashCode) +
        (photoUrl == null ? 0 : photoUrl.hashCode) +
        calendarColor.hashCode +
        isBookableOnline.hashCode +
        isActive.hashCode +
        branchIds.hashCode +
        (role == null ? 0 : role.hashCode) +
        (membershipStatus == null ? 0 : membershipStatus.hashCode);

  factory StaffSummary.fromJson(Map<String, dynamic> json) => _$StaffSummaryFromJson(json);

  Map<String, dynamic> toJson() => _$StaffSummaryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum StaffSummaryRoleEnum {
@JsonValue(r'Owner')
owner(r'Owner'),
@JsonValue(r'BranchManager')
branchManager(r'BranchManager'),
@JsonValue(r'Staff')
staff(r'Staff'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const StaffSummaryRoleEnum(this.value);

final String value;

@override
String toString() => value;
}


enum StaffSummaryMembershipStatusEnum {
@JsonValue(r'Invited')
invited(r'Invited'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Disabled')
disabled(r'Disabled'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const StaffSummaryMembershipStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


