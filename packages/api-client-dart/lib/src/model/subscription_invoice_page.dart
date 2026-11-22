//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/subscription_invoice.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'subscription_invoice_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SubscriptionInvoicePage {
  /// Returns a new [SubscriptionInvoicePage] instance.
  SubscriptionInvoicePage({

    required  this.items,

    required  this.nextCursor,
  });

      /// Yeniden eskiye
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<SubscriptionInvoice> items;



      /// Sonraki sayfa imleci
  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SubscriptionInvoicePage &&
      other.items == items &&
      other.nextCursor == nextCursor;

    @override
    int get hashCode =>
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode);

  factory SubscriptionInvoicePage.fromJson(Map<String, dynamic> json) => _$SubscriptionInvoicePageFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionInvoicePageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

