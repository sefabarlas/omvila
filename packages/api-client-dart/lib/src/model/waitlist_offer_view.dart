//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/waitlist_offer_status.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'waitlist_offer_view.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WaitlistOfferView {
  /// Returns a new [WaitlistOfferView] instance.
  WaitlistOfferView({

    required  this.id,

    required  this.status,

    required  this.startUtc,

    required  this.endUtc,

     this.staffId,

     this.staffName,

    required  this.offeredUtc,

    required  this.holdExpiresUtc,
  });

  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



  @JsonKey(
    
    name: r'status',
    required: true,
    includeIfNull: false,
  unknownEnumValue: WaitlistOfferStatus.unknownDefaultOpenApi,
  )


  final WaitlistOfferStatus status;



  @JsonKey(
    
    name: r'startUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime startUtc;



  @JsonKey(
    
    name: r'endUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime endUtc;



  @JsonKey(
    
    name: r'staffId',
    required: false,
    includeIfNull: false,
  )


  final String? staffId;



  @JsonKey(
    
    name: r'staffName',
    required: false,
    includeIfNull: false,
  )


  final String? staffName;



  @JsonKey(
    
    name: r'offeredUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime offeredUtc;



  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime holdExpiresUtc;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WaitlistOfferView &&
      other.id == id &&
      other.status == status &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.staffId == staffId &&
      other.staffName == staffName &&
      other.offeredUtc == offeredUtc &&
      other.holdExpiresUtc == holdExpiresUtc;

    @override
    int get hashCode =>
        id.hashCode +
        status.hashCode +
        startUtc.hashCode +
        endUtc.hashCode +
        (staffId == null ? 0 : staffId.hashCode) +
        (staffName == null ? 0 : staffName.hashCode) +
        offeredUtc.hashCode +
        holdExpiresUtc.hashCode;

  factory WaitlistOfferView.fromJson(Map<String, dynamic> json) => _$WaitlistOfferViewFromJson(json);

  Map<String, dynamic> toJson() => _$WaitlistOfferViewToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

