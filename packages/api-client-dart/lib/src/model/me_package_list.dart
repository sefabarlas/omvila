//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_package.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_package_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MePackageList {
  /// Returns a new [MePackageList] instance.
  MePackageList({

    required  this.activeCount,

    required  this.items,
  });

      /// \"2 aktif paket\"
          // minimum: 0
  @JsonKey(
    
    name: r'activeCount',
    required: true,
    includeIfNull: false,
  )


  final int activeCount;



      /// Önce kullanılabilirler, sonra bitenler
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<MePackage> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MePackageList &&
      other.activeCount == activeCount &&
      other.items == items;

    @override
    int get hashCode =>
        activeCount.hashCode +
        items.hashCode;

  factory MePackageList.fromJson(Map<String, dynamic> json) => _$MePackageListFromJson(json);

  Map<String, dynamic> toJson() => _$MePackageListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

