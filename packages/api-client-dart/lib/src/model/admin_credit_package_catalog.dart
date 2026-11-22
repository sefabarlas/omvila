//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_credit_package.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_credit_package_catalog.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminCreditPackageCatalog {
  /// Returns a new [AdminCreditPackageCatalog] instance.
  AdminCreditPackageCatalog({

    required  this.packages,

    required  this.version,
  });

      /// Kontöre göre artan
  @JsonKey(
    
    name: r'packages',
    required: true,
    includeIfNull: false,
  )


  final List<AdminCreditPackage> packages;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminCreditPackageCatalog &&
      other.packages == packages &&
      other.version == version;

    @override
    int get hashCode =>
        packages.hashCode +
        version.hashCode;

  factory AdminCreditPackageCatalog.fromJson(Map<String, dynamic> json) => _$AdminCreditPackageCatalogFromJson(json);

  Map<String, dynamic> toJson() => _$AdminCreditPackageCatalogToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

