//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'walk_in_start_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WalkInStartRequest {
  /// Returns a new [WalkInStartRequest] instance.
  WalkInStartRequest({

     this.staffId,

     this.serviceId,
  });

      /// Bilette personel \"Fark etmez\" ise zorunlu
  @JsonKey(
    
    name: r'staffId',
    required: false,
    includeIfNull: false,
  )


  final String? staffId;



      /// Bilette hizmet yoksa zorunlu
  @JsonKey(
    
    name: r'serviceId',
    required: false,
    includeIfNull: false,
  )


  final String? serviceId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WalkInStartRequest &&
      other.staffId == staffId &&
      other.serviceId == serviceId;

    @override
    int get hashCode =>
        staffId.hashCode +
        serviceId.hashCode;

  factory WalkInStartRequest.fromJson(Map<String, dynamic> json) => _$WalkInStartRequestFromJson(json);

  Map<String, dynamic> toJson() => _$WalkInStartRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

