//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'me_reschedule_slot_search.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class MeRescheduleSlotSearch {
  /// Returns a new [MeRescheduleSlotSearch] instance.
  MeRescheduleSlotSearch({

    required  this.date,
  });

      /// Şubenin yerel günü
  @JsonKey(
    
    name: r'date',
    required: true,
    includeIfNull: false,
  )
  @DateOnlyConverter()


  final DateTime date;





    @override
    bool operator ==(Object other) => identical(this, other) || other is MeRescheduleSlotSearch &&
      other.date == date;

    @override
    int get hashCode =>
        date.hashCode;

  factory MeRescheduleSlotSearch.fromJson(Map<String, dynamic> json) => _$MeRescheduleSlotSearchFromJson(json);

  Map<String, dynamic> toJson() => _$MeRescheduleSlotSearchToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

