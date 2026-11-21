//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'unread_notification_count.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class UnreadNotificationCount {
  /// Returns a new [UnreadNotificationCount] instance.
  UnreadNotificationCount({

    required  this.unreadCount,
  });

      /// Zildeki rozet
          // minimum: 0
  @JsonKey(
    
    name: r'unreadCount',
    required: true,
    includeIfNull: false,
  )


  final int unreadCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is UnreadNotificationCount &&
      other.unreadCount == unreadCount;

    @override
    int get hashCode =>
        unreadCount.hashCode;

  factory UnreadNotificationCount.fromJson(Map<String, dynamic> json) => _$UnreadNotificationCountFromJson(json);

  Map<String, dynamic> toJson() => _$UnreadNotificationCountToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

