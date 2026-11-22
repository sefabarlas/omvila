//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_overview_active_brands.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminOverviewActiveBrands {
  /// Returns a new [AdminOverviewActiveBrands] instance.
  AdminOverviewActiveBrands({

    required  this.count,

    required  this.newThisMonth,
  });

      /// Aktif marka
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// Bu ay kaydolan
          // minimum: 0
  @JsonKey(
    
    name: r'newThisMonth',
    required: true,
    includeIfNull: false,
  )


  final int newThisMonth;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminOverviewActiveBrands &&
      other.count == count &&
      other.newThisMonth == newThisMonth;

    @override
    int get hashCode =>
        count.hashCode +
        newThisMonth.hashCode;

  factory AdminOverviewActiveBrands.fromJson(Map<String, dynamic> json) => _$AdminOverviewActiveBrandsFromJson(json);

  Map<String, dynamic> toJson() => _$AdminOverviewActiveBrandsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

