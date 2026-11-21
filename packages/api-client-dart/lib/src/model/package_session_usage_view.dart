//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'package_session_usage_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PackageSessionUsageView {
  /// Returns a new [PackageSessionUsageView] instance.
  PackageSessionUsageView({

    required  this.appointmentId,

    required  this.serviceName,

    required  this.appointmentStartUtc,

    required  this.usedUtc,

     this.revertedUtc,
  });

  @JsonKey(
    
    name: r'appointmentId',
    required: true,
    includeIfNull: false,
  )


  final String appointmentId;



  @JsonKey(
    
    name: r'serviceName',
    required: true,
    includeIfNull: false,
  )


  final String serviceName;



  @JsonKey(
    
    name: r'appointmentStartUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime appointmentStartUtc;



  @JsonKey(
    
    name: r'usedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime usedUtc;



  @JsonKey(
    
    name: r'revertedUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? revertedUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PackageSessionUsageView &&
      other.appointmentId == appointmentId &&
      other.serviceName == serviceName &&
      other.appointmentStartUtc == appointmentStartUtc &&
      other.usedUtc == usedUtc &&
      other.revertedUtc == revertedUtc;

    @override
    int get hashCode =>
        appointmentId.hashCode +
        serviceName.hashCode +
        appointmentStartUtc.hashCode +
        usedUtc.hashCode +
        (revertedUtc == null ? 0 : revertedUtc.hashCode);

  factory PackageSessionUsageView.fromJson(Map<String, dynamic> json) => _$PackageSessionUsageViewFromJson(json);

  Map<String, dynamic> toJson() => _$PackageSessionUsageViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

