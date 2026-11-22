//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_ref.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandRef {
  /// Returns a new [AdminBrandRef] instance.
  AdminBrandRef({

    required  this.id,

    required  this.name,
  });

      /// Marka
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



      /// Marka adı
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandRef &&
      other.id == id &&
      other.name == name;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode;

  factory AdminBrandRef.fromJson(Map<String, dynamic> json) => _$AdminBrandRefFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandRefToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

