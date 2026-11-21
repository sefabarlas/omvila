//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/me_notification.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_notification_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeNotificationPage {
  /// Returns a new [MeNotificationPage] instance.
  MeNotificationPage({

    required  this.items,

    required  this.nextCursor,

    required  this.unreadCount,
  });

  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<MeNotification> items;



  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;



          // minimum: 0
  @JsonKey(
    
    name: r'unreadCount',
    required: true,
    includeIfNull: false,
  )


  final int unreadCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeNotificationPage &&
      other.items == items &&
      other.nextCursor == nextCursor &&
      other.unreadCount == unreadCount;

    @override
    int get hashCode =>
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode) +
        unreadCount.hashCode;

  factory MeNotificationPage.fromJson(Map<String, dynamic> json) => _$MeNotificationPageFromJson(json);

  Map<String, dynamic> toJson() => _$MeNotificationPageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

