//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/deposit_rule.dart';
import 'package:omvila_api/src/model/service_stage.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'service_write.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ServiceWrite {
  /// Returns a new [ServiceWrite] instance.
  ServiceWrite({

    required  this.categoryId,

    required  this.name,

     this.description,

    required  this.durationMinutes,

    required  this.price,

    required  this.priceDisplay,

     this.bufferBeforeMinutes = 0,

     this.bufferAfterMinutes = 0,

    required  this.deposit,

     this.requiresApproval,

     this.minNoticeMinutes,

     this.maxAdvanceDays,

     this.resourceTypeId,

    required  this.isOnlineBookable,

     this.isActive = true,

     this.sortOrder,

     this.stages,

    required  this.staffIds,
  });

  @JsonKey(
    
    name: r'categoryId',
    required: true,
    includeIfNull: false,
  )


  final String categoryId;



  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'description',
    required: false,
    includeIfNull: false,
  )


  final String? description;



      /// Aşama varsa aşama toplamına eşit olmalı
          // minimum: 5
          // maximum: 720
  @JsonKey(
    
    name: r'durationMinutes',
    required: true,
    includeIfNull: false,
  )


  final int durationMinutes;



  @JsonKey(
    
    name: r'price',
    required: true,
    includeIfNull: false,
  )


  final Money price;



  @JsonKey(
    
    name: r'priceDisplay',
    required: true,
    includeIfNull: false,
  unknownEnumValue: ServiceWritePriceDisplayEnum.unknownDefaultOpenApi,
  )


  final ServiceWritePriceDisplayEnum priceDisplay;



          // minimum: 0
          // maximum: 120
  @JsonKey(
    defaultValue: 0,
    name: r'bufferBeforeMinutes',
    required: false,
    includeIfNull: false,
  )


  final int? bufferBeforeMinutes;



          // minimum: 0
          // maximum: 120
  @JsonKey(
    defaultValue: 0,
    name: r'bufferAfterMinutes',
    required: false,
    includeIfNull: false,
  )


  final int? bufferAfterMinutes;



  @JsonKey(
    
    name: r'deposit',
    required: true,
    includeIfNull: false,
  )


  final DepositRule deposit;



  @JsonKey(
    
    name: r'requiresApproval',
    required: false,
    includeIfNull: false,
  )


  final bool? requiresApproval;



          // minimum: 0
  @JsonKey(
    
    name: r'minNoticeMinutes',
    required: false,
    includeIfNull: false,
  )


  final int? minNoticeMinutes;



          // minimum: 1
          // maximum: 365
  @JsonKey(
    
    name: r'maxAdvanceDays',
    required: false,
    includeIfNull: false,
  )


  final int? maxAdvanceDays;



  @JsonKey(
    
    name: r'resourceTypeId',
    required: false,
    includeIfNull: false,
  )


  final String? resourceTypeId;



  @JsonKey(
    
    name: r'isOnlineBookable',
    required: true,
    includeIfNull: false,
  )


  final bool isOnlineBookable;



  @JsonKey(
    defaultValue: true,
    name: r'isActive',
    required: false,
    includeIfNull: false,
  )


  final bool? isActive;



  @JsonKey(
    
    name: r'sortOrder',
    required: false,
    includeIfNull: false,
  )


  final int? sortOrder;



  @JsonKey(
    
    name: r'stages',
    required: false,
    includeIfNull: false,
  )


  final List<ServiceStage>? stages;



  @JsonKey(
    
    name: r'staffIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> staffIds;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ServiceWrite &&
      other.categoryId == categoryId &&
      other.name == name &&
      other.description == description &&
      other.durationMinutes == durationMinutes &&
      other.price == price &&
      other.priceDisplay == priceDisplay &&
      other.bufferBeforeMinutes == bufferBeforeMinutes &&
      other.bufferAfterMinutes == bufferAfterMinutes &&
      other.deposit == deposit &&
      other.requiresApproval == requiresApproval &&
      other.minNoticeMinutes == minNoticeMinutes &&
      other.maxAdvanceDays == maxAdvanceDays &&
      other.resourceTypeId == resourceTypeId &&
      other.isOnlineBookable == isOnlineBookable &&
      other.isActive == isActive &&
      other.sortOrder == sortOrder &&
      other.stages == stages &&
      other.staffIds == staffIds;

    @override
    int get hashCode =>
        categoryId.hashCode +
        name.hashCode +
        (description == null ? 0 : description.hashCode) +
        durationMinutes.hashCode +
        price.hashCode +
        priceDisplay.hashCode +
        bufferBeforeMinutes.hashCode +
        bufferAfterMinutes.hashCode +
        deposit.hashCode +
        (requiresApproval == null ? 0 : requiresApproval.hashCode) +
        (minNoticeMinutes == null ? 0 : minNoticeMinutes.hashCode) +
        (maxAdvanceDays == null ? 0 : maxAdvanceDays.hashCode) +
        (resourceTypeId == null ? 0 : resourceTypeId.hashCode) +
        isOnlineBookable.hashCode +
        isActive.hashCode +
        sortOrder.hashCode +
        stages.hashCode +
        staffIds.hashCode;

  factory ServiceWrite.fromJson(Map<String, dynamic> json) => _$ServiceWriteFromJson(json);

  Map<String, dynamic> toJson() => _$ServiceWriteToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ServiceWritePriceDisplayEnum {
@JsonValue(r'Fixed')
fixed(r'Fixed'),
@JsonValue(r'StartingFrom')
startingFrom(r'StartingFrom'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ServiceWritePriceDisplayEnum(this.value);

final String value;

@override
String toString() => value;
}


