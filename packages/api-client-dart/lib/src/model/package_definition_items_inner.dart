//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'package_definition_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageDefinitionItemsInner {
  /// Returns a new [PackageDefinitionItemsInner] instance.
  PackageDefinitionItemsInner({

    required  this.serviceId,

    required  this.serviceName,

    required  this.sessionCount,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



  @JsonKey(
    
    name: r'serviceName',
    required: true,
    includeIfNull: false,
  )


  final String serviceName;



  @JsonKey(
    
    name: r'sessionCount',
    required: true,
    includeIfNull: false,
  )


  final int sessionCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageDefinitionItemsInner &&
      other.serviceId == serviceId &&
      other.serviceName == serviceName &&
      other.sessionCount == sessionCount;

    @override
    int get hashCode =>
        serviceId.hashCode +
        serviceName.hashCode +
        sessionCount.hashCode;

  factory PackageDefinitionItemsInner.fromJson(Map<String, dynamic> json) => _$PackageDefinitionItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$PackageDefinitionItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

