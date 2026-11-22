//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/role_access_level.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'role_matrix_rows_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class RoleMatrixRowsInner {
  /// Returns a new [RoleMatrixRowsInner] instance.
  RoleMatrixRowsInner({

    required  this.group,

    required  this.permissions,

    required  this.owner,

    required  this.branchManager,

    required  this.staff,
  });

      /// Satır (Takvim ve randevu, Müşteri telefonu, Müşteri kartı ve notlar, Kasa ve ödeme alma, Raporlar ve prim, Hizmet ve fiyat düzenleme, Ayarlar)
  @JsonKey(
    
    name: r'group',
    required: true,
    includeIfNull: false,
  unknownEnumValue: RoleMatrixRowsInnerGroupEnum.unknownDefaultOpenApi,
  )


  final RoleMatrixRowsInnerGroupEnum group;



      /// Satırın kapsadığı izin kodları
  @JsonKey(
    
    name: r'permissions',
    required: true,
    includeIfNull: false,
  )


  final List<String> permissions;



  @JsonKey(
    
    name: r'owner',
    required: true,
    includeIfNull: false,
  unknownEnumValue: RoleAccessLevel.unknownDefaultOpenApi,
  )


  final RoleAccessLevel owner;



  @JsonKey(
    
    name: r'branchManager',
    required: true,
    includeIfNull: false,
  unknownEnumValue: RoleAccessLevel.unknownDefaultOpenApi,
  )


  final RoleAccessLevel branchManager;



  @JsonKey(
    
    name: r'staff',
    required: true,
    includeIfNull: false,
  unknownEnumValue: RoleAccessLevel.unknownDefaultOpenApi,
  )


  final RoleAccessLevel staff;





    @override
    bool operator ==(Object other) => identical(this, other) || other is RoleMatrixRowsInner &&
      other.group == group &&
      other.permissions == permissions &&
      other.owner == owner &&
      other.branchManager == branchManager &&
      other.staff == staff;

    @override
    int get hashCode =>
        group.hashCode +
        permissions.hashCode +
        owner.hashCode +
        branchManager.hashCode +
        staff.hashCode;

  factory RoleMatrixRowsInner.fromJson(Map<String, dynamic> json) => _$RoleMatrixRowsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$RoleMatrixRowsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Satır (Takvim ve randevu, Müşteri telefonu, Müşteri kartı ve notlar, Kasa ve ödeme alma, Raporlar ve prim, Hizmet ve fiyat düzenleme, Ayarlar)
enum RoleMatrixRowsInnerGroupEnum {
@JsonValue(r'CalendarAndAppointments')
calendarAndAppointments(r'CalendarAndAppointments'),
@JsonValue(r'CustomerPhone')
customerPhone(r'CustomerPhone'),
@JsonValue(r'CustomerCardAndNotes')
customerCardAndNotes(r'CustomerCardAndNotes'),
@JsonValue(r'CashAndPayments')
cashAndPayments(r'CashAndPayments'),
@JsonValue(r'ReportsAndCommission')
reportsAndCommission(r'ReportsAndCommission'),
@JsonValue(r'ServicesAndPrices')
servicesAndPrices(r'ServicesAndPrices'),
@JsonValue(r'Settings')
settings(r'Settings'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const RoleMatrixRowsInnerGroupEnum(this.value);

final String value;

@override
String toString() => value;
}


