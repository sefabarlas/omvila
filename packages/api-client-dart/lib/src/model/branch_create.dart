//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'branch_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BranchCreate {
  /// Returns a new [BranchCreate] instance.
  BranchCreate({

    required  this.name,

    required  this.addressLine,

    required  this.district,

    required  this.city,

     this.phone,

     this.copyWorkingHoursFromBranchId,

     this.copyServicePricesFromBranchId,
  });

  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'addressLine',
    required: true,
    includeIfNull: false,
  )


  final String addressLine;



  @JsonKey(
    
    name: r'district',
    required: true,
    includeIfNull: false,
  )


  final String district;



  @JsonKey(
    
    name: r'city',
    required: true,
    includeIfNull: false,
  )


  final String city;



  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: true,
  )


  final String? phone;



      /// \"Çalışma saatleri: İşletme saatleriyle aynı\"
  @JsonKey(
    
    name: r'copyWorkingHoursFromBranchId',
    required: false,
    includeIfNull: true,
  )


  final String? copyWorkingHoursFromBranchId;



      /// \"Hizmet ve fiyatlar: Kadıköy'den kopyala\"
  @JsonKey(
    
    name: r'copyServicePricesFromBranchId',
    required: false,
    includeIfNull: true,
  )


  final String? copyServicePricesFromBranchId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BranchCreate &&
      other.name == name &&
      other.addressLine == addressLine &&
      other.district == district &&
      other.city == city &&
      other.phone == phone &&
      other.copyWorkingHoursFromBranchId == copyWorkingHoursFromBranchId &&
      other.copyServicePricesFromBranchId == copyServicePricesFromBranchId;

    @override
    int get hashCode =>
        name.hashCode +
        addressLine.hashCode +
        district.hashCode +
        city.hashCode +
        (phone == null ? 0 : phone.hashCode) +
        (copyWorkingHoursFromBranchId == null ? 0 : copyWorkingHoursFromBranchId.hashCode) +
        (copyServicePricesFromBranchId == null ? 0 : copyServicePricesFromBranchId.hashCode);

  factory BranchCreate.fromJson(Map<String, dynamic> json) => _$BranchCreateFromJson(json);

  Map<String, dynamic> toJson() => _$BranchCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

