//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_ref.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerRef {
  /// Returns a new [CustomerRef] instance.
  CustomerRef({

    required  this.brandCustomerId,

    required  this.fullName,

     this.phone,

     this.phoneMasked,
  });

  @JsonKey(
    
    name: r'brandCustomerId',
    required: true,
    includeIfNull: false,
  )


  final String brandCustomerId;



  @JsonKey(
    
    name: r'fullName',
    required: true,
    includeIfNull: false,
  )


  final String fullName;



      /// E.164 telefon
  @JsonKey(
    
    name: r'phone',
    required: false,
    includeIfNull: false,
  )


  final String? phone;



  @JsonKey(
    
    name: r'phoneMasked',
    required: false,
    includeIfNull: false,
  )


  final String? phoneMasked;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerRef &&
      other.brandCustomerId == brandCustomerId &&
      other.fullName == fullName &&
      other.phone == phone &&
      other.phoneMasked == phoneMasked;

    @override
    int get hashCode =>
        brandCustomerId.hashCode +
        fullName.hashCode +
        phone.hashCode +
        phoneMasked.hashCode;

  factory CustomerRef.fromJson(Map<String, dynamic> json) => _$CustomerRefFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerRefToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

