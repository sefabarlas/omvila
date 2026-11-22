//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_status_change.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandStatusChange {
  /// Returns a new [AdminBrandStatusChange] instance.
  AdminBrandStatusChange({

    required  this.status,

    required  this.reason,
  });

      /// Yeni durum
  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminBrandStatusChangeStatusEnum.unknownDefaultOpenApi,
  )


  final AdminBrandStatusChangeStatusEnum status;



      /// Neden; denetime yazılır
  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  )


  final String reason;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandStatusChange &&
      other.status == status &&
      other.reason == reason;

    @override
    int get hashCode =>
        status.hashCode +
        reason.hashCode;

  factory AdminBrandStatusChange.fromJson(Map<String, dynamic> json) => _$AdminBrandStatusChangeFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandStatusChangeToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Yeni durum
enum AdminBrandStatusChangeStatusEnum {
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Suspended')
suspended(r'Suspended'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminBrandStatusChangeStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


