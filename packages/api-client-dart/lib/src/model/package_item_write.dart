//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'package_item_write.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageItemWrite {
  /// Returns a new [PackageItemWrite] instance.
  PackageItemWrite({

    required  this.serviceId,

    required  this.sessionCount,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



          // minimum: 1
          // maximum: 100
  @JsonKey(
    
    name: r'sessionCount',
    required: true,
    includeIfNull: false,
  )


  final int sessionCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageItemWrite &&
      other.serviceId == serviceId &&
      other.sessionCount == sessionCount;

    @override
    int get hashCode =>
        serviceId.hashCode +
        sessionCount.hashCode;

  factory PackageItemWrite.fromJson(Map<String, dynamic> json) => _$PackageItemWriteFromJson(json);

  Map<String, dynamic> toJson() => _$PackageItemWriteToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

