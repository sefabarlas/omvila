//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_page_counts.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandPageCounts {
  /// Returns a new [AdminBrandPageCounts] instance.
  AdminBrandPageCounts({

    required  this.active,

    required  this.trialing,
  });

      /// Aktif marka
          // minimum: 0
  @JsonKey(
    
    name: r'active',
    required: true,
    includeIfNull: false,
  )


  final int active;



      /// Denemedeki marka
          // minimum: 0
  @JsonKey(
    
    name: r'trialing',
    required: true,
    includeIfNull: false,
  )


  final int trialing;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandPageCounts &&
      other.active == active &&
      other.trialing == trialing;

    @override
    int get hashCode =>
        active.hashCode +
        trialing.hashCode;

  factory AdminBrandPageCounts.fromJson(Map<String, dynamic> json) => _$AdminBrandPageCountsFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandPageCountsToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

