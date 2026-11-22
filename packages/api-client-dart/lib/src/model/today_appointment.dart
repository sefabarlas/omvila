//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/appointment_status.dart';
import 'package:omvila_api/src/model/deposit_status.dart';
import 'package:omvila_api/src/model/display_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'today_appointment.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class TodayAppointment {
  /// Returns a new [TodayAppointment] instance.
  TodayAppointment({

    required  this.appointmentId,

    required  this.startUtc,

    required  this.endUtc,

    required  this.status,

    required  this.displayStatus,

    required  this.customerName,

    required  this.isNewCustomer,

    required  this.serviceSummary,

    required  this.staffNames,

     this.holdExpiresUtc,

     this.depositStatus,
  });

      /// Randevu kimliği (RandevuDetay)
  @JsonKey(
    
    name: r'appointmentId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentId;



      /// Başlangıç
  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



      /// Bitiş; \"45 dk\" süre istemcide
  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AppointmentStatus.unknownDefaultOpenApi,
  )


  final AppointmentStatus status;



  @JsonKey(
    
    name: r'displayStatus',
    required: true,
    includeIfNull: false,
  unknownEnumValue: DisplayStatus.unknownDefaultOpenApi,
  )


  final DisplayStatus displayStatus;



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



      /// Hizmet adları; birden çok kalemde \"Boya + Fön\"
  @JsonKey(
    
    name: r'serviceSummary',
    required: true,
    includeIfNull: false,
  )


  final String serviceSummary;



      /// Kalemlerin personeli (tekrarsız, kalem sırasıyla)
  @JsonKey(
    
    name: r'staffNames',
    required: true,
    includeIfNull: false,
  )


  final List<String> staffNames;



      /// Yalnız PendingApproval; bu anda yanıtlanmazsa otomatik iptal
  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? holdExpiresUtc;



      /// Yalnız kapora istenmiş randevuda bulunur
  @JsonKey(
    
    name: r'depositStatus',
    required: false,
    includeIfNull: false,
  unknownEnumValue: DepositStatus.unknownDefaultOpenApi,
  )


  final DepositStatus? depositStatus;





    @override
    bool operator ==(Object other) => identical(this, other) || other is TodayAppointment &&
      other.appointmentId == appointmentId &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.status == status &&
      other.displayStatus == displayStatus &&
      other.customerName == customerName &&
      other.isNewCustomer == isNewCustomer &&
      other.serviceSummary == serviceSummary &&
      other.staffNames == staffNames &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.depositStatus == depositStatus;

    @override
    int get hashCode =>
        appointmentId.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        status.hashCode +
        displayStatus.hashCode +
        customerName.hashCode +
        isNewCustomer.hashCode +
        serviceSummary.hashCode +
        staffNames.hashCode +
        (holdExpiresUtc == null ? 0 : holdExpiresUtc.hashCode) +
        depositStatus.hashCode;

  factory TodayAppointment.fromJson(Map<String, dynamic> json) => _$TodayAppointmentFromJson(json);

  Map<String, dynamic> toJson() => _$TodayAppointmentToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

