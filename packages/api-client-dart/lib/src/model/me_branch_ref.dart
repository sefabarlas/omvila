//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'me_branch_ref.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeBranchRef {
  /// Returns a new [MeBranchRef] instance.
  MeBranchRef({

    required  this.id,

    required  this.name,

    required  this.district,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;



  @JsonKey(
    
    name: r'district',
    required: true,
    includeIfNull: false,
  )


  final String district;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeBranchRef &&
      other.id == id &&
      other.name == name &&
      other.district == district;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        district.hashCode;

  factory MeBranchRef.fromJson(Map<String, dynamic> json) => _$MeBranchRefFromJson(json);

  Map<String, dynamic> toJson() => _$MeBranchRefToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

