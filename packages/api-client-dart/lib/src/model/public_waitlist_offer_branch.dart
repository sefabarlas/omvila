//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'public_waitlist_offer_branch.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicWaitlistOfferBranch {
  /// Returns a new [PublicWaitlistOfferBranch] instance.
  PublicWaitlistOfferBranch({

    required  this.id,

    required  this.name,
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





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicWaitlistOfferBranch &&
      other.id == id &&
      other.name == name;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode;

  factory PublicWaitlistOfferBranch.fromJson(Map<String, dynamic> json) => _$PublicWaitlistOfferBranchFromJson(json);

  Map<String, dynamic> toJson() => _$PublicWaitlistOfferBranchToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

