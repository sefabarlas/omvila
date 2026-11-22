//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_subscription_list_item.dart';
import 'package:omvila_api/src/model/admin_subscription_page_counts.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_subscription_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSubscriptionPage {
  /// Returns a new [AdminSubscriptionPage] instance.
  AdminSubscriptionPage({

    required  this.counts,

    required  this.items,

    required  this.nextCursor,
  });

  @JsonKey(
    
    name: r'counts',
    required: true,
    includeIfNull: false,
  )


  final AdminSubscriptionPageCounts counts;



      /// Aciliyete göre sıralı
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AdminSubscriptionListItem> items;



      /// Sonraki sayfa imleci
  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSubscriptionPage &&
      other.counts == counts &&
      other.items == items &&
      other.nextCursor == nextCursor;

    @override
    int get hashCode =>
        counts.hashCode +
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode);

  factory AdminSubscriptionPage.fromJson(Map<String, dynamic> json) => _$AdminSubscriptionPageFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSubscriptionPageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

