//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'customer_deletion_preview_active_packages_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerDeletionPreviewActivePackagesInner {
  /// Returns a new [CustomerDeletionPreviewActivePackagesInner] instance.
  CustomerDeletionPreviewActivePackagesInner({

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



  @JsonKey(
    
    name: r'remainingSessions',
    required: true,
    includeIfNull: false,
  )


  final int remainingSessions;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerDeletionPreviewActivePackagesInner &&
      other.customerPackageId == customerPackageId &&
      other.packageName == packageName &&
      other.remainingSessions == remainingSessions;

    @override
    int get hashCode =>
        customerPackageId.hashCode +
        packageName.hashCode +
        remainingSessions.hashCode;

  factory CustomerDeletionPreviewActivePackagesInner.fromJson(Map<String, dynamic> json) => _$CustomerDeletionPreviewActivePackagesInnerFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerDeletionPreviewActivePackagesInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

