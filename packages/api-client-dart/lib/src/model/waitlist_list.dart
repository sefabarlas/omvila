//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:omvila_api/src/model/waitlist_entry_view.dart';
import 'package:omvila_api/src/model/waitlist_overview.dart';
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';

part 'waitlist_list.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class WaitlistList {
  /// Returns a new [WaitlistList] instance.
  WaitlistList({

    required  this.branchId,

    required  this.timeZone,

    required  this.generatedUtc,

    required  this.items,

    required  this.overview,
  });

  @JsonKey(
    
    name: r'branchId',
    required: true,
    includeIfNull: false,
  )


  final String branchId;



      /// IANA saat dilimi
  @JsonKey(
    
    name: r'timeZone',
    required: true,
    includeIfNull: false,
  )


  final String timeZone;



      /// Sunucunun \"şimdi\"si; kalan teklif süresi buna göre sayılır
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


  final List<WaitlistEntryView> items;



  @JsonKey(
    
    name: r'overview',
    required: true,
    includeIfNull: false,
  )


  final WaitlistOverview overview;





    @override
    bool operator ==(Object other) => identical(this, other) || other is WaitlistList &&
      other.branchId == branchId &&
      other.timeZone == timeZone &&
      other.generatedUtc == generatedUtc &&
      other.items == items &&
      other.overview == overview;

    @override
    int get hashCode =>
        branchId.hashCode +
        timeZone.hashCode +
        generatedUtc.hashCode +
        items.hashCode +
        overview.hashCode;

  factory WaitlistList.fromJson(Map<String, dynamic> json) => _$WaitlistListFromJson(json);

  Map<String, dynamic> toJson() => _$WaitlistListToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

