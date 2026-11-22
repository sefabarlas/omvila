//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/subscription_invoice_status.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/subscription_payment_method.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'subscription_invoice.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SubscriptionInvoice {
  /// Returns a new [SubscriptionInvoice] instance.
  SubscriptionInvoice({

    required  this.id,

    required  this.invoiceNumber,

    required  this.periodStart,

    required  this.periodEnd,

    required  this.amount,

    required  this.tax,

    required  this.total,

    required  this.status,

    required  this.issuedUtc,

    required  this.paidUtc,

    required  this.paidVia,
  });

      /// Fatura kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Boşluksuz fatura numarası (\"No\")
  @JsonKey(
    
    name: r'invoiceNumber',
    required: true,
    includeIfNull: false,
  )


  final String invoiceNumber;



      /// Dönem başı (\"Dönem\")
  @JsonKey(
    
    name: r'periodStart',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime periodStart;



      /// Dönem sonu
  @JsonKey(
    
    name: r'periodEnd',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime periodEnd;



      /// KDV hariç tutar
  @JsonKey(
    
    name: r'amount',
    required: true,
    includeIfNull: false,
  )


  final Money amount;



      /// KDV
  @JsonKey(
    
    name: r'tax',
    required: true,
    includeIfNull: false,
  )


  final Money tax;



      /// Toplam (\"Tutar\")
  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final Money total;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: SubscriptionInvoiceStatus.unknownDefaultOpenApi,
  )


  final SubscriptionInvoiceStatus status;



      /// Kesildiği an
  @JsonKey(
    
    name: r'issuedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime issuedUtc;



      /// Ödendiği an
  @JsonKey(
    
    name: r'paidUtc',
    required: true,
    includeIfNull: true,
  )


  final DateTime? paidUtc;



      /// Ödeme yolu; ödenmediyse null
  @JsonKey(
    
    name: r'paidVia',
    required: true,
    includeIfNull: true,
  unknownEnumValue: SubscriptionPaymentMethod.unknownDefaultOpenApi,
  )


  final SubscriptionPaymentMethod? paidVia;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SubscriptionInvoice &&
      other.id == id &&
      other.invoiceNumber == invoiceNumber &&
      other.periodStart == periodStart &&
      other.periodEnd == periodEnd &&
      other.amount == amount &&
      other.tax == tax &&
      other.total == total &&
      other.status == status &&
      other.issuedUtc == issuedUtc &&
      other.paidUtc == paidUtc &&
      other.paidVia == paidVia;

    @override
    int get hashCode =>
        id.hashCode +
        invoiceNumber.hashCode +
        periodStart.hashCode +
        periodEnd.hashCode +
        amount.hashCode +
        tax.hashCode +
        total.hashCode +
        status.hashCode +
        issuedUtc.hashCode +
        (paidUtc == null ? 0 : paidUtc.hashCode) +
        (paidVia == null ? 0 : paidVia.hashCode);

  factory SubscriptionInvoice.fromJson(Map<String, dynamic> json) => _$SubscriptionInvoiceFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionInvoiceToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

