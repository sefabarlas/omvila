//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'usable_package_item.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class UsablePackageItem {
  /// Returns a new [UsablePackageItem] instance.
  UsablePackageItem({

    required  this.customerPackageItemId,

    required  this.serviceId,

    required  this.packageName,

    required  this.remainingSessions,

    required  this.totalSessions,

     this.expiresOn,
  });

  @JsonKey(
    
    name: r'customerPackageItemId',
    required: true,
    includeIfNull: false,
  )


  final String customerPackageItemId;



  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



  @JsonKey(
    
    name: r'packageName',
    required: true,
    includeIfNull: false,
  )


  final String packageName;



  @JsonKey(
    
    name: r'remainingSessions',
    required: true,
    includeIfNull: false,
  )


  final int remainingSessions;



  @JsonKey(
    
    name: r'totalSessions',
    required: true,
    includeIfNull: false,
  )


  final int totalSessions;



  @JsonKey(
    
    name: r'expiresOn',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? expiresOn;





    @override
    bool operator ==(Object other) => identical(this, other) || other is UsablePackageItem &&
      other.customerPackageItemId == customerPackageItemId &&
      other.serviceId == serviceId &&
      other.packageName == packageName &&
      other.remainingSessions == remainingSessions &&
      other.totalSessions == totalSessions &&
      other.expiresOn == expiresOn;

    @override
    int get hashCode =>
        customerPackageItemId.hashCode +
        serviceId.hashCode +
        packageName.hashCode +
        remainingSessions.hashCode +
        totalSessions.hashCode +
        (expiresOn == null ? 0 : expiresOn.hashCode);

  factory UsablePackageItem.fromJson(Map<String, dynamic> json) => _$UsablePackageItemFromJson(json);

  Map<String, dynamic> toJson() => _$UsablePackageItemToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

