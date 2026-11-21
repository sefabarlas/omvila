//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/package_definition.dart';
import 'package:omvila_api/src/model/package_overview.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'package_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageList {
  /// Returns a new [PackageList] instance.
  PackageList({

    required  this.items,

    required  this.overview,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<PackageDefinition> items;



  @JsonKey(
    
    name: r'overview',
    required: true,
    includeIfNull: false,
  )


  final PackageOverview overview;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageList &&
      other.items == items &&
      other.overview == overview;

    @override
    int get hashCode =>
        items.hashCode +
        overview.hashCode;

  factory PackageList.fromJson(Map<String, dynamic> json) => _$PackageListFromJson(json);

  Map<String, dynamic> toJson() => _$PackageListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

