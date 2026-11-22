//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/usable_package_item.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_list_my_packages200_response.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicListMyPackages200Response {
  /// Returns a new [PublicListMyPackages200Response] instance.
  PublicListMyPackages200Response({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<UsablePackageItem> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicListMyPackages200Response &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory PublicListMyPackages200Response.fromJson(Map<String, dynamic> json) => _$PublicListMyPackages200ResponseFromJson(json);

  Map<String, dynamic> toJson() => _$PublicListMyPackages200ResponseToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

