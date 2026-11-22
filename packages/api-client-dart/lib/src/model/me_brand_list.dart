//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_brand.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_brand_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeBrandList {
  /// Returns a new [MeBrandList] instance.
  MeBrandList({

    required  this.items,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<MeBrand> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeBrandList &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory MeBrandList.fromJson(Map<String, dynamic> json) => _$MeBrandListFromJson(json);

  Map<String, dynamic> toJson() => _$MeBrandListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

