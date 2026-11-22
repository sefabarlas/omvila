//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'waitlist_offer_request.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WaitlistOfferRequest {
  /// Returns a new [WaitlistOfferRequest] instance.
  WaitlistOfferRequest({

    required  this.startUtc,

    required  this.staffId,
  });

  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'staffId',
    required: true,
    includeIfNull: false,
  )


  final String staffId;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WaitlistOfferRequest &&
      other.startUtc == startUtc &&
      other.staffId == staffId;

    @override
    int get hashCode =>
        startUtc.hashCode +
        staffId.hashCode;

  factory WaitlistOfferRequest.fromJson(Map<String, dynamic> json) => _$WaitlistOfferRequestFromJson(json);

  Map<String, dynamic> toJson() => _$WaitlistOfferRequestToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

