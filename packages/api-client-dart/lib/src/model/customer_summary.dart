//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/money.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'customer_summary.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class CustomerSummary {
  /// Returns a new [CustomerSummary] instance.
  CustomerSummary({

    required  this.id,

    required  this.fullName,

     this.phone,

     this.phoneMasked,

     this.birthYear,

    required  this.visitCount,

    required  this.noShowCount,

     this.lastVisitOn,

     this.nextAppointmentUtc,

     this.totalSpent,

     this.isNew,

     this.isNoShowRisk,

    required  this.tags,

     this.notes,
  });

      /// BrandCustomer kimliği
  @JsonKey(
    
    name: r'id',
    required: true,
    includeIfNull: false,
  )


  final String id;



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



  @JsonKey(
    
    name: r'birthYear',
    required: false,
    includeIfNull: false,
  )


  final int? birthYear;



  @JsonKey(
    
    name: r'visitCount',
    required: true,
    includeIfNull: false,
  )


  final int visitCount;



  @JsonKey(
    
    name: r'noShowCount',
    required: true,
    includeIfNull: false,
  )


  final int noShowCount;



  @JsonKey(
    
    name: r'lastVisitOn',
    required: false,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime? lastVisitOn;



  @JsonKey(
    
    name: r'nextAppointmentUtc',
    required: false,
    includeIfNull: false,
  )


  final DateTime? nextAppointmentUtc;



  @JsonKey(
    
    name: r'totalSpent',
    required: false,
    includeIfNull: false,
  )


  final Money? totalSpent;



  @JsonKey(
    
    name: r'isNew',
    required: false,
    includeIfNull: false,
  )


  final bool? isNew;



  @JsonKey(
    
    name: r'isNoShowRisk',
    required: false,
    includeIfNull: false,
  )


  final bool? isNoShowRisk;



  @JsonKey(
    
    name: r'tags',
    required: true,
    includeIfNull: false,
  )


  final List<String> tags;



  @JsonKey(
    
    name: r'notes',
    required: false,
    includeIfNull: false,
  )


  final String? notes;





    @override
    bool operator ==(Object other) => identical(this, other) || other is CustomerSummary &&
      other.id == id &&
      other.fullName == fullName &&
      other.phone == phone &&
      other.phoneMasked == phoneMasked &&
      other.birthYear == birthYear &&
      other.visitCount == visitCount &&
      other.noShowCount == noShowCount &&
      other.lastVisitOn == lastVisitOn &&
      other.nextAppointmentUtc == nextAppointmentUtc &&
      other.totalSpent == totalSpent &&
      other.isNew == isNew &&
      other.isNoShowRisk == isNoShowRisk &&
      other.tags == tags &&
      other.notes == notes;

    @override
    int get hashCode =>
        id.hashCode +
        fullName.hashCode +
        phone.hashCode +
        phoneMasked.hashCode +
        (birthYear == null ? 0 : birthYear.hashCode) +
        visitCount.hashCode +
        noShowCount.hashCode +
        (lastVisitOn == null ? 0 : lastVisitOn.hashCode) +
        (nextAppointmentUtc == null ? 0 : nextAppointmentUtc.hashCode) +
        totalSpent.hashCode +
        isNew.hashCode +
        isNoShowRisk.hashCode +
        tags.hashCode +
        (notes == null ? 0 : notes.hashCode);

  factory CustomerSummary.fromJson(Map<String, dynamic> json) => _$CustomerSummaryFromJson(json);

  Map<String, dynamic> toJson() => _$CustomerSummaryToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

