//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_sms_sender_rejection.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSmsSenderRejection {
  /// Returns a new [AdminSmsSenderRejection] instance.
  AdminSmsSenderRejection({

    required  this.reason,
  });

      /// Salonun göreceği ret nedeni
  @JsonKey(
    
    name: r'reason',
    required: true,
    includeIfNull: false,
  )


  final String reason;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSmsSenderRejection &&
      other.reason == reason;

    @override
    int get hashCode =>
        reason.hashCode;

  factory AdminSmsSenderRejection.fromJson(Map<String, dynamic> json) => _$AdminSmsSenderRejectionFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSmsSenderRejectionToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

