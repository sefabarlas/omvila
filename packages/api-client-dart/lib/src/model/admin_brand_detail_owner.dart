//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_detail_owner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandDetailOwner {
  /// Returns a new [AdminBrandDetailOwner] instance.
  AdminBrandDetailOwner({

    required  this.fullName,

    required  this.phoneMasked,
  });

      /// Sahibin adı
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
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandDetailOwner &&
      other.fullName == fullName &&
      other.phoneMasked == phoneMasked;

    @override
    int get hashCode =>
        fullName.hashCode +
        phoneMasked.hashCode;

  factory AdminBrandDetailOwner.fromJson(Map<String, dynamic> json) => _$AdminBrandDetailOwnerFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandDetailOwnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

