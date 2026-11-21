//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/walk_in_ticket_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'walk_in_ticket_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WalkInTicketView {
  /// Returns a new [WalkInTicketView] instance.
  WalkInTicketView({

    required  this.id,

    required  this.branchId,

    required  this.queueDate,

    required  this.queueNumber,

    required  this.status,

    required  this.customerName,

     this.brandCustomerId,

     this.phoneMasked,

     this.serviceId,

     this.serviceName,

     this.preferredStaffId,

     this.preferredStaffName,

    required  this.joinedUtc,

     this.estimatedWaitMinutes,

     this.appointmentId,

    required  this.version,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// Şubenin yerel günü
  @JsonKey(
    
    name: r'queueDate',
    required: true,
    includeIfNull: false,
  )


  final DateTime queueDate;



      /// Şube + gün bazında 1'den
  @JsonKey(
    
    name: r'queueNumber',
    required: true,
    includeIfNull: false,
  )


  final int queueNumber;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WalkInTicketStatus.unknownDefaultOpenApi,
  )


  final WalkInTicketStatus status;



  @JsonKey(
    
    name: r'customerName',
    required: true,
    includeIfNull: false,
  )


  final String customerName;



      /// Var olan müşteri seçildiyse veya bilet randevuya çevrildiyse
  @JsonKey(
    
    name: r'brandCustomerId',
    required: false,
    includeIfNull: false,
  )


  final String? brandCustomerId;



      /// Telefon verildiyse (\"sıra gelince SMS\")
  @JsonKey(
    
    name: r'phoneMasked',
    required: false,
    includeIfNull: false,
  )


  final String? phoneMasked;



  @JsonKey(
    
    name: r'serviceId',
    required: false,
    includeIfNull: false,
  )


  final String? serviceId;



  @JsonKey(
    
    name: r'serviceName',
    required: false,
    includeIfNull: false,
  )


  final String? serviceName;



  @JsonKey(
    
    name: r'preferredStaffId',
    required: false,
    includeIfNull: false,
  )


  final String? preferredStaffId;



      /// null ise \"Fark etmez\"
  @JsonKey(
    
    name: r'preferredStaffName',
    required: false,
    includeIfNull: false,
  )


  final String? preferredStaffName;



      /// \"Bekliyor 12 dk\"
  @JsonKey(
    
    name: r'joinedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime joinedUtc;



      /// Bu kişinin tahmini beklemesi (öndekilerin kalan süreleri / müsait personel)
  @JsonKey(
    
    name: r'estimatedWaitMinutes',
    required: false,
    includeIfNull: false,
  )


  final int? estimatedWaitMinutes;



      /// Randevuya çevrildiyse
  @JsonKey(
    
    name: r'appointmentId',
    required: false,
    includeIfNull: false,
  )


  final String? appointmentId;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WalkInTicketView &&
      other.id == id &&
      other.branchId == branchId &&
      other.queueDate == queueDate &&
      other.queueNumber == queueNumber &&
      other.status == status &&
      other.customerName == customerName &&
      other.brandCustomerId == brandCustomerId &&
      other.phoneMasked == phoneMasked &&
      other.serviceId == serviceId &&
      other.serviceName == serviceName &&
      other.preferredStaffId == preferredStaffId &&
      other.preferredStaffName == preferredStaffName &&
      other.joinedUtc == joinedUtc &&
      other.estimatedWaitMinutes == estimatedWaitMinutes &&
      other.appointmentId == appointmentId &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        branchId.hashCode +
        queueDate.hashCode +
        queueNumber.hashCode +
        status.hashCode +
        customerName.hashCode +
        (brandCustomerId == null ? 0 : brandCustomerId.hashCode) +
        (phoneMasked == null ? 0 : phoneMasked.hashCode) +
        (serviceId == null ? 0 : serviceId.hashCode) +
        (serviceName == null ? 0 : serviceName.hashCode) +
        (preferredStaffId == null ? 0 : preferredStaffId.hashCode) +
        (preferredStaffName == null ? 0 : preferredStaffName.hashCode) +
        joinedUtc.hashCode +
        (estimatedWaitMinutes == null ? 0 : estimatedWaitMinutes.hashCode) +
        (appointmentId == null ? 0 : appointmentId.hashCode) +
        version.hashCode;

  factory WalkInTicketView.fromJson(Map<String, dynamic> json) => _$WalkInTicketViewFromJson(json);

  Map<String, dynamic> toJson() => _$WalkInTicketViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

