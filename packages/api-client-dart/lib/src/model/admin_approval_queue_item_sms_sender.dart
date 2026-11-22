//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_approval_queue_item_sms_sender.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminApprovalQueueItemSmsSender {
  /// Returns a new [AdminApprovalQueueItemSmsSender] instance.
  AdminApprovalQueueItemSmsSender({

    required  this.requestedName,

    required  this.currentEffectiveName,

    required  this.version,
  });

      /// Başvurulan ad
  @JsonKey(
    
    name: r'requestedName',
    required: true,
    includeIfNull: false,
  )


  final String requestedName;



      /// SMS'lerin şu an gittiği ad
  @JsonKey(
    
    name: r'currentEffectiveName',
    required: true,
    includeIfNull: false,
  )


  final String currentEffectiveName;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminApprovalQueueItemSmsSender &&
      other.requestedName == requestedName &&
      other.currentEffectiveName == currentEffectiveName &&
      other.version == version;

    @override
    int get hashCode =>
        requestedName.hashCode +
        currentEffectiveName.hashCode +
        version.hashCode;

  factory AdminApprovalQueueItemSmsSender.fromJson(Map<String, dynamic> json) => _$AdminApprovalQueueItemSmsSenderFromJson(json);

  Map<String, dynamic> toJson() => _$AdminApprovalQueueItemSmsSenderToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

