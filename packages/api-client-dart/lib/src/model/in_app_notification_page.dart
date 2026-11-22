//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/in_app_notification.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'in_app_notification_page.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class InAppNotificationPage {
  /// Returns a new [InAppNotificationPage] instance.
  InAppNotificationPage({

    required  this.items,

    required  this.nextCursor,

    required  this.unreadCount,
  });

      /// Yeniden eskiye
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<InAppNotification> items;



      /// Sonraki sayfa imleci
  @JsonKey(
    
    name: r'nextCursor',
    required: true,
    includeIfNull: true,
  )


  final String? nextCursor;



      /// Zildeki rozet; sayfadan bağımsız toplam okunmamış
          // minimum: 0
  @JsonKey(
    
    name: r'unreadCount',
    required: true,
    includeIfNull: false,
  )


  final int unreadCount;





    @override
    bool operator ==(Object other) => identical(this, other) || other is InAppNotificationPage &&
      other.items == items &&
      other.nextCursor == nextCursor &&
      other.unreadCount == unreadCount;

    @override
    int get hashCode =>
        items.hashCode +
        (nextCursor == null ? 0 : nextCursor.hashCode) +
        unreadCount.hashCode;

  factory InAppNotificationPage.fromJson(Map<String, dynamic> json) => _$InAppNotificationPageFromJson(json);

  Map<String, dynamic> toJson() => _$InAppNotificationPageToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

