//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/sms_sender.dart';
import 'package:omvila_api/src/model/industry_code.dart';
import 'package:omvila_api/src/model/admin_credit_transaction.dart';
import 'package:omvila_api/src/model/admin_brand_detail_subscription.dart';
import 'package:omvila_api/src/model/admin_brand_detail_owner.dart';
import 'package:omvila_api/src/model/admin_brand_detail_appointments_this_month.dart';
import 'package:omvila_api/src/model/admin_brand_detail_credits.dart';
import 'package:omvila_api/src/model/branch_ref.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'admin_brand_detail.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class AdminBrandDetail {
  /// Returns a new [AdminBrandDetail] instance.
  AdminBrandDetail({

    required  this.id,

    required  this.name,

    required  this.slug,

    required  this.industryCode,

    required  this.city,

    required  this.brandStatus,

    required  this.createdUtc,

    required  this.bookingUrl,

    required  this.owner,

    required  this.branches,

    required  this.activeStaffCount,

    required  this.customerCount,

    required  this.googleCalendarConnectedStaffCount,

    required  this.smsSender,

    required  this.subscription,

    required  this.credits,

    required  this.appointmentsThisMonth,

    required  this.recentCreditTransactions,

    required  this.version,
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



      /// Bağlantı adı
  @JsonKey(
    
    name: r'slug',
    required: true,
    includeIfNull: false,
  )


  final String slug;



  @JsonKey(
    
    name: r'industryCode',
    required: true,
    includeIfNull: false,
  unknownEnumValue: IndustryCode.unknownDefaultOpenApi,
  )


  final IndustryCode industryCode;



      /// Ana şubenin ili
  @JsonKey(
    
    name: r'city',
    required: true,
    includeIfNull: true,
  )


  final String? city;



      /// Marka durumu
  @JsonKey(
    
    name: r'brandStatus',
    required: true,
    includeIfNull: false,
  unknownEnumValue: AdminBrandDetailBrandStatusEnum.unknownDefaultOpenApi,
  )


  final AdminBrandDetailBrandStatusEnum brandStatus;



      /// \"kayıt 11 Kasım\"
  @JsonKey(
    
    name: r'createdUtc',
    required: true,
    includeIfNull: false,
  )


  final DateTime createdUtc;



      /// \"Rezervasyon bağlantısı\"
  @JsonKey(
    
    name: r'bookingUrl',
    required: true,
    includeIfNull: false,
  )


  final String bookingUrl;



  @JsonKey(
    
    name: r'owner',
    required: true,
    includeIfNull: false,
  )


  final AdminBrandDetailOwner owner;



      /// \"Şubeler\"
  @JsonKey(
    
    name: r'branches',
    required: true,
    includeIfNull: false,
  )


  final List<BranchRef> branches;



      /// \"Personel · 6 aktif\"
          // minimum: 0
  @JsonKey(
    
    name: r'activeStaffCount',
    required: true,
    includeIfNull: false,
  )


  final int activeStaffCount;



      /// \"Müşteri kaydı\"
          // minimum: 0
  @JsonKey(
    
    name: r'customerCount',
    required: true,
    includeIfNull: false,
  )


  final int customerCount;



      /// \"Google Takvim · 3 personelde bağlı (tek yönlü)\"
          // minimum: 0
  @JsonKey(
    
    name: r'googleCalendarConnectedStaffCount',
    required: true,
    includeIfNull: false,
  )


  final int googleCalendarConnectedStaffCount;



  @JsonKey(
    
    name: r'smsSender',
    required: true,
    includeIfNull: false,
  )


  final SmsSender smsSender;



  @JsonKey(
    
    name: r'subscription',
    required: true,
    includeIfNull: false,
  )


  final AdminBrandDetailSubscription subscription;



  @JsonKey(
    
    name: r'credits',
    required: true,
    includeIfNull: false,
  )


  final AdminBrandDetailCredits credits;



  @JsonKey(
    
    name: r'appointmentsThisMonth',
    required: true,
    includeIfNull: false,
  )


  final AdminBrandDetailAppointmentsThisMonth appointmentsThisMonth;



      /// Son 30 günün kontör hareketleri (en fazla 10)
  @JsonKey(
    
    name: r'recentCreditTransactions',
    required: true,
    includeIfNull: false,
  )


  final List<AdminCreditTransaction> recentCreditTransactions;



      /// Kaynak sürümü (ETag ile aynı değer)
  @JsonKey(
    
    name: r'version',
    required: true,
    includeIfNull: false,
  )


  final String version;





    @override
    bool operator ==(Object other) => identical(this, other) || other is AdminBrandDetail &&
      other.id == id &&
      other.name == name &&
      other.slug == slug &&
      other.industryCode == industryCode &&
      other.city == city &&
      other.brandStatus == brandStatus &&
      other.createdUtc == createdUtc &&
      other.bookingUrl == bookingUrl &&
      other.owner == owner &&
      other.branches == branches &&
      other.activeStaffCount == activeStaffCount &&
      other.customerCount == customerCount &&
      other.googleCalendarConnectedStaffCount == googleCalendarConnectedStaffCount &&
      other.smsSender == smsSender &&
      other.subscription == subscription &&
      other.credits == credits &&
      other.appointmentsThisMonth == appointmentsThisMonth &&
      other.recentCreditTransactions == recentCreditTransactions &&
      other.version == version;

    @override
    int get hashCode =>
        id.hashCode +
        name.hashCode +
        slug.hashCode +
        industryCode.hashCode +
        (city == null ? 0 : city.hashCode) +
        brandStatus.hashCode +
        createdUtc.hashCode +
        bookingUrl.hashCode +
        owner.hashCode +
        branches.hashCode +
        activeStaffCount.hashCode +
        customerCount.hashCode +
        googleCalendarConnectedStaffCount.hashCode +
        smsSender.hashCode +
        subscription.hashCode +
        credits.hashCode +
        appointmentsThisMonth.hashCode +
        recentCreditTransactions.hashCode +
        version.hashCode;

  factory AdminBrandDetail.fromJson(Map<String, dynamic> json) => _$AdminBrandDetailFromJson(json);

  Map<String, dynamic> toJson() => _$AdminBrandDetailToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

/// Marka durumu
enum AdminBrandDetailBrandStatusEnum {
@JsonValue(r'Onboarding')
onboarding(r'Onboarding'),
@JsonValue(r'Active')
active(r'Active'),
@JsonValue(r'Suspended')
suspended(r'Suspended'),
@JsonValue(r'Archived')
archived(r'Archived'),
@JsonValue(r'unknown_default_open_api')
unknownDefaultOpenApi(r'unknown_default_open_api');

const AdminBrandDetailBrandStatusEnum(this.value);

final String value;

@override
String toString() => value;
}


