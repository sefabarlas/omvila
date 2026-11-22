//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/credit_package.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'credit_package_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CreditPackageList {
  /// Returns a new [CreditPackageList] instance.
  CreditPackageList({

    required  this.items,
  });

      /// Kontöre göre artan
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<CreditPackage> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CreditPackageList &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory CreditPackageList.fromJson(Map<String, dynamic> json) => _$CreditPackageListFromJson(json);

  Map<String, dynamic> toJson() => _$CreditPackageListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

