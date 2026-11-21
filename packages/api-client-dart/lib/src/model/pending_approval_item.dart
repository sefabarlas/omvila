//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_channel.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'pending_approval_item.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PendingApprovalItem {
  /// Returns a new [PendingApprovalItem] instance.
  PendingApprovalItem({

    required  this.appointmentId,

     this.reference,

    required  this.branchId,

    required  this.branchName,

    required  this.timeZone,

    required  this.customerName,

    required  this.isNewCustomer,

    required  this.serviceSummary,

     this.staffName,

    required  this.startUtc,

    required  this.endUtc,

    required  this.holdExpiresUtc,

    required  this.createdUtc,

    required  this.channel,
  });

      /// Randevu kimliği (yan paneli açar)
  @JsonKey(
    
    name: r'appointmentId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentId;



      /// Kısa görünen numara
  @JsonKey(
    
    name: r'reference',
    required: false,
    includeIfNull: false,
  )


  final String? reference;



      /// Şube
  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// Şube adı (çok şubeli markada gösterilir)
  @JsonKey(
    
    name: r'branchName',
    required: true,
    includeIfNull: false,
  )


  final String branchName;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// Müşteri adı
  @JsonKey(
    
    name: r'customerName',
    required: true,
    includeIfNull: false,
  )


  final String customerName;



      /// İlk randevusu mu
  @JsonKey(
    
    name: r'isNewCustomer',
    required: true,
    includeIfNull: false,
  )


  final bool isNewCustomer;



      /// Hizmet adları; birden çok kalemde \"Saç kesimi + Fön\"
  @JsonKey(
    
    name: r'serviceSummary',
    required: true,
    includeIfNull: false,
  )


  final String serviceSummary;



      /// İlk kalemin personeli
  @JsonKey(
    
    name: r'staffName',
    required: false,
    includeIfNull: false,
  )


  final String? staffName;



      /// Randevu başlangıcı
  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



      /// Randevu bitişi
  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



      /// Bu anda yanıtlanmamışsa otomatik iptal olur (A5)
  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime holdExpiresUtc;



      /// Talebin geldiği an
  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;



  @JsonKey(
    
    name: r'channel',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AppointmentChannel.unknownDefaultOpenApi,
  )


  final AppointmentChannel channel;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PendingApprovalItem &&
      other.appointmentId == appointmentId &&
      other.reference == reference &&
      other.branchId == branchId &&
      other.branchName == branchName &&
      other.timeZone == timeZone &&
      other.customerName == customerName &&
      other.isNewCustomer == isNewCustomer &&
      other.serviceSummary == serviceSummary &&
      other.staffName == staffName &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.createdUtc == createdUtc &&
      other.channel == channel;

    @override
    int get hashCode =>
        appointmentId.hashCode +
        reference.hashCode +
        branchId.hashCode +
        branchName.hashCode +
        timeZone.hashCode +
        customerName.hashCode +
        isNewCustomer.hashCode +
        serviceSummary.hashCode +
        (staffName == null ? 0 : staffName.hashCode) +
        startUtc.hashCode +
        endUtc.hashCode +
        holdExpiresUtc.hashCode +
        createdUtc.hashCode +
        channel.hashCode;

  factory PendingApprovalItem.fromJson(Map<String, dynamic> json) => _$PendingApprovalItemFromJson(json);

  Map<String, dynamic> toJson() => _$PendingApprovalItemToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

