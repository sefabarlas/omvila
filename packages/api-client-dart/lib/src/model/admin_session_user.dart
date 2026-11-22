//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_session_user.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminSessionUser {
  /// Returns a new [AdminSessionUser] instance.
  AdminSessionUser({

    required  this.id,

    required  this.fullName,

    required  this.phoneMasked,
  });

      /// Kullanıcı
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Ad
  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// Maskeli telefon
  @JsonKey(
    
    name: r'phoneMasked',
    required: true,
    includeIfNull: false,
  )


  final String phoneMasked;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminSessionUser &&
      other.id == id &&
      other.fullName == fullName &&
      other.phoneMasked == phoneMasked;

    @override
    int get hashCode =>
        id.hashCode +
        fullName.hashCode +
        phoneMasked.hashCode;

  factory AdminSessionUser.fromJson(Map<String, dynamic> json) => _$AdminSessionUserFromJson(json);

  Map<String, dynamic> toJson() => _$AdminSessionUserToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

