//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/staff_detail_all_of_working_hours.dart';
import 'package:omvila_api/src/model/staff_service_assignment.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'staff_detail.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class StaffDetail {
  /// Returns a new [StaffDetail] instance.
  StaffDetail({

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

     this.phoneMasked,

     this.bio,

    required  this.serviceCommissionPercent,

    required  this.saleCommissionPercent,

    required  this.services,

    required  this.workingHours,

     this.googleCalendarStatus,

    required  this.version,
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
  unknownEnumValue: StaffDetailRoleEnum.unknownDefaultOpenApi,
  )


  final StaffDetailRoleEnum? role;



  @JsonKey(
    
    name: r'membershipStatus',
    required: false,
    includeIfNull: false,
  unknownEnumValue: StaffDetailMembershipStatusEnum.unknownDefaultOpenApi,
  )


  final StaffDetailMembershipStatusEnum? membershipStatus;



  @JsonKey(
    
    name: r'phoneMasked',
    required: false,
    includeIfNull: false,
  )


  final String? phoneMasked;



  @JsonKey(
    
    name: r'bio',
    required: false,
    includeIfNull: false,
  )


  final String? bio;



  @JsonKey(
    
    name: r'serviceCommissionPercent',
    required: true,
    includeIfNull: false,
  )


  final num serviceCommissionPercent;



  @JsonKey(
    
    name: r'saleCommissionPercent',
    required: true,
    includeIfNull: false,
  )


  final num saleCommissionPercent;



  @JsonKey(
    
    name: r'services',
    required: true,
    includeIfNull: false,
  )


  final List<StaffServiceAssignment> services;



  @JsonKey(
    
    name: r'workingHours',
    required: true,
    includeIfNull: false,
  )


  final List<StaffDetailAllOfWorkingHours> workingHours;



  @JsonKey(
    
    name: r'googleCalendarStatus',
    required: false,
    includeIfNull: false,
  unknownEnumValue: StaffDetailGoogleCalendarStatusEnum.unknownDefaultOpenApi,
  )


  final StaffDetailGoogleCalendarStatusEnum? googleCalendarStatus;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is StaffDetail &&
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
      other.membershipStatus == membershipStatus &&
      other.phoneMasked == phoneMasked &&
      other.bio == bio &&
      other.serviceCommissionPercent == serviceCommissionPercent &&
      other.saleCommissionPercent == saleCommissionPercent &&
      other.services == services &&
      other.workingHours == workingHours &&
      other.googleCalendarStatus == googleCalendarStatus &&
      other.version == version;

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
        (membershipStatus == null ? 0 : membershipStatus.hashCode) +
        (phoneMasked == null ? 0 : phoneMasked.hashCode) +
        (bio == null ? 0 : bio.hashCode) +
        serviceCommissionPercent.hashCode +
        saleCommissionPercent.hashCode +
        services.hashCode +
        workingHours.hashCode +
        (googleCalendarStatus == null ? 0 : googleCalendarStatus.hashCode) +
        version.hashCode;

  factory StaffDetail.fromJson(Map<String, dynamic> json) => _$StaffDetailFromJson(json);

  Map<String, dynamic> toJson() => _$StaffDetailToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum StaffDetailRoleEnum {
@JsonValue(r'Owner')
owner(r'Owner'),
@JsonValue(r'BranchManager')
branchManager(r'BranchManager'),
@JsonValue(r'Staff')
staff(r'Staff'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const StaffDetailRoleEnum(this.value);

final String value;

@override
String toString() => value;
}


enum StaffDetailMembershipStatusEnum {
@JsonValue(r'Invited')
invited(r'Invited'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Disabled')
disabled(r'Disabled'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const StaffDetailMembershipStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


enum StaffDetailGoogleCalendarStatusEnum {
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'TokenExpired')
tokenExpired(r'TokenExpired'),
@JsonValue(r'Revoked')
revoked(r'Revoked'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const StaffDetailGoogleCalendarStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


