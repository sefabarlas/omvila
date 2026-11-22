//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/admin_credit_transaction.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_credit_transaction_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminCreditTransactionPage {
  /// Returns a new [AdminCreditTransactionPage] instance.
  AdminCreditTransactionPage({

    required  this.items,

    required  this.nextCursor,
  });

      /// Yeniden eskiye
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<AdminCreditTransaction> items;



      /// Sonraki sayfa imleci
  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminCreditTransactionPage &&
      other.items == items &&
      other.nextCursor == nextCursor;

    @override
    int get hashCode =>
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode);

  factory AdminCreditTransactionPage.fromJson(Map<String, dynamic> json) => _$AdminCreditTransactionPageFromJson(json);

  Map<String, dynamic> toJson() => _$AdminCreditTransactionPageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

