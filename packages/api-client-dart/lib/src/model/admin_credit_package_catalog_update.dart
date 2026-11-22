//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_credit_package.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_credit_package_catalog_update.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminCreditPackageCatalogUpdate {
  /// Returns a new [AdminCreditPackageCatalogUpdate] instance.
  AdminCreditPackageCatalogUpdate({

    required  this.packages,
  });

      /// Düzenlenen paketler (code ile eşleşir)
  @JsonKey(
    
    name: r'packages',
    required: true,
    includeIfNull: false,
  )


  final List<AdminCreditPackage> packages;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminCreditPackageCatalogUpdate &&
      other.packages == packages;

    @override
    int get hashCode =>
        packages.hashCode;

  factory AdminCreditPackageCatalogUpdate.fromJson(Map<String, dynamic> json) => _$AdminCreditPackageCatalogUpdateFromJson(json);

  Map<String, dynamic> toJson() => _$AdminCreditPackageCatalogUpdateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

