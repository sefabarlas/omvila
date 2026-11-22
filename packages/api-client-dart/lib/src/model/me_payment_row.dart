//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_brand_ref.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/payment_method.dart';
import 'package:omvila_api/src/model/me_payment_kind.dart';
import 'package:omvila_api/src/model/me_payment_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_payment_row.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MePaymentRow {
  /// Returns a new [MePaymentRow] instance.
  MePaymentRow({

    required  this.id,

    required  this.kind,

    required  this.status,

    required  this.brand,

    required  this.amount,

    required  this.method,

    required  this.occurredUtc,

    required  this.timeZone,

     this.refundedAmount,

     this.appointmentId,

     this.appointmentStartUtc,

     this.serviceNames,

     this.packageName,

     this.packageSessionCount,
  });

      /// Tahsilat kaydı; AwaitingConfirmation satırında randevu kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'kind',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MePaymentKind.unknownDefaultOpenApi,
  )


  final MePaymentKind kind;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: MePaymentStatus.unknownDefaultOpenApi,
  )


  final MePaymentStatus status;



  @JsonKey(
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final MeBrandRef brand;



  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



  @JsonKey(
    
    name: r'method',
    required: true,
    includeIfNull: false,
  unknownEnumValue: PaymentMethod.unknownDefaultOpenApi,
  )


  final PaymentMethod method;



      /// Tahsilat anı; AwaitingConfirmation satırında müşterinin bildirdiği an
  @JsonKey(
    
    name: r'occurredUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime occurredUtc;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// Yalnız iade işaretlendiyse
  @JsonKey(
    
    name: r'refundedAmount',
    required: false,
    includeIfNull: false,
  )


  final Money? refundedAmount;



      /// Kapora ve randevu ödemesinde randevu
  @JsonKey(
    
    name: r'appointmentId',
    required: false,
    includeIfNull: false,
  )


  final String? appointmentId;



      /// \"Alındı · 26 Kasım randevusu\"
  @JsonKey(
    
    name: r'appointmentStartUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? appointmentStartUtc;



      /// Randevu ödemesinde hizmetler (\"Boya ve fön\")
  @JsonKey(
    
    name: r'serviceNames',
    required: false,
    includeIfNull: false,
  )


  final List<String>? serviceNames;



      /// Paket satın almada
  @JsonKey(
    
    name: r'packageName',
    required: false,
    includeIfNull: false,
  )


  final String? packageName;



      /// \"5 seans\"
  @JsonKey(
    
    name: r'packageSessionCount',
    required: false,
    includeIfNull: false,
  )


  final int? packageSessionCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MePaymentRow &&
      other.id == id &&
      other.kind == kind &&
      other.status == status &&
      other.brand == brand &&
      other.amount == amount &&
      other.method == method &&
      other.occurredUtc == occurredUtc &&
      other.timeZone == timeZone &&
      other.refundedAmount == refundedAmount &&
      other.appointmentId == appointmentId &&
      other.appointmentStartUtc == appointmentStartUtc &&
      other.serviceNames == serviceNames &&
      other.packageName == packageName &&
      other.packageSessionCount == packageSessionCount;

    @override
    int get hashCode =>
        id.hashCode +
        kind.hashCode +
        status.hashCode +
        brand.hashCode +
        amount.hashCode +
        method.hashCode +
        occurredUtc.hashCode +
        timeZone.hashCode +
        refundedAmount.hashCode +
        (appointmentId == null ? 0 : appointmentId.hashCode) +
        (appointmentStartUtc == null ? 0 : appointmentStartUtc.hashCode) +
        serviceNames.hashCode +
        (packageName == null ? 0 : packageName.hashCode) +
        (packageSessionCount == null ? 0 : packageSessionCount.hashCode);

  factory MePaymentRow.fromJson(Map<String, dynamic> json) => _$MePaymentRowFromJson(json);

  Map<String, dynamic> toJson() => _$MePaymentRowToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

