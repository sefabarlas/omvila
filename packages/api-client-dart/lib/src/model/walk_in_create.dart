//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'walk_in_create.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WalkInCreate {
  /// Returns a new [WalkInCreate] instance.
  WalkInCreate({

    required  this.branchId,

     this.brandCustomerId,

    required  this.fullName,

     this.phone,

     this.serviceId,

     this.preferredStaffId,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// Var olan müşteri seçildiyse
  @JsonKey(
    
    name: r'brandCustomerId',
    required: false,
    includeIfNull: false,
  )


  final String? brandCustomerId;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// İsteğe bağlı; girilirse sırası yaklaşınca SMS gider
  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: false,
  )


  final String? phone;



  @JsonKey(
    
    name: r'serviceId',
    required: false,
    includeIfNull: false,
  )


  final String? serviceId;



      /// null = fark etmez
  @JsonKey(
    
    name: r'preferredStaffId',
    required: false,
    includeIfNull: false,
  )


  final String? preferredStaffId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WalkInCreate &&
      other.branchId == branchId &&
      other.brandCustomerId == brandCustomerId &&
      other.fullName == fullName &&
      other.phone == phone &&
      other.serviceId == serviceId &&
      other.preferredStaffId == preferredStaffId;

    @override
    int get hashCode =>
        branchId.hashCode +
        brandCustomerId.hashCode +
        fullName.hashCode +
        phone.hashCode +
        (serviceId == null ? 0 : serviceId.hashCode) +
        (preferredStaffId == null ? 0 : preferredStaffId.hashCode);

  factory WalkInCreate.fromJson(Map<String, dynamic> json) => _$WalkInCreateFromJson(json);

  Map<String, dynamic> toJson() => _$WalkInCreateToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

