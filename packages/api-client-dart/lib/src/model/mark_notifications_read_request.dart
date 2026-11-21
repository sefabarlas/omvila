//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'mark_notifications_read_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MarkNotificationsReadRequest {
  /// Returns a new [MarkNotificationsReadRequest] instance.
  MarkNotificationsReadRequest({

     this.notificationIds,

     this.all,
  });

      /// Okundu sayılacak bildirimler
  @JsonKey(
    
    name: r'notificationIds',
    required: false,
    includeIfNull: false,
  )


  final Set<String>? notificationIds;



      /// \"Tümünü okundu say\"
  @JsonKey(
    
    name: r'all',
    required: false,
    includeIfNull: false,
  )


  final bool? all;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MarkNotificationsReadRequest &&
      other.notificationIds == notificationIds &&
      other.all == all;

    @override
    int get hashCode =>
        notificationIds.hashCode +
        all.hashCode;

  factory MarkNotificationsReadRequest.fromJson(Map<String, dynamic> json) => _$MarkNotificationsReadRequestFromJson(json);

  Map<String, dynamic> toJson() => _$MarkNotificationsReadRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

