//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/commission_entry_status.dart';
import 'package:omvila_api/src/model/commission_entry_type.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'my_commission_entry.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MyCommissionEntry {
  /// Returns a new [MyCommissionEntry] instance.
  MyCommissionEntry({

    required  this.id,

    required  this.type,

    required  this.amountMinor,

    required  this.status,

    required  this.occurredUtc,

    required  this.customerName,

    required  this.description,

     this.appointmentId,

     this.customerPackageId,
  });

      /// Prim kaydı
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'type',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CommissionEntryType.unknownDefaultOpenApi,
  )


  final CommissionEntryType type;



      /// Tutar (kuruş); Reversed kayıtta eksi
  @JsonKey(
    
    name: r'amountMinor',
    required: true,
    includeIfNull: false,
  )


  final int amountMinor;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: CommissionEntryStatus.unknownDefaultOpenApi,
  )


  final CommissionEntryStatus status;



      /// Hizmet priminde randevu başlangıcı; satış priminde satış anı
  @JsonKey(
    
    name: r'occurredUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime occurredUtc;



      /// Müşteri adı
  @JsonKey(
    
    name: r'customerName',
    required: true,
    includeIfNull: false,
  )


  final String customerName;



      /// Hizmet adı veya paket adı
  @JsonKey(
    
    name: r'description',
    required: true,
    includeIfNull: false,
  )


  final String description;



      /// Hizmet priminde randevu
  @JsonKey(
    
    name: r'appointmentId',
    required: false,
    includeIfNull: false,
  )


  final String? appointmentId;



      /// Satış priminde müşteri paketi
  @JsonKey(
    
    name: r'customerPackageId',
    required: false,
    includeIfNull: false,
  )


  final String? customerPackageId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MyCommissionEntry &&
      other.id == id &&
      other.type == type &&
      other.amountMinor == amountMinor &&
      other.status == status &&
      other.occurredUtc == occurredUtc &&
      other.customerName == customerName &&
      other.description == description &&
      other.appointmentId == appointmentId &&
      other.customerPackageId == customerPackageId;

    @override
    int get hashCode =>
        id.hashCode +
        type.hashCode +
        amountMinor.hashCode +
        status.hashCode +
        occurredUtc.hashCode +
        customerName.hashCode +
        description.hashCode +
        (appointmentId == null ? 0 : appointmentId.hashCode) +
        (customerPackageId == null ? 0 : customerPackageId.hashCode);

  factory MyCommissionEntry.fromJson(Map<String, dynamic> json) => _$MyCommissionEntryFromJson(json);

  Map<String, dynamic> toJson() => _$MyCommissionEntryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

