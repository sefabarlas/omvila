//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'plan_ref.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PlanRef {
  /// Returns a new [PlanRef] instance.
  PlanRef({

    required  this.code,

    required  this.name,
  });

      /// Plan kodu (solo, salon, salon_plus)
  @JsonKey(
    
    name: r'code',
    required: true,
    includeIfNull: false,
  )


  final String code;



      /// Ekrandaki ad (Tekil, Salon, Salon Plus)
  @JsonKey(
    
    name: r'name',
    required: true,
    includeIfNull: false,
  )


  final String name;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PlanRef &&
      other.code == code &&
      other.name == name;

    @override
    int get hashCode =>
        code.hashCode +
        name.hashCode;

  factory PlanRef.fromJson(Map<String, dynamic> json) => _$PlanRefFromJson(json);

  Map<String, dynamic> toJson() => _$PlanRefToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

