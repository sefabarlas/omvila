//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'slot_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SlotItemsInner {
  /// Returns a new [SlotItemsInner] instance.
  SlotItemsInner({

    required  this.serviceId,

    required  this.staffId,

     this.staffName,

    required  this.startUtc,

    required  this.endUtc,

     this.price,
  });

  @JsonKey(
    
    name: r'serviceId',
    required: true,
    includeIfNull: false,
  )


  final String serviceId;



  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;



  @JsonKey(
    
    name: r'staffName',
    required: false,
    includeIfNull: false,
  )


  final String? staffName;



  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



  @JsonKey(
    
    name: r'price',
    required: false,
    includeIfNull: false,
  )


  final Money? price;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SlotItemsInner &&
      other.serviceId == serviceId &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.price == price;

    @override
    int get hashCode =>
        serviceId.hashCode +
        staffId.hashCode +
        staffName.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        price.hashCode;

  factory SlotItemsInner.fromJson(Map<String, dynamic> json) => _$SlotItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$SlotItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

