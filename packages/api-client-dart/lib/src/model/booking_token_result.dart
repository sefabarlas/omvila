//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'booking_token_result.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class BookingTokenResult {
  /// Returns a new [BookingTokenResult] instance.
  BookingTokenResult({

    required  this.bookingToken,

    required  this.expiresInSeconds,

    required  this.phoneMasked,

     this.knownCustomerFirstName,
  });

  @JsonKey(
    
    name: r'bookingToken',
    required: true,
    includeIfNull: false,
  )


  final String bookingToken;



  @JsonKey(
    
    name: r'expiresInSeconds',
    required: true,
    includeIfNull: false,
  )


  final int expiresInSeconds;



  @JsonKey(
    
    name: r'phoneMasked',
    required: true,
    includeIfNull: false,
  )


  final String phoneMasked;



      /// \"Tekrar hoş geldiniz, Ayşe\"
  @JsonKey(
    
    name: r'knownCustomerFirstName',
    required: false,
    includeIfNull: false,
  )


  final String? knownCustomerFirstName;





    @override
    bool operator ==(Object other) => identical(this, other) || other is BookingTokenResult &&
      other.bookingToken == bookingToken &&
      other.expiresInSeconds == expiresInSeconds &&
      other.phoneMasked == phoneMasked &&
      other.knownCustomerFirstName == knownCustomerFirstName;

    @override
    int get hashCode =>
        bookingToken.hashCode +
        expiresInSeconds.hashCode +
        phoneMasked.hashCode +
        (knownCustomerFirstName == null ? 0 : knownCustomerFirstName.hashCode);

  factory BookingTokenResult.fromJson(Map<String, dynamic> json) => _$BookingTokenResultFromJson(json);

  Map<String, dynamic> toJson() => _$BookingTokenResultToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

