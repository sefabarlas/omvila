//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_brand_package_badge.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeBrandPackageBadge {
  /// Returns a new [MeBrandPackageBadge] instance.
  MeBrandPackageBadge({

    required  this.customerPackageId,

    required  this.packageName,

    required  this.remainingSessions,
  });

  @JsonKey(
    
    name: r'customerPackageId',
    required: true,
    includeIfNull: false,
  )


  final String customerPackageId;



  @JsonKey(
    
    name: r'packageName',
    required: true,
    includeIfNull: false,
  )


  final String packageName;



          // minimum: 1
  @JsonKey(
    
    name: r'remainingSessions',
    required: true,
    includeIfNull: false,
  )


  final int remainingSessions;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeBrandPackageBadge &&
      other.customerPackageId == customerPackageId &&
      other.packageName == packageName &&
      other.remainingSessions == remainingSessions;

    @override
    int get hashCode =>
        customerPackageId.hashCode +
        packageName.hashCode +
        remainingSessions.hashCode;

  factory MeBrandPackageBadge.fromJson(Map<String, dynamic> json) => _$MeBrandPackageBadgeFromJson(json);

  Map<String, dynamic> toJson() => _$MeBrandPackageBadgeToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

