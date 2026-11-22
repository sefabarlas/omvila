//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/bank_transaction.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'bank_transaction_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BankTransactionPage {
  /// Returns a new [BankTransactionPage] instance.
  BankTransactionPage({

    required  this.items,

    required  this.nextCursor,
  });

      /// Değer tarihine göre yeniden eskiye
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<BankTransaction> items;



      /// Sonraki sayfa imleci
  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BankTransactionPage &&
      other.items == items &&
      other.nextCursor == nextCursor;

    @override
    int get hashCode =>
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode);

  factory BankTransactionPage.fromJson(Map<String, dynamic> json) => _$BankTransactionPageFromJson(json);

  Map<String, dynamic> toJson() => _$BankTransactionPageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

