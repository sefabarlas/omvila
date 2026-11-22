//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_review_invite_staff.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicReviewInviteStaff {
  /// Returns a new [PublicReviewInviteStaff] instance.
  PublicReviewInviteStaff({

    required  this.name,

     this.photoUrl,
  });

  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'photoUrl',
    required: false,
    includeIfNull: false,
  )


  final String? photoUrl;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicReviewInviteStaff &&
      other.name == name &&
      other.photoUrl == photoUrl;

    @override
    int get hashCode =>
        name.hashCode +
        (photoUrl == null ? 0 : photoUrl.hashCode);

  factory PublicReviewInviteStaff.fromJson(Map<String, dynamic> json) => _$PublicReviewInviteStaffFromJson(json);

  Map<String, dynamic> toJson() => _$PublicReviewInviteStaffToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

