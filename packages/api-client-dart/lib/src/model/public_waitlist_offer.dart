//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/waitlist_offer_status.dart';
import 'package:omvila_api/src/model/money.dart';
import 'package:omvila_api/src/model/public_waitlist_offer_items_inner.dart';
import 'package:omvila_api/src/model/public_waitlist_offer_brand.dart';
import 'package:omvila_api/src/model/public_waitlist_offer_branch.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'public_waitlist_offer.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class PublicWaitlistOffer {
  /// Returns a new [PublicWaitlistOffer] instance.
  PublicWaitlistOffer({

    required  this.id,

    required  this.status,

    required  this.brand,

    required  this.branch,

     this.customerFirstName,

    required  this.startUtc,

    required  this.endUtc,

    required  this.timeZone,

    required  this.holdExpiresUtc,

    required  this.generatedUtc,

    required  this.items,

    required  this.total,

    required  this.amountDue,

     this.requiresApproval,

     this.depositAmount,

     this.appointmentAccessCode,

    required  this.waitlistAccessCode,

    required  this.version,
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
    
    name: r'brand',
    required: true,
    includeIfNull: false,
  )


  final PublicWaitlistOfferBrand brand;



  @JsonKey(
    
    name: r'branch',
    required: true,
    includeIfNull: false,
  )


  final PublicWaitlistOfferBranch branch;



      /// \"Merve, beklediğiniz saat boşaldı\"
  @JsonKey(
    
    name: r'customerFirstName',
    required: false,
    includeIfNull: false,
  )


  final String? customerFirstName;



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



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



  @JsonKey(
    
    name: r'holdExpiresUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime holdExpiresUtc;



      /// Sunucunun \"şimdi\"si; geri sayım holdExpiresUtc − generatedUtc ile başlar (cihaz saati kullanılmaz)
  @JsonKey(
    
    name: r'generatedUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime generatedUtc;



  @JsonKey(
    
    name: r'items',
    required: true,
    includeIfNull: false,
  )


  final List<PublicWaitlistOfferItemsInner> items;



  @JsonKey(
    
    name: r'total',
    required: true,
    includeIfNull: false,
  )


  final Money total;



      /// \"Salonda ödenecek\"
  @JsonKey(
    
    name: r'amountDue',
    required: true,
    includeIfNull: false,
  )


  final Money amountDue;



      /// Kabulde randevu PendingApproval oluşur (salon onayı veya kapora)
  @JsonKey(
    
    name: r'requiresApproval',
    required: false,
    includeIfNull: false,
  )


  final bool? requiresApproval;



      /// Kabulde istenecek kapora; yoksa bulunmaz
  @JsonKey(
    
    name: r'depositAmount',
    required: false,
    includeIfNull: false,
  )


  final Money? depositAmount;



      /// Kabul edildiyse oluşan randevunun kodu (omvila.com/r/{code})
  @JsonKey(
    
    name: r'appointmentAccessCode',
    required: false,
    includeIfNull: false,
  )


  final String? appointmentAccessCode;



      /// \"Listeden çık\" için bekleme kaydının kodu
  @JsonKey(
    
    name: r'waitlistAccessCode',
    required: true,
    includeIfNull: false,
  )


  final String waitlistAccessCode;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is PublicWaitlistOffer &&
      other.id == id &&
      other.status == status &&
      other.brand == brand &&
      other.branch == branch &&
      other.customerFirstName == customerFirstName &&
      other.startUtc == startUtc &&
      other.endUtc == endUtc &&
      other.timeZone == timeZone &&
      other.holdExpiresUtc == holdExpiresUtc &&
      other.generatedUtc == generatedUtc &&
      other.items == items &&
      other.total == total &&
      other.amountDue == amountDue &&
      other.requiresApproval == requiresApproval &&
      other.depositAmount == depositAmount &&
      other.appointmentAccessCode == appointmentAccessCode &&
      other.waitlistAccessCode == waitlistAccessCode &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        status.hashCode +
        brand.hashCode +
        branch.hashCode +
        (customerFirstName == null ? 0 : customerFirstName.hashCode) +
        startUtc.hashCode +
        endUtc.hashCode +
        timeZone.hashCode +
        holdExpiresUtc.hashCode +
        generatedUtc.hashCode +
        items.hashCode +
        total.hashCode +
        amountDue.hashCode +
        requiresApproval.hashCode +
        depositAmount.hashCode +
        (appointmentAccessCode == null ? 0 : appointmentAccessCode.hashCode) +
        waitlistAccessCode.hashCode +
        version.hashCode;

  factory PublicWaitlistOffer.fromJson(Map<String, dynamic> json) => _$PublicWaitlistOfferFromJson(json);

  Map<String, dynamic> toJson() => _$PublicWaitlistOfferToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

