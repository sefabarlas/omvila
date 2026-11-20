//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_session_user.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageSessionUser {
  /// Returns a new [ManageSessionUser] instance.
  ManageSessionUser({

    required  this.id,

    required  this.fullName,

    required  this.phoneMasked,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



  @JsonKey(
    
    name: r'phoneMasked',
    required: true,
    includeIfNull: false,
  )


  final String phoneMasked;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageSessionUser &&
      other.id == id &&
      other.fullName == fullName &&
      other.phoneMasked == phoneMasked;

    @override
    int get hashCode =>
        id.hashCode +
        fullName.hashCode +
        phoneMasked.hashCode;

  factory ManageSessionUser.fromJson(Map<String, dynamic> json) => _$ManageSessionUserFromJson(json);

  Map<String, dynamic> toJson() => _$ManageSessionUserToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

