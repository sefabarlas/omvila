//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_brand_page_counts.dart';
import 'package:omvila_api/src/model/admin_brand_list_item.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandPage {
  /// Returns a new [AdminBrandPage] instance.
  AdminBrandPage({

    required  this.counts,

    required  this.items,

    required  this.nextCursor,
  });

  @JsonKey(
    
    name: r'counts',
    required: true,
    includeIfNull: false,
  )


  final AdminBrandPageCounts counts;



      /// Ada göre sıralı
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AdminBrandListItem> items;



      /// Sonraki sayfa imleci
  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandPage &&
      other.counts == counts &&
      other.items == items &&
      other.nextCursor == nextCursor;

    @override
    int get hashCode =>
        counts.hashCode +
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode);

  factory AdminBrandPage.fromJson(Map<String, dynamic> json) => _$AdminBrandPageFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandPageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

