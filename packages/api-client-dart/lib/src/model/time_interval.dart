//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:copy_with_extension/copy_with_extension.dart';
import 'package:json_annotation/json_annotation.dart';
// ignore: unused_import
import 'package:omvila_api/src/date_only.dart';

part 'time_interval.g.dart';


@CopyWith()
@JsonSerializable(
  checked: true,
  createToJson: true,
  disallowUnrecognizedKeys: false,
  explicitToJson: true,
)
class TimeInterval {
  /// Returns a new [TimeInterval] instance.
  TimeInterval({

    required  this.start,

    required  this.end,
  });

      /// Yerel saat HH:mm (şubenin saat dilimine göre)
  @JsonKey(
    
    name: r'start',
    required: true,
    includeIfNull: false,
  )


  final String start;



      /// Yerel saat HH:mm (şubenin saat dilimine göre)
  @JsonKey(
    
    name: r'end',
    required: true,
    includeIfNull: false,
  )


  final String end;





    @override
    bool operator ==(Object other) => identical(this, other) || other is TimeInterval &&
      other.start == start &&
      other.end == end;

    @override
    int get hashCode =>
        start.hashCode +
        end.hashCode;

  factory TimeInterval.fromJson(Map<String, dynamic> json) => _$TimeIntervalFromJson(json);

  Map<String, dynamic> toJson() => _$TimeIntervalToJson(this);

  @override
  String toString() {
    return toJson().toString();
  }

}

