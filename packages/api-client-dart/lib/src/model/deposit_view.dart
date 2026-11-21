//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/deposit_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deposit_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class DepositView {
  /// Returns a new [DepositView] instance.
  DepositView({

    required  this.required_,

    required  this.status,

     this.transferCode,

     this.customerReportedUtc,

     this.receivedUtc,

     this.remindedUtc,

     this.refundedUtc,

     this.refundedAmount,
  });

  @JsonKey(
    
    name: r'required',
    required: true,
    includeIfNull: false,
  )


  final Money required_;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DepositStatus.unknownDefaultOpenApi,
  )


  final DepositStatus status;



  @JsonKey(
    
    name: r'transferCode',
    required: false,
    includeIfNull: false,
  )


  final String? transferCode;



  @JsonKey(
    
    name: r'customerReportedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? customerReportedUtc;



  @JsonKey(
    
    name: r'receivedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? receivedUtc;



      /// Salonun \"Hatırlat\" ile kapora SMS'ini yeniden gönderdiği an (randevu başına en fazla bir kez); doluysa buton pasif
  @JsonKey(
    
    name: r'remindedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? remindedUtc;



      /// \"Salon iade etti\" işaretlendiği an
  @JsonKey(
    
    name: r'refundedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? refundedUtc;



      /// İade edildi olarak işaretlenen tutar (alınan kaporayı aşmaz; kısmi olabilir)
  @JsonKey(
    
    name: r'refundedAmount',
    required: false,
    includeIfNull: false,
  )


  final Money? refundedAmount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is DepositView &&
      other.required_ == required_ &&
      other.status == status &&
      other.transferCode == transferCode &&
      other.customerReportedUtc == customerReportedUtc &&
      other.receivedUtc == receivedUtc &&
      other.remindedUtc == remindedUtc &&
      other.refundedUtc == refundedUtc &&
      other.refundedAmount == refundedAmount;

    @override
    int get hashCode =>
        required_.hashCode +
        status.hashCode +
        transferCode.hashCode +
        (customerReportedUtc == null ? 0 : customerReportedUtc.hashCode) +
        (receivedUtc == null ? 0 : receivedUtc.hashCode) +
        (remindedUtc == null ? 0 : remindedUtc.hashCode) +
        (refundedUtc == null ? 0 : refundedUtc.hashCode) +
        refundedAmount.hashCode;

  factory DepositView.fromJson(Map<String, dynamic> json) => _$DepositViewFromJson(json);

  Map<String, dynamic> toJson() => _$DepositViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

