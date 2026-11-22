//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'package_refund_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageRefundRequest {
  /// Returns a new [PackageRefundRequest] instance.
  PackageRefundRequest({

    required  this.amount,

     this.note,
  });

      /// 0'dan büyük, ödenen tutardan küçük veya eşit
  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



      /// Salon içi not
  @JsonKey(
    
    name: r'note',
    required: false,
    includeIfNull: false,
  )


  final String? note;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageRefundRequest &&
      other.amount == amount &&
      other.note == note;

    @override
    int get hashCode =>
        amount.hashCode +
        (note == null ? 0 : note.hashCode);

  factory PackageRefundRequest.fromJson(Map<String, dynamic> json) => _$PackageRefundRequestFromJson(json);

  Map<String, dynamic> toJson() => _$PackageRefundRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

