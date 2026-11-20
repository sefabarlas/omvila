//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'manage_create_services_from_templates_request_items_inner.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class ManageCreateServicesFromTemplatesRequestItemsInner {
  /// Returns a new [ManageCreateServicesFromTemplatesRequestItemsInner] instance.
  ManageCreateServicesFromTemplatesRequestItemsInner({

    required  this.templateId,

     this.price,

     this.durationMinutes,
  });

  @JsonKey(
    
    name: r'templateId',
    required: true,
    includeIfNull: false,
  )


  final String templateId;



  @JsonKey(
    
    name: r'price',
    required: false,
    includeIfNull: false,
  )


  final Money? price;



          // minimum: 5
          // maximum: 720
  @JsonKey(
    
    name: r'durationMinutes',
    required: false,
    includeIfNull: false,
  )


  final int? durationMinutes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is ManageCreateServicesFromTemplatesRequestItemsInner &&
      other.templateId == templateId &&
      other.price == price &&
      other.durationMinutes == durationMinutes;

    @override
    int get hashCode =>
        templateId.hashCode +
        price.hashCode +
        durationMinutes.hashCode;

  factory ManageCreateServicesFromTemplatesRequestItemsInner.fromJson(Map<String, dynamic> json) => _$ManageCreateServicesFromTemplatesRequestItemsInnerFromJson(json);

  Map<String, dynamic> toJson() => _$ManageCreateServicesFromTemplatesRequestItemsInnerToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

