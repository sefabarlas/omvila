//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_overview_funnel.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminOverviewFunnel {
  /// Returns a new [AdminOverviewFunnel] instance.
  AdminOverviewFunnel({

    required  this.windowDays,

    required  this.signedUp,

    required  this.addedServices,

    required  this.setWorkingHours,

    required  this.firstOnlineAppointment,
  });

      /// Pencere (gün)
  @JsonKey(
    
    name: r'windowDays',
    required: true,
    includeIfNull: false,
  )


  final int windowDays;



      /// Kaydoldu
          // minimum: 0
  @JsonKey(
    
    name: r'signedUp',
    required: true,
    includeIfNull: false,
  )


  final int signedUp;



      /// Hizmet ekledi
          // minimum: 0
  @JsonKey(
    
    name: r'addedServices',
    required: true,
    includeIfNull: false,
  )


  final int addedServices;



      /// Çalışma saatlerini girdi
          // minimum: 0
  @JsonKey(
    
    name: r'setWorkingHours',
    required: true,
    includeIfNull: false,
  )


  final int setWorkingHours;



      /// İlk online randevu
          // minimum: 0
  @JsonKey(
    
    name: r'firstOnlineAppointment',
    required: true,
    includeIfNull: false,
  )


  final int firstOnlineAppointment;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminOverviewFunnel &&
      other.windowDays == windowDays &&
      other.signedUp == signedUp &&
      other.addedServices == addedServices &&
      other.setWorkingHours == setWorkingHours &&
      other.firstOnlineAppointment == firstOnlineAppointment;

    @override
    int get hashCode =>
        windowDays.hashCode +
        signedUp.hashCode +
        addedServices.hashCode +
        setWorkingHours.hashCode +
        firstOnlineAppointment.hashCode;

  factory AdminOverviewFunnel.fromJson(Map<String, dynamic> json) => _$AdminOverviewFunnelFromJson(json);

  Map<String, dynamic> toJson() => _$AdminOverviewFunnelToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

