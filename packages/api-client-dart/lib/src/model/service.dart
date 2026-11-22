//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/deposit_rule.dart';
import 'package:omvila_api/src/model/service_stage.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'service.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class Service {
  /// Returns a new [Service] instance.
  Service({

    required  this.id,

    required  this.categoryId,

    required  this.name,

     this.description,

    required  this.durationMinutes,

    required  this.price,

    required  this.priceDisplay,

     this.isBranchPrice,

    required  this.bufferBeforeMinutes,

    required  this.bufferAfterMinutes,

    required  this.deposit,

     this.requiresApproval,

     this.minNoticeMinutes,

     this.maxAdvanceDays,

     this.resourceTypeId,

    required  this.isOnlineBookable,

    required  this.isActive,

     this.sortOrder,

    required  this.stages,

    required  this.staffIds,

    required  this.version,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



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
  unknownEnumValue: ServicePriceDisplayEnum.unknownDefaultOpenApi,
  )


  final ServicePriceDisplayEnum priceDisplay;



      /// Fiyat şubeye özel mi (branchId ile listelendiğinde)
  @JsonKey(
    
    name: r'isBranchPrice',
    required: false,
    includeIfNull: false,
  )


  final bool? isBranchPrice;



  @JsonKey(
    
    name: r'bufferBeforeMinutes',
    required: true,
    includeIfNull: false,
  )


  final int bufferBeforeMinutes;



  @JsonKey(
    
    name: r'bufferAfterMinutes',
    required: true,
    includeIfNull: false,
  )


  final int bufferAfterMinutes;



  @JsonKey(
    
    name: r'deposit',
    required: true,
    includeIfNull: false,
  )


  final DepositRule deposit;



      /// null = marka ayarı
  @JsonKey(
    
    name: r'requiresApproval',
    required: false,
    includeIfNull: false,
  )


  final bool? requiresApproval;



  @JsonKey(
    
    name: r'minNoticeMinutes',
    required: false,
    includeIfNull: false,
  )


  final int? minNoticeMinutes;



  @JsonKey(
    
    name: r'maxAdvanceDays',
    required: false,
    includeIfNull: false,
  )


  final int? maxAdvanceDays;



      /// Oda veya cihaz
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
    
    name: r'isActive',
    required: true,
    includeIfNull: false,
  )


  final bool isActive;



  @JsonKey(
    
    name: r'sortOrder',
    required: false,
    includeIfNull: false,
  )


  final int? sortOrder;



  @JsonKey(
    
    name: r'stages',
    required: true,
    includeIfNull: false,
  )


  final List<ServiceStage> stages;



  @JsonKey(
    
    name: r'staffIds',
    required: true,
    includeIfNull: false,
  )


  final List<String> staffIds;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is Service &&
      other.id == id &&
      other.categoryId == categoryId &&
      other.name == name &&
      other.description == description &&
      other.durationMinutes == durationMinutes &&
      other.price == price &&
      other.priceDisplay == priceDisplay &&
      other.isBranchPrice == isBranchPrice &&
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
      other.staffIds == staffIds &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        categoryId.hashCode +
        name.hashCode +
        (description == null ? 0 : description.hashCode) +
        durationMinutes.hashCode +
        price.hashCode +
        priceDisplay.hashCode +
        isBranchPrice.hashCode +
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
        staffIds.hashCode +
        version.hashCode;

  factory Service.fromJson(Map<String, dynamic> json) => _$ServiceFromJson(json);

  Map<String, dynamic> toJson() => _$ServiceToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

enum ServicePriceDisplayEnum {
@JsonValue(r'Fixed')
fixed(r'Fixed'),
@JsonValue(r'StartingFrom')
startingFrom(r'StartingFrom'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const ServicePriceDisplayEnum(this.value);

final String value;

@override
String toString() => value;
}


