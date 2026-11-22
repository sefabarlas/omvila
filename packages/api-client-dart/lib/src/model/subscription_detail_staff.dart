//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'subscription_detail_staff.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class SubscriptionDetailStaff {
  /// Returns a new [SubscriptionDetailStaff] instance.
  SubscriptionDetailStaff({

    required  this.count,

    required  this.limit,
  });

      /// Etkin personel sayısı
          // minimum: 0
  @JsonKey(
    
    name: r'count',
    required: true,
    includeIfNull: false,
  )


  final int count;



      /// Plan personel sınırı; null = sınırsız veya tanımlı değil
          // minimum: 0
  @JsonKey(
    
    name: r'limit',
    required: true,
    includeIfNull: true,
  )


  final int? limit;





    @override
    bool operator ==(Object other) => identical(this, other) || other is SubscriptionDetailStaff &&
      other.count == count &&
      other.limit == limit;

    @override
    int get hashCode =>
        count.hashCode +
        (limit == null ? 0 : limit.hashCode);

  factory SubscriptionDetailStaff.fromJson(Map<String, dynamic> json) => _$SubscriptionDetailStaffFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionDetailStaffToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

