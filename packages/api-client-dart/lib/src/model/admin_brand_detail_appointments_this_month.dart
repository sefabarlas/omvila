//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_detail_appointments_this_month.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandDetailAppointmentsThisMonth {
  /// Returns a new [AdminBrandDetailAppointmentsThisMonth] instance.
  AdminBrandDetailAppointmentsThisMonth({

    required  this.count,

    required  this.onlinePercent,
  });

      /// Bu ay randevu
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// Online oranı
          // minimum: 0
          // maximum: 100
  @JsonKey(
    
    name: r'onlinePercent',
    required: true,
    includeIfNull: true,
  )


  final num? onlinePercent;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandDetailAppointmentsThisMonth &&
      other.count == count &&
      other.onlinePercent == onlinePercent;

    @override
    int get hashCode =>
        count.hashCode +
        (onlinePercent == null ? 0 : onlinePercent.hashCode);

  factory AdminBrandDetailAppointmentsThisMonth.fromJson(Map<String, dynamic> json) => _$AdminBrandDetailAppointmentsThisMonthFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandDetailAppointmentsThisMonthToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

