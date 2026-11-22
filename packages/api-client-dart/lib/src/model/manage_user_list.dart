//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/manage_user.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'manage_user_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageUserList {
  /// Returns a new [ManageUserList] instance.
  ManageUserList({

    required  this.items,
  });

      /// Ada göre sıralı
  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<ManageUser> items;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageUserList &&
      other.items == items;

    @override
    int get hashCode =>
        items.hashCode;

  factory ManageUserList.fromJson(Map<String, dynamic> json) => _$ManageUserListFromJson(json);

  Map<String, dynamic> toJson() => _$ManageUserListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

