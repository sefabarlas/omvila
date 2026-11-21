//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'my_commission_summary_next_accrual.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MyCommissionSummaryNextAccrual {
  /// Returns a new [MyCommissionSummaryNextAccrual] instance.
  MyCommissionSummaryNextAccrual({

    required  this.appointmentId,

    required  this.customerName,
  });

  @JsonKey(
    
    name: r'appointmentId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentId;



  @JsonKey(
    
    name: r'customerName',
    required: true,
    includeIfNull: false,
  )


  final String customerName;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MyCommissionSummaryNextAccrual &&
      other.appointmentId == appointmentId &&
      other.customerName == customerName;

    @override
    int get hashCode =>
        appointmentId.hashCode +
        customerName.hashCode;

  factory MyCommissionSummaryNextAccrual.fromJson(Map<String, dynamic> json) => _$MyCommissionSummaryNextAccrualFromJson(json);

  Map<String, dynamic> toJson() => _$MyCommissionSummaryNextAccrualToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

